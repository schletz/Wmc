<?php
class ProductsController extends Controller
{
    public function get() {
        $this->viewData = array('message' => "Hello from Products Controller, Action get!");
    }
    
    public function getAllProducts() {  
        return [1,2,3];
    }
    
    public function post() {
        // Sendet einen Redirect. Das ist für Redirect after Post wichtig, damit beim
        // Aktualisieren die Daten nicht mehrmals in die Datenbank eingetragen werden.
        $this->redirect = "?controller=Products";
    }
}

