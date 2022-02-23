<?php
class StoresController extends Controller
{
    /**
     * Reagiert auf GET /?controller=stores
     */
    public function get()
    {
        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        return $this->view(['stores' => $db->query("SELECT * FROM Store")->fetchAll(PDO::FETCH_CLASS)]);
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
        return $this->view(['store' => $stmt->fetchAll(PDO::FETCH_CLASS)[0]], 'storedetails');
    }

    /**
     * Reagiert auf POST /?controller=stores&action=store
     */
    public function postStore()
    {
        $body = $this->body;
        if (strtotime($body->closedate) < time()) {
            return $this->view(['store' => $body, 'error' => "Das Datum muss in der Zukunft liegen."], 'storedetails');
            return;
        }

        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        try {
            $stmt = $db->prepare('UPDATE Store SET Name = :name, CloseDate = :closeDate WHERE Guid = :guid');
            $stmt->execute(['guid' => $body->guid, "name" => $body->name, "closeDate" => $body->closedate]);
            // Redirect after POST, sonst wird beim Aktualisieren das Formular nochmals gesendet.
            return $this->redirect("?controller=stores");
        } catch (Exception $e) {
            return $this->view(['store' => $body, 'error' => $e->getMessage()], 'storeDetails');
        }
    }

    /**
     * Reagiert auf GET /?controller=stores&action=allStores und liefert ein JSON zurück (keine View)
     */
    public function getAllStores()
    {
        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        return $this->ok($db->query("SELECT * FROM Store")->fetchAll(PDO::FETCH_CLASS));
    }
}
