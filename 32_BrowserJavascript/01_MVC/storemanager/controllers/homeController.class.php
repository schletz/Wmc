<?php
class HomeController extends Controller
{
    public function get() {
        return $this->view(array('message' => "Hello from Home Controller, Action get!"));
    }

    public function post() {
        return $this->view(array('message' => "Hello from Home Controller, Action post!"));
    }
}

