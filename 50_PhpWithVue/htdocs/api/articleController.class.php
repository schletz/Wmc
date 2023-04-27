<?php
require_once("controller.class.php");
class ArticleController extends Controller
{
    // Default Route (Request mit Parameter controller ohne method)
    // GET /api/?controller=article
    public function get()
    {
        // getData liefert ein JSON. Das wird als Antwort zurückgegeben.
        return $this->getData("SELECT Guid, Content FROM Article");
    }

    // GET /api/?controller=article&method=getArticleById&guid=46f5e025-fb38-4a28-83a2-692f96bf8174
    public function getArticleById()
    {
        $data = $this->getData(
            "SELECT Guid, Content FROM Article WHERE Guid = ?",
            array($this->getParams["guid"]),      // Queryparameter guid auslesen.
            asJson: false                         // Array statt JSON liefern.
        );

        if (count($data) != 1) $this->sendNotFoundAndExit();
        return json_encode($data[0]);
    }

    // GET /api/?controller=article&method=getArticleCount
    public function getArticleCount()
    {
        $data = $this->getData("SELECT COUNT(*) AS Count FROM Article", asJson: false);
        return json_encode(["count" => $data[0]["Count"]]);
    }

    /**
     * addUser
     * POST /api/?controller=article&method=addArticle
     * Legt einen Artikel in der Datenbank an. Die Daten müssen als JSON Request Body gesendet werden.
     * @return void
     */
    public function addArticle()
    {
        $authdata = $this->checkAuthentication();
        if (!$authdata) $this->sendUnauthorizedAndExit();

        $guid = $this->generateGuid();
        $this->getData(
            "INSERT INTO Article (Guid, Content) VALUES(?, ?)",
            array($guid, $this->requestBody->content)
        );
        // Die GUID an den Client senden.
        return json_encode(["guid" => $guid]);
    }
}
