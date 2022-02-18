<?php
abstract class Controller
{
    public string $viewName = "";      // Wird vom Router auf den Defaultwert (= Controllername) gesetzt.
    public array $viewData = array();  // Wird in der Variable $viewData in den Views bereitgestellt.
    public object $getParams;
    public object $body;               // Gelesener JSON Content (wenn vorhanden)
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
            $this->body = json_decode(file_get_contents("php://input"));
        }
        $this->getParams = (object) $_GET;
        $this->body = (object) $_POST;
    }

    public function ok($data) {
        return [
            'status' => 200,
            'data' => $data
        ];
    }

    public function badRequest($data) {
        return [
            'status' => 400,
            'data' => $data
        ];
    }

    public function redirect(string $location) {
        return [
            'status' => 302,
            'location' => $location
        ];
    }    

}
