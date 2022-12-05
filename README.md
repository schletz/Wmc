# Webentwicklung und Mobile Computing an der HTL Spengergasse

## Benötigte Software ([zur Anleitung](Software_Installation.md))

Installiere folgende Software:

- Installation von VS Code und Node.js für die Frontend Entwicklung
  - Wichtig bei der Installation von Node.js: Installiere den Server in *C:\nodejs* und füge
    mit der Option "Add to PATH" diesen Pfad zur Umgebungsvariable hinzu.
- XAMPP mit PHP
  - Von https://sourceforge.net/projects/xampp/files/XAMPP%20Windows/8.1.6/ die
    Datei *xampp-portable-windows-x64-8.1.6-0-VS16.7z* laden.
    Direktlink: https://downloads.sourceforge.net/project/xampp/XAMPP%20Windows/8.1.6/xampp-portable-windows-x64-8.1.6-0-VS16.7z?ts=gAAAAABjGJYFUsmmuPBXjV6Da59CzLHEtKEkGRAr7yWHvRN_RWdp3ljVfNM3cLJitAhKi_VZprTVY9d6dQVLNQAF_-ZRpFFDOg%3D%3D&r=https%3A%2F%2Fsourceforge.net%2Fprojects%2Fxampp%2Ffiles%2FXAMPP%2520Windows%2F8.1.6%2Fxampp-portable-windows-x64-8.1.6-0-VS16.7z%2Fdownload
  - Das 7z File öffnen und den xampp Ordner auf *C:* ziehen. Es muss jeder einen Ordner *C:\xampp*
    haben.
  - Im Startmenü "Systemumgebungsvariablen" eingeben. Danach auf "Umgebungsvariablen" klicken und
    im Bereich "System variables" zur Variable *PATH* den Wert *C:\xampp\php* hinzufügen. In
    der Konsole muss der Befehl *php --version* funktionieren.
  - Die Datei *xampp-control.exe* starten und den Server *Apache* starten.
  - Den *Inhalt* von *C:\xampp\htdocs* mit Shift+Entf löschen.
  - Nach dem Klonen des Repositories die Eingabeaufforderung als Admin öffnen. In
    *C:\xampp\htdocs* den Befehl
    `mklink /D storemanager C:\WMC\wmc\30_MVC\storemanager`
    eingeben.
- Einen (S)FTP Client wie WinSCP

## Inhalt 

### 1 Das MVC Konzept ([zum Inhalt](30_MVC))

- Entwicklung mit PHP und Apache
- Views und Routing
- Controller
- Datenbankzugriff

### 2 JavaScript Sprachgrundlagen ([zum Inhalt](31_JavaScript))

- Intro
- Variablen und Datentypen
- JSON, Arrays und Sets
- Funktionen
  - Callbacks und Closures
  - Prototype, this und new
  - Arrow functions und Arraymethoden
- Classes
- Modules

### 3 Eine Vue.js Applikation erstellen: Projekt Spengernews

- [HOWTO: Anlegen des Backends](32_Vuejs/01_Backend.md)
- [HOWTO: Anlegen von Controllern](32_Vuejs/02_Controller.md)
  
- [Teil 1: Anlegen der App](32_Vuejs/Teil01)
- [Teil 2: Scoped Styles und minimal API (.NET) als Backend](32_Vuejs/Teil02)
- [Teil 3: Clientseitiges Routing und DB Anbindung](32_Vuejs/Teil03)

### 4 Node.js Projekte ohne SPA Framework erstellen ([zum Inhalt](33_Webpack))

- Webpack
- EsLint
- Babel

## Nutzungsbedingungen

Dieses Repository kann frei im Unterricht verwendet werden. Bei der Verwendung von Unterlagen
oder Beispielen ist die Originalquelle (**https://github.com/schletz/Wmc**) anzugeben.

## Lehrplan

[Zum Lehrplan](Lehrplan.md)

## Klonen des Repositories
Installiere die neueste Version von git (Link: https://git-scm.com/downloads) mit den Standardeinstellungen. Gehe danach in die Windows Eingabeaufforderung (cmd) und führe in einem geeigneten Ordner (z. B. C:\WMC) den Befehl aus:
```
git clone https://github.com/schletz/Wmc.git
```

Soll der neueste Stand vom Server geladen werden, führe die Datei resetGit.cmd aus. Achtung: alle lokalen Änderungen werden dabei zurückgesetzt.
