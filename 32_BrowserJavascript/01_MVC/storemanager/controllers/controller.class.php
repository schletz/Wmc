<?php
abstract class Controller
{
    public string $viewName = "";      // Wird vom Router auf den Defaultwert (= Controllername) gesetzt.
    public array $viewData = array();  // Wird in der Variable $viewData in den Views bereitgestellt.
    public object $jsonBody;           // Gelesener JSON Content (wenn vorhanden)
    public string $redirect = "";      // Der Controller kann 302 redirect zu einer anderen Seite veranlassen.
    abstract public function get();

    public function post()
    {
    }
    public function put()
    {
    }
    public function delete()
    {
    }
    public function onExecute()
    {
    }
    public function readRequestBody()
    {
        if (isset($_SERVER["CONTENT_TYPE"]) && $_SERVER["CONTENT_TYPE"] == "application/json") {
            $this->jsonBody = json_decode(file_get_contents("php://input"));
        }
    }
}
