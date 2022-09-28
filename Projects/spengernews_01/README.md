# Projekt Spengernews Teil 1: Anlegen des Projektes

Ein neues Vue.js Projekt wird in der Konsole mit dem Befehl

```
npm init vue@3
```
angelegt. Es ist ein Assistent, bei dem wir folgende Einstellungen konfiguriert haben:

```
Vue.js - The Progressive JavaScript Framework

√ Project name: ... spengershop
√ Add TypeScript? ... No
√ Add JSX Support? ... No
√ Add Vue Router for Single Page Application development? ... Yes
√ Add Pinia for state management? ... No
√ Add Vitest for Unit Testing? ... No
√ Add Cypress for both Unit and End-to-End testing? ... No
√ Add ESLint for code quality? ... Yes
√ Add Prettier for code formatting? ... Yes
```

## Das Komponentenkonzept

Wir werden die Nachrichtenseite https://orf.at als Vorlage nehmen. Wir identifizieren mehrere
Bereiche:

![](components.png)

Diese Bereiche werden als *components* abgebildet. Eine Component ist ein eigenständiger Bereich

- der eine Darstellung (Rendering) besitzt.
- Parameter bekommen und events aussenden kann.
- Logik beinhaltet.
- Daten über fetch nachladen kann.

In Vue.js werden im Ordner *components* Dateien mit der Endung *.vue* angelegt. Sie müssen aus mehreren
Worten bestehen (also NewsFlash statt Newsflash), um eine Verwechslung mit html Elementen auszuschließen.

In der Datei [App.vue](src/App.vue) werden die Komponenten wie HTML Elemente eingebunden:

```javascript
<script setup>
import ImagePanel from './components/ImagePanel.vue'
import NewsFlash from './components/NewsFlash.vue'
</script>

<template>
  <h1>Spengernews</h1>
  <ImagePanel></ImagePanel>
  <NewsFlash></NewsFlash>  
</template>
```

## Wichtige npm Befehle

Du kannst in dem Verzeichnis, wo auch die Datei *package.json* ist, mehrere Kommandos ausführen.
Am Besten blende mit *CTRL+Ö* in VS Code das Terminal zum Ausführen der Befehle ein.

> Hinweis: Starte nicht mehrere Terminals gleichzeitig. Sonst kommt es zu einem Fehler, dass der
> Port für den dev Server schon belegt ist.

### npm install

Installiert alle Dependencies und kopiert sie in den Ordner *node_modules*. Dieser Ordner darf nicht
ins Repository kopiert werden, muss also über *.gitignore* ausgeschlossen werden.

### npm run dev

Startet den dev Server zum Testen der Applikation.

### npm run build

Erstellt im Ordner *dist* ein sogenanntes *bundle*. Es besteht aus einer index.html Datei, den
JS Dateien und CSS Dateien. Diese Dateien können über einen Webserver wie apache (XAMPP) einfach
ausgeliefert werden.
