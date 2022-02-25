<?php
/**
 * Modelklasse für einen Storeeintrag in der DB.
 * Wir verwenden keine Datentypen, da das Model durchaus uninitialisierte Werte haben kann
 * (wenn der User z. B. keine Eingaben schickt)
 * Achtung: Es wird die Option PDO::CASE_LOWER verwendet, daher ist alles kleinzuschreiben!
 */
class Store
{
    public $guid;
    public $name;
    public $closedate;
    public $url;
    public $managerid;

    public function validate() : object {
        $errors = new stdClass();
        if (!empty($this->closedate) && strtotime($this->closedate) < time()) {
            $errors->closedate = 'Das Datum liegt in der Vergangenheit.';
        }
        if (!preg_match("/^[^<>]{2,100}$/", $this->name)) {
            $errors->name = 'Der Name ist ungültig.';
        }        
        return $errors;
    }
}
