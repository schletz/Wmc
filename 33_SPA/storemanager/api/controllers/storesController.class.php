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
        return $this->view(['stores' => $db->query("SELECT s.*, u.Username FROM Store s LEFT JOIN User u ON (s.ManagerId = u.Id)")->fetchAll(PDO::FETCH_CLASS, 'Store')]);
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
        $store = $stmt->fetchAll(PDO::FETCH_CLASS, 'Store')[0];

        // Userliste für das Dropdown Feld lesen.
        $users = $db->query("SELECT Id, Username FROM User")->fetchAll(PDO::FETCH_CLASS);
        // Alle Spalten werden als lowercase in Properties gemappt (Konfiguration)
        return $this->view(['store' => $store, 'users' => $users], 'storedetails');
    }

    /**
     * Reagiert auf GET /?controller=stores&action=newStore
     */
    public function getNewStore()
    {
        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        // Userliste für das Dropdown Feld lesen.
        $users = $db->query("SELECT Id, Username FROM User")->fetchAll(PDO::FETCH_CLASS);
        return $this->view(['users' => $users], 'storedetails');
    }

    /**
     * Reagiert auf POST /?controller=stores&action=store
     */
    public function postStore()
    {
        // Auch bei Fehlern müssen wir die Userliste in viewData['user'] liefern, da sonst
        // das Dropdown nicht dargestellt werden kann.
        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        $users = $db->query("SELECT Id, Username FROM User")->fetchAll(PDO::FETCH_CLASS);

        // Eine Instanz von Store aus dem Request Body lesen.
        $store = $this->bind("Store");
        $validation = $store->validate();

        if (!empty((array)$validation)) {
            return $this->view(['store' => $store, 'validation' => $validation, 'users' => $users], "storedetails");
            return;
        }
        try {
            $this->upsertStore($store);
            // Redirect after POST, sonst wird beim Aktualisieren das Formular nochmals gesendet.
            return $this->redirect("?controller=stores");
        } catch (Exception $e) {
            return $this->view(['store' => $store, 'error' => $e->getMessage()], "storedetails");
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

    /**
     * Aktualisiert einen Store in der Datenbank oder fügt einen ein, wenn keine GUID geliefert
     * wurde.
     */
    private function upsertStore($store)
    {
        $isNewStore = !isset($store->guid);

        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        // INSERT or UPDATE?
        if ($isNewStore) {
            $store->guid = guid();
            // Wir lesen vom Model, daher sind alle Parameter (= Properties der Klasse Store) kleinzuschreiben!
            $stmt = $db->prepare('INSERT INTO Store (Guid, Name, CloseDate, Url, ManagerId) VALUES (:guid, :name, :closedate, :url, :managerid)');
            // Die Parameter müssen in ein Array konvertiert werden, sonst funktioniert das Mapping nicht.
            $stmt->execute((array) $store);
        } else {
            // Wir lesen vom Model, daher sind alle Parameter kleinzuschreiben!
            $stmt = $db->prepare('UPDATE Store SET Name = :name, CloseDate = :closedate, Url = :url, ManagerId = :managerid WHERE Guid = :guid');
            $stmt->execute((array) $store);
        }
    }
}
