<?php
require('models/store.class.php');
class StoresController extends Controller
{
    /**
     * Reagiert auf GET /?controller=stores
     */
    public function get()
    {
        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        // Instanzen der Modelklasse Store in models/store.class.php liefern.
        return $this->view(['stores' => $db->query("SELECT * FROM Store")->fetchAll(PDO::FETCH_CLASS, 'Store')]);
    }

    /**
     * Reagiert auf GET /?controller=stores&action=store
     */
    public function getStore()
    {
        if (!isset($this->getParams->guid)) return $this->redirect("?controller=stores");
        $guid = $this->getParams->guid;

        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        $stmt = $db->prepare('SELECT * FROM Store WHERE Guid = :guid');
        $stmt->execute(['guid' => $guid]);
        // Alle Spalten werden als lowercase in Properties gemappt (Konfiguration)
        return $this->view(['store' => $stmt->fetchAll(PDO::FETCH_CLASS)[0], 'Store'], 'storedetails');
    }

    /**
     * Reagiert auf GET /?controller=stores&action=store
     */
    public function getNewStore()
    {
        return $this->view([], 'newstore');
    }

    /**
     * Reagiert auf POST /?controller=stores&action=newStore
     */
    public function postNewStore()
    {
        return $this->postStore();
    }

    /**
     * Reagiert auf POST /?controller=stores&action=store
     */
    public function postStore()
    {
        // Eine Instanz von Store aus dem Request Body lesen.
        $store = $this->bind("Store");
        $isNewStore = !isset($store->guid);
        // Im Fehlerfall schicken leiten wir zu dieser View zurück.
        $viewName = $isNewStore ? "newstore" : "storedetails";

        if (!empty($store->closedate) && strtotime($store->closedate) < time()) {
            return $this->view(['store' => $store, 'error' => "Das Datum muss in der Zukunft liegen."], $viewName);
            return;
        }

        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        try {
            // INSERT or UPDATE?
            if ($isNewStore) {
                $store->guid = guid();
                // Wir lesen vom Model, daher sind alle Parameter (= Properties der Klasse Store) kleinzuschreiben!
                $stmt = $db->prepare('INSERT INTO Store (Guid, Name, CloseDate) VALUES (:guid, :name, :closedate)');
                // Die Parameter müssen in ein Array konvertiert werden, sonst funktioniert das Mapping nicht.
                $stmt->execute((array) $store);
            } else {
                // Wir lesen vom Model, daher sind alle Parameter kleinzuschreiben!
                $stmt = $db->prepare('UPDATE Store SET Name = :name, CloseDate = :closedate WHERE Guid = :guid');
                $stmt->execute((array) $store);
            }
            // Redirect after POST, sonst wird beim Aktualisieren das Formular nochmals gesendet.
            return $this->redirect("?controller=stores");
        } catch (Exception $e) {
            return $this->view(['store' => $store, 'error' => $e->getMessage()], $viewName);
        }
    }

    /**
     * Reagiert auf GET /?controller=stores&action=allStores und liefert ein JSON zurück (keine View)
     */
    public function getAllStores()
    {
        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        return $this->ok($db->query("SELECT * FROM Store")->fetchAll(PDO::FETCH_CLASS, 'Store'));
    }

    /**
     * Reagiert auf GET /?controller=stores&action=deleteStore
     */
    public function getDeleteStore()
    {
        if (!isset($this->getParams->guid)) return $this->redirect("?controller=stores");
        $guid = $this->getParams->guid;

        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        $stmt = $db->prepare('DELETE FROM Store WHERE Guid = :guid');
        $stmt->execute(['guid' => $guid]);
        return $this->redirect("?controller=stores");
    }    
}
