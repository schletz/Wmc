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
    der Konsole muss der Befehl *php --version* version.
    
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


### 3 JavaScript im Browser: Vue.js Templates ([zum Inhalt](32_VueJsTemplates))

- Das document object, HTMLElement Nodes
- Promises
- Laden mit fetch
- Templates mit Vue.js
- Komplexeres Statemanagement mit Vue.js


### 4 Node.js Projekte erstellen ([zum Inhalt](33_Webpack))

- Webpack
- EsLint
- Babel


### 5 Single Page Application Frameworks ([zum Inhalt](34_SPA))

- Komponenten entwickeln
- Wiederverwendbare Komponenten
- Kommunikation mit dem Backend
- Clientseitiges Routing
- State Management

## Nutzungsbedingungen

Dieses Repository kann frei im Unterricht verwendet werden. Bei der Verwendung von Unterlagen
oder Beispielen ist die Originalquelle (**https://github.com/schletz/Wmc**) anzugeben.

## Lehrplan

[Zum Lehrplan](Lehrplan.md)
