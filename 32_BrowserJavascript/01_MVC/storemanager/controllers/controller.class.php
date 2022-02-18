<?php
abstract class Controller
{
    public string $viewName = "";      // Wird vom Router auf den Defaultwert (= Controllername) gesetzt.
    public array $viewData = array();  // Wird in der Variable $viewData in den Views bereitgestellt.
    public object $getParams;
    public object $body;               // Gelesener JSON Content (wenn vorhanden)
    abstract public function get();

    function __construct()
    {
        if (isset($_SERVER["CONTENT_TYPE"]) && $_SERVER["CONTENT_TYPE"] == "application/json") {
            $this->body = json_decode(file_get_contents("php://input"));
            return;
        }
        $this->getParams = (object) $_GET;
        $this->body = (object) $_POST;
    }
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
    // public function readRequestBody()
    // {
    //     if (isset($_SERVER["CONTENT_TYPE"]) && $_SERVER["CONTENT_TYPE"] == "application/json") {
    //         $this->body = json_decode(file_get_contents("php://input"));
    //         return;
    //     }
    //     $this->getParams = (object) $_GET;
    //     $this->body = (object) $_POST;
    // }

    protected function ok($data)
    {
        return ['status' => 200, 'data' => $data];
    }

    protected function created($data)
    {
        return ['status' => 201, 'data' => $data];
    }

    protected function noContent()
    {
        return ['status' => 204];
    }

    protected function badRequest($data)
    {
        return ['status' => 400, 'data' => $data];
    }

    protected function redirect(string $location)
    {
        return ['status' => 302, 'location' => $location];
    }
}
