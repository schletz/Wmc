<?php
abstract class Controller
{
    // For hashing. GENERATE A NEW ONE!
    private $salt = "vAok7Z2KnQPn0AcHFsgI8qJlf6fDJffLqKkDs7v5xm8=";
    // MYSQL (sensitive informations, DO NOT PUBLISH):
    private $dbHost = "";
    private $dbName = "";
    private $dbUser = "";
    private $dbPass = "";
    // SQLITE
    private $sqliteDb = "users.db";
    // Accessible fields for your own controller.
    protected $dbConn = null;            // for $this->dbConn->lastInsertId()

    public $getParams;                   // array; for all GET parameters. Access with $this->getParams["name].
    public $postParams;                  // array; for all POST parameters (x-www-form-urlencoded)
    public $requestBody;                 // object; request body (for JSON Body requests). Access with $this->requestBody->propertyname.

    public function __construct()
    {
        $this->requestBody = new stdClass();
    }

    // Default method; called if there is no method query parameter in the request.
    abstract public function get();

    /**
     * connectToDb
     * Sets the database connection. If $sqliteDb is not empty, we will use a SQLite db.
     * Else we use MySQL with the configured credentials.
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
     * Executes a query and returns the result as JSON string or assotiative array is asJson is false.
     * @param string $query query SQL query to execute. Parameters are defined with a ? 
     * @example
     * $ctrl->getData("SELECT * FROM Persons WHERE P_ID = ? AND P_Vorname = ?", array(12, 'Max'))
     * @param array $param parameter Values for all parameters of the query.
     * @param bool $asJson If true, the result will returned as JSON string. Else as assotiative array. 
     * @returns If asJson is true a JSON Array with the result, if asJson is false an assotiative array.
     * @throws Exception Database exception.
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

    /**
     * Returns HTTP status 400 and exit script execution.
     */
    protected function sendBadRequestAndExit()
    {
        http_response_code(400);
        exit(0);
    }

    /**
     * Returns HTTP status 401 and exit script execution.
     * @param string $data Payload. Will be sent 1:1 to the client, should be a JSON string. 
     */
    protected function sendUnauthorizedAndExit($data = null)
    {
        http_response_code(401);
        if ($data !== null) echo $data;
        exit(0);
    }

    /**
     * Returns HTTP status 404 and exit script execution.
     * @param string $data Payload. Will be sent 1:1 to the client, should be a JSON string. 
     */
    protected function sendNotFoundAndExit($data = null)
    {
        http_response_code(404);
        if ($data !== null) echo $data;
        exit(0);
    }

    /**
     * Returns HTTP status 204 and exit script execution.
     */
    protected function sendNoContentAndExit()
    {
        http_response_code(204);
        exit(0);
    }

    /**
     * checkAuthentication
     * Checks whether a cookie with the name php_api_auth or a bearer token in the Authorization
     * Header has been submitted. The hash of the token or cookie is checked.
     * @return array Assotiatice array with the cookie or token payload.
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
     * Sends the set-cookie header to set a cookie.
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
        // To allow the devserver to access the cookie, we send the secure flag (HTTPS only)
        // and set the SameSite Policy to None. and set the SameSite Policy to None.
        setcookie("php_api_auth", $token, ["secure" => true, "expires" => $expires, "samesite" => "None"]);
    }

    /**
     * setDeleteCookieHeader
     * Sends an expired cookie to remove the cookie on the client.
     * @return void
     */
    protected function setDeleteCookieHeader()
    {
        setcookie("php_api_auth", "", ["secure" => true, "expires" => 0, "samesite" => "None"]);
    }

    /**
     * generateAuthToken
     * Creates a token that can then be sent as a bearer token in the Authorize header.
     * Can be used for an android, ... client.
     * @param  array $payload Array with the payload to be encoded.
     * @param  int $expires   UNIX Timestamp when the token should expire.
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
        // Send token or cookie as <data>.<hash>
        return base64_encode($data) . "." . base64_encode($hash);
    }

    /**
     * Generates a random 128bit UUID (v4)
     * @return string UUID formatted as hex string xxxx-xxx-xx... (36 chars)
     */
    protected function generateGuid()
    {
        $data = random_bytes(16);
        return vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(strtoupper(bin2hex($data)), 4));
    }

    private function checkAuthToken($token)
    {
        // Allow only base64 characters in data and hash section.
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
