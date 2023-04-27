<?php
abstract class Controller
{
    private $salt = "vAok7Z2KnQPn0AcHFsgI8qJlf6fDJffLqKkDs7v5xm8=";
    // MYSQL:
    private $dbHost = "";
    private $dbName = "";
    private $dbUser = "";
    private $dbPass = "";
    // SQLITE
    private $sqliteDb = "users.db";
    // Folgende Felder sind für die Implementierung eigener Controllerklassen interessant.
    protected $dbConn = null;            // für $this->dbConn->lastInsertId()

    public $getParams;                   // alle übergebenen GET Parameter
    public $postParams;                  // alle übergebenen POST Parameter (x-www-form-urlencoded)
    public $requestBody;                 // Request Body (bei JSON Body requests)

    public function __construct()
    {
        $this->requestBody = new stdClass();
    }

    // Defaultmethode, muss im eigenen Controller implementiert werden.
    abstract public function get();

    /**
     * connectToDb
     * Verbindet zur Datenbank und setzt die Membervariable dbConn.
     * @return void
     */
    private function connectToDb()
    {
        $this->dbConn = $this->sqliteDb
            ? new PDO("sqlite:{$this->sqliteDb}")
            : new PDO("mysql:host={$this->dbHost};dbname={$this->dbName};charset=utf8", $this->dbUser, $this->dbPass);
    }

    /**
     * getData
     * Führt eine Abfrage in der Datenbank durch und liefert das Ergebnis als JSON zurück. 
     * @param string query Die SQL Abfrage, die ausgeführt werden soll. Parameter können als ? 
     * angegeben werden. Diese werden dann aus dem Array befüllt.
     * @example
     * $ctrl->getData("SELECT * FROM Personen WHERE P_ID = ? AND P_Vorname = ?", array(12, 'Max'))
     * @param array parameter Ein Array mit den zu befüllenden Parametern.
     * @returns Ein JSON Objekt mit allen Daten. Dieses Objekt ist ein JSON Array, und jeder 
     * Datensatz wird als JSON Objekt mit dem Namen der Spalte zurückgegeben.
     * [{id:12, name:"Mustermann", vorname:"max"],{...},{...},...]
     * @throws Exception Meldung mit der errorInfo aus der Datenbank, falls die Abfrage misslingt.
     */
    protected function getData($query, $param = array(), $asJson = true)
    {
        if ($this->dbConn === null) $this->connectToDb();
        if (!is_array($param)) $param = array($param);
        $stmt = $this->dbConn->prepare($query);
        if ($stmt === false) {
            $err = $this->dbConn->errorInfo();
            throw new Exception("Datenbankfehler bei Prepare: {$err[2]}");
        }
        if ($stmt->execute($param) === false) {
            $err = $stmt->errorInfo();
            throw new Exception("Datenbankfehler bei der Abfrage: {$err[2]}");
        }
        $rows = $stmt->fetchAll(PDO::FETCH_ASSOC);
        return $asJson ? json_encode($rows, JSON_THROW_ON_ERROR) : $rows;
    }

    protected function sendBadRequestAndExit()
    {
        http_response_code(400);
        exit(0);
    }

    protected function sendUnauthorizedAndExit($data = null)
    {
        http_response_code(401);
        if ($data !== null) echo $data;
        exit(0);
    }

    protected function sendNotFoundAndExit($data = null)
    {
        http_response_code(404);
        if ($data !== null) echo $data;
        exit(0);
    }

    protected function sendNoContentAndExit()
    {
        http_response_code(204);
        exit(0);
    }

    /**
     * checkAuthentication
     * Prüft, ob ein Cookie mit dem Namen php_api_auth oder ein Bearer Token im Authorization Header
     * übermittelt wurde. Der Hash des Tokens oder des Cookies wird geprüft.
     * @return object Objekt mit den Daten des Cookies oder des Tokens.
     */
    protected function checkAuthentication()
    {
        $payload = false;
        $headers = apache_request_headers();
        if (isset($_COOKIE["php_api_auth"])) {
            $payload = $this->checkAuthToken($_COOKIE["php_api_auth"]);
        }
        if (isset($headers['Authorization'])) {
            if (preg_match('/^Bearer\s(?<token>.+)/', trim($headers['Authorization']), $match))
                $payload = $this->checkAuthToken($match["token"]);
        }
        if ($payload === false) {
            http_response_code(401);
            exit(0);
        }
        return $payload;
    }

    /**
     * setCookieHeader
     * Sendet den set-cookie header, um ein Cookie zu setzen.
     * @param  array $payload Array mit dem zu codierenden Payload.
     * @return void
     */
    protected function setCookieHeader($payload)
    {
        if (gettype($payload) != "array") {
            throw new Exception("Payload is not an array.");
        }
        $expires = time() + 60 * 60 * 3;
        $token = $this->generateAuthToken($payload, $expires);
        // Damit der Devserver auch auf das Cookie zugreifen kann, senden wir das secure flag (nur HTTPS)
        // und setzen die SameSite Policy auf None.
        setcookie("php_api_auth", $token, ["secure" => true, "expires" => $expires, "samesite" => "None"]);
    }

    /**
     * setDeleteCookieHeader
     * Sendet ein expired Cookie, um das Cookie am Client zu löschen.
     * @return void
     */
    protected function setDeleteCookieHeader()
    {
        setcookie("php_api_auth", "", ["secure" => true, "expires" => 0, "samesite" => "None"]);
    }

    /**
     * generateAuthToken
     * Erstellt einen Token, der dann als Bearer Token im Authorize Header gesendet werden kann.
     * Wird für den Android Client verwendet.
     * @param  array $payload Array mit dem zu codierenden Payload.
     * @param  int $expires UNIX Timestamp, wann der Token ablaufen soll.
     * @return void
     */
    protected function generateAuthToken($payload, $expires)
    {
        if (gettype($payload) != "array") {
            throw new Exception("Payload is not an array.");
        }
        $payload["expires"] = $expires;
        $data = json_encode($payload);
        $hash = crypt($data, $this->salt);
        return base64_encode($data) . "." . base64_encode($hash);
    }

    protected function generateGuid()
    {
        $data = random_bytes(16);
        return vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(strtoupper(bin2hex($data)), 4));
    }

    private function checkAuthToken($token)
    {
        if (!preg_match("/^(?<data>[-A-Za-z0-9+\\/]+={0,3})\\.(?<hash>[-A-Za-z0-9+\\/]+={0,3})$/", $token, $match)) {
            return false;
        }
        $data = base64_decode($match["data"]);
        $hash = base64_decode($match["hash"]);
        if (crypt($data, $this->salt) != $hash) return false;
        $decodedData = json_decode($data);
        if ($decodedData->expires < time()) return false;
        return $decodedData;
    }
}
