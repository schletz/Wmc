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

    protected function view($data = null, $viewName = "")
    {
        if (preg_match('/^[a-z]{0,100}$/', $viewName) !== 1)
            throw new InvalidArgumentException("Der Viewname $viewName darf nur aus Kleinbuchstaben bestehen.");

        $this->escapeData($data);
        return ['data' => $data, 'viewName' => $viewName];
    }

    protected function ok($data)
    {
        $this->escapeData($data);
        return ['status' => 200, 'data' => $data];
    }

    protected function created($data)
    {
        $this->escapeData($data);
        return ['status' => 201, 'data' => $data];
    }

    protected function noContent()
    {
        return ['status' => 204];
    }

    protected function badRequest($data = null)
    {
        if (isset($data)) return ['status' => 400, 'data' => $data];
        return ['status' => 400];
    }

    protected function redirect(string $location)
    {
        return ['status' => 302, 'location' => $location];
    }

    /**
     * Wandelt HTML Sonderzeichen für die Ausgabe in HTML Entities um. Das ist sehr wichtig,
     * sonst kann ein User "<script>...</script> in die Textfelder schreiben.
     */
    private function escapeData(&$data)
    {
        if (is_string($data)) {
            $data = htmlspecialchars($data);
            return;
        }
        $isArrayObj = is_object($data) << 1 | is_array($data);  // Bitmaske: Objekt oder Array
        if (!$isArrayObj) return;
        foreach ($data as $key => $val) {
            if ($isArrayObj & 1) {                    // Array? (1. Bit gesetzt)
                $this->escapeData($data[$key]);
            }
            if ($isArrayObj & 2) {                    // Object? (2. Bit gesetzt)
                $this->escapeData($data->{$key});
            }
        }
    }
}
