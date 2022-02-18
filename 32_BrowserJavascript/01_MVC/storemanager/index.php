<?php

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
 * Falls die View eine Datei (viewname).php.css besitzt, schreiben wir einen HTML Link zu
 * diesem CSS. Es kann im Layout im head Teil aufgerufen werden. Das ist nötig, da CSS nicht
 * im Body definiert werden darf.
 */
function writeCssLink()
{
    // CSS Laden. Existiert ein File im Views Ordner mit dem Namen (viewname).php.css, so wird
    // darauf verwiesen. Da dies im Header sein muss, braucht es diese Logik.
    $cssFile = "views/{$GLOBALS['viewName']}.php.css";
    if (file_exists($cssFile)) {
        echo "<link rel=\"stylesheet\" href=\"{$cssFile}\" />";
    }
}

/**
 * Schreibt die entsprechende View. Wird keine Datei mit dem Namen views/viewname.php gefunden,
 * wird die Datei notFount.php geschrieben.
 */
function renderBody()
{
    // Damit die View auf die Variable $viewData zugreifen kann, muss sie hier definiert werden.
    $viewData = $GLOBALS['controllerInstance']->viewData;
    // Sucht im Ordner Views nach dem festgelegten Viewnamen. Er ist standardmäßig der
    // Controllername, außer eine Controllermethode überschreibt $this->viewName
    $filename = "views/{$GLOBALS['viewName']}.php";
    if (file_exists($filename)) {
        require($filename);
    } else {
        require('notFound.php');
    }
}
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
 * Liefert die Action Methode Daten zurück, so wirddas Ergebnis der Action Methode als JSON
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

require('controllers/controller.class.php');

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
$controllerInstance->readRequestBody();
$controllerInstance->onExecute();
$response = $controllerInstance->$method();

// Falls der Controller eine Redirect URL gesetzt hat, senden wir 302 redirect und beenden.
if (isset($response) && $response['status'] == 302) {
    header("Location: {$response['location']}");
    exit(0);
}

// Die Action Methode liefert Daten zurück? Dann geben wir sie einfach als bei komplexen Typen
// als JSON aus (sonst als Text) und beenden.
if (isset($response) && isset($response['data'])) {
    http_response_code(isset($response['status']) ? $response['status'] : 200);
    if (is_array($response['data']) || is_object($response['data']))
        header('Content-Type: application/json; charset=utf-8');
    echo json_encode($response['data']);
    exit(0);
}

$viewName = empty($controllerInstance->viewName) ? $controller : $controllerInstance->viewName;

// Damit die Auswirkungen - falls Schadcode injected wird - abgefangen werden sagen wir dem Browser
// welche Features unsere Seite braucht. So kann z. B. nicht durch ein Script der Standort
// abgefragt werden, da wir dies in den Permissions schon ausschließen.
header("X-Frame-Options: DENY");
header("X-Content-Type-Options: nosniff");
header("Referrer-Policy: no-referrer");
header("Permissions-Policy: accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
// https://wiki.selfhtml.org/wiki/Sicherheit/Content_Security_Policy
// 'unsafe-eval' wird für VueJS Templates in HTML Files benötigt.
header("Content-Security-Policy: default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data:");

require('layout.php');
