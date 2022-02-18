<?php
class ServiceProvider
{
    private array $services = array();

    function addService($id, $factory) 
    {
        $this->services[$id] = $factory;
    }

    function getInstance($id) 
    {
        return $this->services[$id]();
    }
}
?>