<?php
class StoresController extends Controller
{
    public function get() {
        $this->viewData = array('message' => "Hello from Stores Controller, Action get!");
    }

    public function getAllStores() {  
        return [1,2,3];
    }

    public function post() {

    }
}

