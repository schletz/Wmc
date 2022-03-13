<?php
class Database
{
    private string $connectionString;
    function __construct($connectionString)
    {
        $this->connectionString = $connectionString;
    }
    
}
