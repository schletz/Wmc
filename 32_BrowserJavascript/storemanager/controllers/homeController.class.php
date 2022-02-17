<?php
class HomeController extends Controller
{
    public function get() {
        $this->viewData = array('message' => "Hello from Home Controller, Action get!");
    }

    public function post() {
        $this->viewData = array('message' => "Hello from Home Controller, Action post!");
    }
}

