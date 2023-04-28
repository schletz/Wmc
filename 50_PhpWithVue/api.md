# PHP API

Im Ordner *htdocs/api* liegt die API für die Single Page App.
Mit dem Aufruf *http://localhost/api/?controller=article* kann sie getestet werden.

## Datenbankzugriff

In der Datei [controller.class.php](htdocs/api/controller.class.php) kann der Datenbankzugriff konfiguriert werden.
In der Demo ist eine SQLite Datenbank konfiguriert, es kann auch eine MySQL Datenbank verwendet werden.
Setze dafür die Verbindungsdaten und setze die Variable *sqliteDb* auf einen Leerstring.

> **Vorsicht:** Die Datei enthält mit dem Verbindungsstring sensible Daten.
> Achte darauf, dass sie nicht in ein öffentliches Repo hochgeladen wird.

Als Erweiterung kannst du auch die Konfiguration von einer externen Datei (z. B. config.php und config.localhost.php) für Production und Development laden.

Ebenfalls im Controller ist eine Variable *salt*.
Sie wird zum Hashen des Cookies verwendet (*crypt* Funktion in php).
Der Inhalt sollte neu generiert werden (z. B. mit https://generate.plus/en/base64) *und darf natürlich auch nicht veröffentlicht werden*.

## Controller

Der Controller *ArticleController* stellt z. B. Methoden zur Bearbeitung der *Article* Tabelle bereit:

**htdocs/api/articleController.class.php**
```php
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
```

## Verwenden der API

#### Requests der Form GET */api/?controller=(controllername)*

Es wird die *get()* Methode des Entsprechenden Controllers (*testController.class.php*) aufgerufen und das Ergebnis zurückgeliefert.
*http://localhost/api/?controller=article* liefert zum Beispiel eine Liste aller Artikel.

#### Requests der Form GET */api/?controller=(controllername)&method=(methodenname)*

Soll eine bestimmte Methoden des Controllers zugegriffen werden, kann mit dem Parameter *method* die Methode angegeben werden.
So ruft z. B. *http://localhost/api/?controller=article&method=getArticleCount* die Methode *getArticleCount* auf, die die Anzahl der Artikel als JSON zurückgibt.


#### Requests der Form GET */api/?controller=(controllername)&method=(methodenname)&(variables)*

Es können andere GET Parameter angegeben werden.
Diese können im Controller mit `$this->getParams["<var_name>"]` angesprochen werden.
So liefert *http://localhost/api/?controller=article&method=getArticleById&guid=46f5e025-fb38-4a28-83a2-692f96bf8174* den entsprechenden Artikel.

#### POST Request mit JSON Body

Die Single Page App sendet POST Daten als JSON im Request Body.
Der Request Body kann mit `$this->requestBody` angesprochen werden.

## Schreiben eines neuen Controllers

1. Kopiere die Klasse *articleController.class.php* und benenne sie um.
   Der Name vor Controller (in diesem Fall test) wird beim Parameter *controller* angegeben.
2. Füge in der *index.php* den Controller mit *require_once* ein.

## Authentication

Die API bietet 2 Möglichkeiten, sich zu Authentifizieren:
- Über ein Cookie.
- Über einen Auth Token im Authorization Header.

### Cookie based

Um die Cookie Authentication zu testen, starte Postman.
Sende einen POST Request an https://localhost/api/?controller=user&method=addUser mit folgenden Daten (*Body* - *raw* - *JSON*):

```json
{
    "username": "user01",
    "password": "1111",
    "firstname": "firstname",
    "lastname": "lastname",
    "email": "e@mail"
}
```

Sende danach einen POST Request an https://localhost/api/?controller=user&method=login mit folgenden Daten (*Body* - *raw* - *JSON*):

```json
{
    "username": "user01",
    "password": "1111"
}
```

Als Antwort wird ein Cookie gesetzt.
Ein GET Request an *https://localhost/api/?controller=user&method=getProfile* zeigt nun den angemeldeten User an.

### Bearer Token

Die Android App kann einen Token anfordern.
Sende dafür in Postman an die Adresse *https://localhost/api/?controller=user&method=getAuthToken* den selben Payload wie bei *Cookies*.
Als Antwort wird eine JSON Response mit dem Property *authToken* gesendet.

```json
{
    "authToken": "eyJ1c2VyIjoiU2FseVN0YW0iLCJmaXJzdG5hbWUiOiJTYWx5IiwibGFzdG5hbWUiOiJTdGFtIiwicm9sZSI6MCwiZXhwaXJlcyI6MTY4MjM0MTA4MH0=.dkE4Q09RSzQ0cjBBSQ=="
}
```

Dieser Token kann dann in Postman unter *Authorization* als Token im GET Request an *https://localhost/api/?controller=user&method=getProfile* übermittelt werden. 
![](postman_token_1201.png)

Der Token ist 3 Stunden gültig. Dies kann im [userController.class.php](htdocs/api/userController.class.php) in der Methode *getAuthToken()* gesetzt werden.
