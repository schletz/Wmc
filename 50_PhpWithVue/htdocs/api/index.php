<?php
error_reporting(E_ALL ^ E_NOTICE);
// Throw all errors and warnings as error to send it as json in catch.
set_error_handler(function ($errno, $errstr, $errfile, $errline) {
    // error was suppressed with the @-operator
    if (error_reporting() === 0) return false;
    throw new ErrorException($errstr, 0, $errno, $errfile, $errline);
});
// TODO: Alle Controller einbinden, die vorhanden sind.
require_once("articleController.class.php");
require_once("userController.class.php");

header('Content-type: application/json; charset=utf-8');  // Send all data with JSON Header.
$headers = apache_request_headers();
// Devserver, die auf localhost:port bzw. 127.0.0.x laufen, das Verwenden der Cookies und CORS erlauben.
if (isset($headers["Origin"]) && preg_match("#^(?<origin>https?://(127.\d{1,3}.\d{1,3}.\d{1,3}|localhost)(:\d{3,5})?)#", $headers["Origin"], $match)) {
    header("Access-Control-Allow-Origin: {$match["origin"]}");
    header("Access-Control-Allow-Credentials: true");
    header("Access-Control-Allow-Headers: content-type");
    // Preflight POST Requests müssen mit HTTP/1.1 200 OK beantwortet werden.
    if ($_SERVER['REQUEST_METHOD'] == "OPTIONS") exit(0);
}

try {
    if (isset($_GET['controller']) && preg_match("/^[A-Za-z][A-Za-z0-9_]+$/", $_GET['controller'])) {
        $controllerName = $_GET['controller'] . "Controller";
    } else {
        throw new Exception("Der Parameter controller wurde nicht angegeben oder ist ungültig.");
    }
    /* Instanziert den Controller mit folgendem Namen: {controller}Controller, also z. B. 
     * SchuelerController, wenn controller=Schueler ist. */
    if (!class_exists($controllerName)) {
        http_response_code(404);
        echo "Die Klasse {$controllerName} existiert nicht oder wurde nicht registriert.";
        exit(0);
    }

    $ctrl = new $controllerName();
    if (!is_subclass_of($ctrl, "Controller")) {
        http_response_code(404);
        echo "Die Klasse {$controllerName} ist kein Controller.";
        exit(0);
    }
    // Query Parameter, POST Parameter (formdata) und Response Body auslesen
    $ctrl->getParams = $_GET;
    $ctrl->postParams = $_POST;
    $body = file_get_contents('php://input');
    if ($body !== false) {
        $json_body = json_decode(file_get_contents('php://input'));
        if ($json_body) $ctrl->requestBody = $json_body;
    }
    /* Wurde ein Methodenname übergeben? Wenn ja, wird diese Methode aufgerufen. Falls nicht, wird
     * die Methode get() aufgerufen. */
    if (!isset($_GET['method'])) {
        $methodName = "get";
    } else if (preg_match("/^[A-Za-z][A-Za-z0-9_]+$/", $_GET['method'])) {
        $methodName = $_GET['method'];
    } else {
        http_response_code(400);
        echo "Der Parameter method hat einen ungültigen Wert.";
        exit(0);
    }

    if (!method_exists($ctrl, $methodName)) {
        http_response_code(404);
        echo "Die Methode {$methodName} existiert nicht im {$controllerName}.";
        exit(0);
    }

    /* Methode aufrufen und die JSON Ausgabe 1:1 ausgeben */
    $data = $ctrl->$methodName();
    // Methode lieferte Content? Dann senden wir diesen 1:1.
    if (gettype($data) === "string") {
        echo $data;
        exit(0);
    }
} catch (Exception $err) {
    http_response_code(500);
    echo json_encode(array(
        'error' => $err->getMessage(),
        'file' => $err->getFile(),
        'line' => $err->getLine(),
        'code' => $err->getCode(),
        'trace' => $err->getTrace()
    ), JSON_UNESCAPED_UNICODE);
}
