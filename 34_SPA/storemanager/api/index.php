<?php
require('serviceProvider.class.php');
require('controllers/controller.class.php');

/**
 * Replacement für die UUID Funktion, da die interne Funktion nicht immer neue Werte liefert.
 */
function guid() {
    $data = random_bytes(16);
    return vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(strtoupper(bin2hex($data)), 4));
}

/**
 * Liest einen GET Parameter, der den Controller oder die Action definiert. Er darf nur aus
 * Buchstaben bestehen. Das ist wichtig, da der Parameter auch zum Laden von Files verwendet
 * wird. Ist der Wert ungültig oder nicht vorhanden, wird der defaultValue
 * zurückgegeben.
 */
function readParam($paramName, $defaultValue)
{
    if (!isset($_GET[$paramName])) return $defaultValue;
    if (preg_match('/^[a-zA-Z]{1,100}$/', $_GET[$paramName]) !== 1) return $defaultValue;
    return $_GET[$paramName];
}
/**
 * Falls die View eine Datei (viewname).php.css besitzt, schreiben wir den Inhalt des CSS Files
 * in den Head. Das ist nötig, da CSS nicht im Body definiert werden darf.
 */
function writeCss()
{
    // CSS Laden. Existiert ein File im Views Ordner mit dem Namen (viewname).php.css, so wird
    // darauf verwiesen. Da dies im Header sein muss, braucht es diese Logik.
    $cssFile = "views/{$GLOBALS['viewName']}.php.css";
    if (!file_exists($cssFile)) return;
    $content = file_get_contents($cssFile);
    echo "<style>";
    echo $content;
    echo "</style>";
}

/**
 * Schreibt die entsprechende View. Wird keine Datei mit dem Namen views/viewname.php gefunden,
 * wird die Datei notFount.php geschrieben.
 */
function renderBody()
{
    // Damit die View auf die Variable $viewData zugreifen kann, muss sie hier definiert werden.
    $viewData = $GLOBALS['viewData'];
    // Sucht im Ordner Views nach dem festgelegten Viewnamen. Er ist standardmäßig der
    // Controllername, außer eine Controllermethode überschreibt $this->viewName
    $filename = "views/{$GLOBALS['viewName']}.php";
    if (file_exists($filename)) {
        require($filename);
    } else {
        require('notFound.php');
    }
}

// *************************************************************************************************
// SERVICES REGISTRIEREN
// *************************************************************************************************

$services = new ServiceProvider();
// Wichtig C:\xampp\php zur PATH Variable hinzufügen. php -v in der Konsole muss
// funktionieren. Dann Apache neu starten. Sonst wird das Modul nicht gefunden.
// Unter Ubuntu muss mit sudo apt-get install -y php-pdo-sqlite das Modul installiert werden.
$services->addService('db', function () {
    $pdo = new PDO('sqlite:stores.db');
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->setAttribute(PDO::ATTR_CASE, PDO::CASE_LOWER);
    return $pdo;
});


/**
 * Liest den Request und instanziert den entsprechenden Controller. Dabei werden folgende
 * GET Parameter gelesen:
 *    controller: Der Name des Controllers, der instanziert werden soll. Dabei wird das Wort
 *                Controller angehängt.
 *    action: Die Methode im Controller, die aufgerufen werden soll. Dabei wird die HTTP Methode
 *            vorangestellt.
 * 
 * Bevor die Methoden aufgerufen werden, wird readRequestBody() und onExecute() des Controllers
 * aufgerufen. In onExecute kann Code untergebracht werden, der bei jeder Requestart ausgeführt
 * wird (vgl. Filter in ASP.NET)
 * 
 * Liefert die Action Methode Daten zurück, so wird das Ergebnis der Action Methode als JSON
 * zurückgegben (also eine API).
 * Ansonsten wird der Inhalt des Properties $viewData in die Variable $viewData geschrieben,
 * damit die einzelnen Views darauf zugreifen können. Der Inhalt des Properties $viewName wird
 * in die Variable $viewName geschrieben. Wird sie im Controller nicht gesetzt, so ist der 
 * Name des Controllers der Viewname.
 * 
 * Beispiele: 
 *     GET /
 *         Instanziert den HomeController in controllers/homeController.class.php und ruft get()
 *         bei einem GET Request auf (oder post(), put(), delete())
 *     GET /?controller=Stores
 *         Instanziert den StoresController in controllers/storesController.class.php.
 *     GET /?controller=Stores&action=AllStores
 *         Instanziert den StoresController in controllers/storesController.class.php und
 *         ruft die Methode getAllStores() auf.
 * 
 */

$controller = readParam('controller', 'home');
$action = readParam('action', '');

$filename = strtolower($controller) . "Controller.class.php";
if (!file_exists("controllers/{$filename}")) {
    $controller = 'home';
    $filename = 'homeController.class.php';
}
$controllerClass = "{$controller}Controller";
// Baut z. B. aus GET Request + Parameter action=AllStores den Wert getAllStores
$method = strtolower($_SERVER['REQUEST_METHOD']) . $action;

// Den entsprechenden Controller suchen, instanzieren und die entsprechende Methode aufrufen.
require("controllers/{$filename}");
$controllerInstance = new $controllerClass;
$response = $controllerInstance->onExecute();
if (!isset($response))
    $response = $controllerInstance->$method();

// Die Action Methode einen Statuscode mit ok, ... zurück? Dann geben wir sie einfach
// bei komplexen Typen als JSON aus (sonst als Text) und beenden.
if (isset($response) && isset($response['status'])) {
    http_response_code($response['status']);
    if ($response['status'] == 302) {
        header("Location: {$response['location']}");
        exit(0);
    }
    // Auf die API dürfen auch Server von localhost zugreifen. Dies ist für dev Server wichtig.
    // Kann in Production weggenommen werden, da der Origin nach Belieben gesendet werden kann.
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Allow-Headers#example_preflight_request
    if (isset($_SERVER['HTTP_ORIGIN']) && strpos($_SERVER['HTTP_ORIGIN'], 'localhost') !== false) {
        header("Access-Control-Allow-Origin: {$_SERVER['HTTP_ORIGIN']}");
        header("Access-Control-Allow-Methods: GET, POST, PUT, DELETE");
        header("Access-Control-Allow-Headers: Content-Type, x-requested-with");
    }
    if (!isset($response['data'])) exit(0);
    if (is_array($response['data']) || is_object($response['data']))
        header('Content-Type: application/json; charset=utf-8');
    echo json_encode($response['data']);
    exit(0);
}
