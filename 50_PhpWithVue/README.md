# Vue.js mit einem PHP Backend

## Vorbereitung

### PHP Umgebung

- Installiere XAMPP über den Installer in das Verzeichnis *C:\\xampp*.
- Füge den Pfad *C:\\xampp\\php* in die PATH Variable hinzu (Systemumgebungsvariablen editieren).

Zur Kontrolle führe in der Konsole den Befehl `php --version` aus.
Er muss die aktuelle PHP Version (8) anzeigen.

#### Zertifikat generieren

Ersetze die Datei *C:\\xampp\\apache\\conf\\openssl.cnf* durch [diese Version](openssl.conf).
Gehe danach in die Konsole und führe den folgenden Befehl aus:

```
C:\xampp\apache\bin\openssl req -config C:\xampp\apache\conf\openssl.cnf -new -sha256 -newkey rsa:2048 -nodes -keyout C:\xampp\apache\conf\ssl.key\server.key -x509 -days 3650 -out C:\xampp\apache\conf\ssl.crt\server.crt
```

Es können alle Nachfragen nach City, etc. mit dem Defaultwert (Enter drücken) beantwortet werden.

Damit der Browser auch dem Zertifikat vertraut, musst du es in Windows installieren.
Klicke dafür doppelt auf die Datei *server.crt* in *C:\\xampp\\apache\\conf*.
Wähle den Punkt *Install Certificate...*.
Bestätige danach die Einstellung *Current User*.
Nun musst du den Zertifikat Store ändern. Wähle *Trusted Root Certification Authorities* (vertrauenswürdige Stammzertifizierungsstellen).
Wenn der Apache Server läuft, musst du ihn neu starten.
Die Seite *https://localhost* muss *ohne Zertifikatswarnung* ausgeliefert werden.

### VS Code

- Installiere die Extension *PHP Intelephense*
- Setze in der *settings.json* Datei (*F1* - *Preferences: Open User Settings (JSON)*) die folgende Einstellung:

```json
"php.validate.executablePath": "C:\\xampp\\php\\php.exe"
```

### Umleiten des htdocs Ordners

Erstelle einen Ordner *C:\\Github* und klone dein Projekt dort hinein (oder erstelle ein neues Repo).
Verknüpfe nun den htdocs Ordner deiner XAMPP Installation mit dem Repository.
Starte dafür als Administrator die Windows Konsole.

```
cd \xampp
rd /S /Q htdocs
mklink /D htdocs C:\Github\<your_repo>\htdocs
```

Wenn du im Explorer auf den Ordner C:\xampp\htdocs klickst, muss der Inhalt des Ordners C:\Github\(your_repo)\htdocs erscheinen.

### Build der Vue.js App

Die Vue.js App wird im Ordner *spa* mit `npm i && npm run build` erstellt.
Der Buildvorgang kopiert die Applikation in den Ordner *htdocs/spa*.
Die Applikation wird dann von Apache (XAMPP) unter *https://localhost* ausgeliefert.

### API

Siehe Dokument [api.md](api.md).

