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

    }
}

