<?php
class ProductsController extends Controller
{
    // GET /?controller=Products
    public function get()
    {
        $this->viewData = array('message' => "Hello from Products Controller, Action get!");
    }

    // GET /?controller=Products&action=AllProducts
    public function getAllProducts()
    {
        return $this->ok([1, 2, 3]);
    }

    // GET /?controller=Products&action=BadRequestDemo
    public function getBadRequestDemo()
    {
        return $this->badRequest(['name' => 'Invalid name']);
    }

    // POST /?controller=Products
    public function post()
    {
        // Sendet einen Redirect. Das ist für Redirect after Post wichtig, damit beim
        // Aktualisieren die Daten nicht mehrmals in die Datenbank eingetragen werden.
        return $this->redirect("?controller=Products");
    }
}
