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
        $this->viewData['stores'] = $db->query("SELECT * FROM Store")->fetchAll(PDO::FETCH_CLASS);
    }

    /**
     * Reagiert auf GET /?controller=stores&action=store
     */
    public function getStore()
    {
        // Sonst würden wir die View store (= controllername) laden.
        $this->viewName = "storeDetails";
        if (!isset($this->getParams->guid)) return $this->redirect("?controller=stores");
        $guid = $this->getParams->guid;

        /** @var PDO $db */
        $db = $GLOBALS['services']->getInstance('db');
        $stmt = $db->prepare('SELECT * FROM Store WHERE Guid = :guid');
        $stmt->execute(['guid' => $guid]);
        // Alle Spalten werden als lowercase in Properties gemappt (Konfiguration)
        $this->viewData['store'] = $stmt->fetchAll(PDO::FETCH_CLASS)[0];
    }

    /**
     * Reagiert auf POST /?controller=stores&action=store
     */
    public function postStore()
    {
        $body = $this->body;
        $this->viewName = "storeDetails";
        // Die gesendeten Daten dem User zurückschicken, sonst gehen bei einem Fehler die
        // Daten verloren.
        $this->viewData['store'] = $body;
        if (strtotime($body->closedate) < time()) {
            $this->viewData['error'] = "Das Datum muss in der Zukunft liegen.";
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
            $this->viewData['error'] = $e->getMessage();
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
