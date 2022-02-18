<?php
abstract class Controller
{
    protected object $getParams;
    protected object $body;               // Gelesener JSON Content (wenn vorhanden)
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

    protected function view($data = null, $viewName = "") {
        return ['data' => $data, 'viewName' => $viewName];
    }

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
