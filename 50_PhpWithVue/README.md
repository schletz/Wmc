# Vue.js mit einem PHP Backend

## Vorbereitung

### Erstellen des Containers für den PHP Server

Im Verzeichnis *php-docker* ist ein konfiguriertes Dockerfile für einen nginx Server mit PHP Unterstützung.
Starte das Skript **[start_container.sh](start_container.sh).**
Wenn du [git](https://git-scm.com/downloads) installiert hast, dann sind *sh* Dateien automatisch im Explorer mit der git bash verknüpft, die das Skript ausführen kann.
Es erstellt das nginx Containerimage, erstellt die Vue.js App und startet dann den Container in Docker Desktop.

Der Container mounted das Verzeichnis *htdocs* als Volume in das interne wwwroot Verzeichnis.
Änderst du in *htdocs* Dateien, werden diese auch aktualisiert vom Server ausgeliefert.

### Zertifikat vertrauen

Klicke doppelt auf die Datei *php-docker/certs/server.crt* in diesem Repository.
Wähle *Install Certificate...* und *Current User*.
Nun musst du den Zertifikat Store ändern. Wähle *Trusted Root Certification Authorities* (vertrauenswürdige Stammzertifizierungsstellen).


### PHP installieren

Lade zuerst von [php.net](https://windows.php.net/download#php-8.2) die PHP Version 8.2.
Verwende den Download *VS16 x64 Thread Safe* und *Zip*.
Entpacke das Archiv in *C:\\php* und füge diesen Ordner zur PATH Variable hinzu.
Benenne danach die Datei *C:\\php\\php.ini-development* in *php.ini* um.
Öffne diese Datei in VS Code (oder einem Editor) und aktiviere die Zeile *extension=pdo_sqlite*, indem du den Strichpunkt vorher entfernst.
In der Konsole muss der Befehl *php -v* die Meldung "PHP 8.2.x" zeigen (und nicht command not found).

### VS Code Extensions für Vue.js und PHP

- Installiere die Extension *[PHP Intelephense](https://marketplace.visualstudio.com/items?itemName=bmewburn.vscode-intelephense-client)*.
- Installiere die Extension *[Vue Language Features (Volar)](https://marketplace.visualstudio.com/items?itemName=Vue.volar)*.
- Installiere die Extension *[ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)*.


### Build der Vue.js App

Die Vue.js App liegt im Ordner *vue_client*
Mit `npm i && npm run build` kann sie erstellt und mit nginx ausgeliefert werden.
Der Buildvorgang kopiert die Applikation in den Ordner *htdocs/app*.
Die Applikation wird dann von nginx unter *https://localhost* ausgeliefert.

## API

Siehe Dokument [api.md](api.md).

## Neues Zertifikat generieren (nicht erforderlich)

Falls du xampp installiert hast, kannst du auch das verwendete Zertifikat ändern.
Das ist für Entwicklungszwecke aber nicht nötig, es wird hier nur der Vollständigkeit halber erklärt.

Für diesen Schritt brauchst du das Programm *openssl*.
Es wird auf *C:\\xampp* verwiesen, da das Paket viele schon installiert haben und openssl darin enthalten ist.

Ersetze die Datei *C:\\xampp\\apache\\conf\\openssl.cnf* durch [diese Version](openssl.conf).
Gehe danach in die Konsole und führe den folgenden Befehl aus:

```
C:\xampp\apache\bin\openssl req -config C:\xampp\apache\conf\openssl.cnf -new -sha256 -newkey rsa:2048 -nodes -keyout server.key -x509 -days 3650 -out server.crt
```

Es können alle Nachfragen nach City, etc. mit dem Defaultwert (Enter drücken) beantwortet werden.

Nun kannst du die 2 erstellten Dateien (*server.crt* und *server.key*) in *php-docker/certs* verschieben und mit *docker build* das Image neu erstellen.
