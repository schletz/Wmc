<?php
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

require_once('controllers/controller.class.php');

$controller = isset($_GET['controller']) ? $_GET['controller'] : 'home';
if (preg_match('/^[A-Z][a-zA-Z]{1,100}$/', $controller) !== 1) {
    $controller = 'home';
}
$action = isset($_GET['action']) ? $_GET['action'] : '';
if (preg_match('/^[A-Z][a-zA-Z]{1,100}$/', $action) !== 1) {
    $action = '';
}

$filename = strtolower($controller) . "Controller.class.php";
if (!file_exists("controllers/{$filename}")) {
    $controller = 'home';
    $filename = 'homeController.class.php';
}
$controllerClass = "{$controller}Controller";
$method = strtolower($_SERVER['REQUEST_METHOD']) . $action;

require("controllers/{$filename}");
$controllerInstance = new $controllerClass;
$controllerInstance->readRequestBody();
$controllerInstance->onExecute();
$data = $controllerInstance->$method();

if (isset($data)) {
    header('Content-Type: application/json; charset=utf-8');
    echo json_encode($controllerInstance->$method());
    exit(0);
}

$viewData = $controllerInstance->viewData;
$viewName = empty($controllerInstance->viewName) ? $controller : $controllerInstance->viewName;
?>
