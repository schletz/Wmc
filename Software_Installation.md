# Installation der benötigten Software

## Node.js

Lade von https://nodejs.org/en/ die LTS Version von Node.js. Beachte bei der Installation folgendes:
- Installiere den Server unter Windows in *C:\\nodejs* und nicht in einem tief verschachtelten Verzeichnis.
- Verwende für die Installation unter macOS die Software Homebrew (https://brew.sh/). Danach kann
  mit *brew install node* im Terminal Node.js installiert werden.

Am Ende der Installation sollte der Befehl *node --version* die aktuelle Version ausgeben:

```
C:\Users\MyUser>node --version
v16.13.1
```

## XAMPP

Lade die ZIP Version von XAMPP und entpacke den Inhalt in das Verzeichnis *C:\xampp*. Die Datei
*apache_start.bat* muss sich in *C:\xampp* befinden (und nicht in *C:\xampp\unterordner*).

Füge danach *C:\xampp\php* zur Pfad Umgebungsvariable vo Windows hinzu, sonst funktioniert das
Laden der Module nicht.

Prüfe danach, ob in der Konsole der Befehl *php -v* die Versionsnummer anzeigt.

## Visual Studio Code

Zum Entwickeln von JavaScript Code gibt es natürlich viele IDEs und Editoren. Wir werden Visual
Studio Code verwenden. Lade daher von https://code.visualstudio.com/ den Editor für dein
Betriebssystem herunter.

> **Hinweis:** Aktiviere beim Setup die Option
>  *Add "Open with Code" action to Windows Explorer file context menu* und
>  *Add "Open with Code" action to Windows Explorer directory context menu*.
> Da Node.js Projekte nicht als Einzeldatei geöffnet werden können, ist diese Option sehr hilfreich!

Danach installiere über das Extension Menü die folgenden Extensions:

- *Vetur* für die Entwicklung von Vue.js Applikationen.
- *PHP Extension Pack*
- *PHP Intelephense*

Öffne nun die Einstellungen (Drücke *F1* oder *SHIFT+CMD+P* für die Menüzeile. Gib dann  
*settings* ein und wähle den Punkt *Preferences: Open Settings (JSON)*. Füge die folgenden
Einstellungen in die Datei ein und speichere sie ab:

```json
"php.executablePath": "C:/xampp/php",
"php.validate.executablePath": "C:/xampp/php/php.exe",
```
