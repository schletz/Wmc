<?php
class ProductsController extends Controller
{
    // GET /?controller=Products
    public function get()
    {
        return $this->view("Hello from Products Controller mit <script>bösem Script</script>, Action get!");
    }

    // GET /?controller=Products&action=AllProducts
    // Liefert HTTP 200 und ein Array mit 3 Zahlen.
    public function getAllProducts()
    {
        return $this->ok([1, 2, 3]);
    }

    // POST /?controller=Products&action=BadRequestDemo
    // Liefert HTTP 400 (Bad Request) mit Daten, die als JSON ausgegeben werden.
    public function postBadRequestDemo()
    {
        return $this->badRequest(['name' => 'Invalid name']);
    }

    // POST /?controller=Products&action=BadRequestDemo
    // Liefert HTTP 400 (Bad Request) mit Daten, die als JSON ausgegeben werden.
    public function postBadRequestMessageDemo()
    {
        return $this->badRequest("Oops");
    }


    // POST /?controller=Products
    public function post()
    {
        // Sendet einen Redirect. Das ist für Redirect after Post wichtig, damit beim
        // Aktualisieren die Daten nicht mehrmals in die Datenbank eingetragen werden.
        return $this->redirect("?controller=Products");
    }

    public function postCheckBody()
    {
        return $this->ok($this->body);
    }

    public function onExecute() {

    }
}
