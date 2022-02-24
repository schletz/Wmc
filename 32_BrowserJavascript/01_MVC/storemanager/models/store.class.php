<?php
/**
 * Modelklasse für einen Storeeintrag in der DB.
 * Achtung: Es wird die Option PDO::CASE_LOWER verwendet, daher ist alles kleinzuschreiben!
 */
class Store
{
    public string $guid;
    public string $name;
    public ?string $closedate;
}
?>