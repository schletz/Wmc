# Ein Node.js Projekt anlegen

Als erstes Projekt in Node.js legen wir eine Applikation an, die COVID QR Codes einlesen und
dekodieren soll.

![](screenshot2.png)

Auf [qr.schletz.webspace.spengergasse.at](https://qr.schletz.webspace.spengergasse.at)
ist eine Livedmo der Applikation abrufbar.

## Starten des Projektes

Im Ordner [CovidQrDecoder](CovidQrDecoder) befindet sich ein fertig implementiertes Projekt.
Gehe in das Verzeichnis mit der Datei *package.json* und führe die folgenden Befehle aus:

```
npm install
npm run serve
```

Auf *https://localhost:9000* wird nun der Dev Server mit der Applikation gestartet.

Zum Verständnis werden die erforderlichen Schritte um ein Projekt mit Webpack zu erstellen 
in den nachfolgenden Punkten erklärt.

## Öffnen und starten von node Projekten

Öffne das Projekt **immer mit "Open Folder"** in VS Code. Öffne keine Einzeldateien, da sonst
die anderen Dateien im Projekt nicht eingelesen werden. Die Konsole wird in VS Code mit *CTRL+Ö*
geöffnet. Hier könenn npm Befehle oder andere Befehle eingegeben werden.

Jedes Node.js Projekt besitzt ein Verzeichnis *node_modules* zum Speichern der installierten
Pakete, die von der Applikation benötigt werden. Wird ein Node.js Projekt geklont, muss
mit

```
npm install
```

dieses Verzeichnis neu erstellt und die Pakete geladen werden. Das Verzeichnis *node_modules*
sollte daher nicht in einem Repository liegen (verwende *.gitignore*) und kann auch gefahrlos
gelöscht werden. Das ist bei der Weitergabe von Projekten sinnvoll, da sehr viele Dateien in diesem
Verzeichnis sind.

## Anlegen des Projektes

Lege ein Verzeichnis *CovidQrDemo* an.

```
md CovidQrDemo
cd CovidQrDemo
```

Lege eine leere Datei mit dem Namen *package.json* an. Dies ist die wichtigste Datei bei
Node.js Projekten. Als ersten Eintrag definieren wir *index.js* als Startdatei:

**package.json**

```javascript
{
  "main": "index.js"
}
```

## Schritt 1: Anlegen der Ordnerstruktur

Lege nun ein Verzeichnis *public* und ein Verzeichnis *src* an. Erstelle im Verzeichnis *public*
eine Datei *index.html* und befülle sie mit einem HTML 5 Grundgerüst. In VS Code kann dies mit
Rufzeichen (!) einfach erledigt werden.

Die Datei *index.js* im Ordner *src* ist vorerst noch leer. Am Ende muss die Struktur so aussehen:

```
CovidQrDemo
     │   package.json
     │
     ├───public
     │       index.html
     │
     └───src
             index.js
```

## Schritt 2: Verwendung von npm

Node.js baut stark auf den Package Manager *npm* auf. Es können Zusatzpakete geladen werden,
sodass wir nicht alles selbst entwickeln müssen.

### Laden des ersten Paketes: base45

Wir möchten COVID QR Codes auswerten. Der Inhalt hat eine spezielle Codierung: base45. Hier werden
binäre Inhalte mit "normalen" Zeichen wie A-Z, 0-9 und einigen Satzzeichen codiert. Geben wir in
Google *npm base45* ein, erhalten wir 2 Suchergebnisse: *base45 - npm* und *base45-web*.

Bevor ein Paket geladen wird, kontrollieren wir auf der npm Seite einige Punkte:

- Wann wurde das Paket zuletzt aktualisiert? Verwende keine Pakete, deren letztes Update schon
  jahrelang zurück liegt.
- In welcher Version liegt das Paket vor? Verwende keine Pakete, die nur in der Version 0
  vorliegen.
- Klicke auf die Homepage des Projektes. Ist das Paket dort gut beschrieben?
- Und zum Schluss das Wichtigste: Dieser Code soll im Browser ausgeführt werden. Ist das Paket
  auch im Browser lauffähig? Leider gibt es kein einheitliches Kennzeichen. Meist sind
  Pakete für den Browser speziell mit *web* oder *browser* gekennzeichnet.
- Sieh dir auch den Quellcode, vor allem die Datei *package.json* an. Hat das Paket Abhängigkeiten
  zu Paketen, die den oben genannten Kriterien nicht entsprechen?

Es gibt einige Pakete, die sich im Browser nicht ausführen lassen. Node.js ist nur das Buildsystem,
schlussendlich führt der Browser dann das Bundle aus. Greift z. B. ein Paket mit dem *fs* Modul
auf das Dateisystem zu, wird dies nie im Browser funktionieren-

Wir verwenden das Paket [base45-web](https://www.npmjs.com/package/base45-web), da es auch im
Browser ausführbar ist. Ein Paket wird mit dem Befehl

```
npm install (Paketname)
```

im Ordner mit der Datei *package.json* installiert. Standardmäßig wird das Paket als Dependency
in die Datei *package.json* aufgenommen. Zusätzlich wird ein Ordner *node_modules* angelegt, wo
das Paket gespeichert wird.

Am Besten verwende die Konsole in VS Code (STRG + Ö). Mit *npm install base45-web* installieren
wir das Paket.

### Verwenden des Paketes in index.js

Nun verwenden wir das Paket und schreiben eine einfache Decode Funktion. Sie prüft mit einem
regulären Ausdruck, ob nur gültige Base45 Zeichen im String vorkommen.

Der Befehl *import* wirkt wie using in C#. Erst nach dem *import* Befehl kann das Paket unter dem
angegebenen Namen genutzt werden. Das Paket *buffer* gehört zu den Standardpaketen von Node.js
und muss daher nicht mit npm installiert werden.

```javascript
import base45 from 'base45-web'
import { Buffer } from 'buffer'

function decode(base45String) {
    base45String ??= "";  // Avoid null
    const match = /^([ $%*+\-./:0-9A-Z]+)$/.exec(base45String);
    if (!match) { throw "Invalid base45 string."; }
    return Buffer.from(base45.decode(base45String)).toString('utf-8');
}

export {
    decode
}
```

#### import oder require?

Oft findet man Code, der *require* statt *import* verwendet. Das Schlüsselwort *import* ist nur
in Modulen zulässig. Module sind js Dateien, die mit dem *export* Statement Klassen, Funktionen oder
andere Objekte exportieren. Es ist die bevorzugte Variante, da auch
spezifische Objekte aus dem Modul geladen werden können. So verwenden wir z. B. beim Import des
Modules *Buffer* nur die Klasse *Buffer*. Es steht z. B. auch *Blob* zur Verfügung, was wir
allerdings nicht brauchen. Je weniger Objekte wir importieren, desto kleiner wird das Bundle.
Außerhalb von Modulen wird *require()* zum Einbinden von Codedateien verwendet.

Weitere Informationen gibt es im MDN zum
[import](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Statements/import)
oder [export](https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Statements/export)
Statement.

## Schritt 3: Nutzen von Webpack

### Was ist Webpack?

Durch den Einsatz von Paketen ergeben sich *Abhängigkeiten* (Dependencies). Wenn wir einfach
unsere index.js Datei im Browser einbinden, werden die Funktionen aus den Zusatzpaketen nicht
gefunden. Der Browser kann auch keine Pakete einfach nachinstallieren. Wir brauchen also einen
Mechanismus, der unseren Code samt den verwendeten Code aus den Paketen zusammenbündelt. Diese
Datei nennt man *bundle*. Diese Datei kann dann mit *script src* eingebunden werden.

### Installation von Webpack

Webpack ist ein sehr mächtiges Paket. Auf [der Projektseite](https://webpack.js.org/guides/getting-started)
siehst du die vielen Konfigurationsmöglichkeiten.

Wir installieren 3 Pakete: *webpack*, *webpack-cli* und das *html-webpack-plugin*. 
Der Parameter *save-dev* gibt an, dass das
Paket nur zur Entwicklung benötigt wird. Es wird in der Datei *package.json* unter *devDependencies*
eingetragen.

```
npm install webpack webpack-cli html-webpack-plugin  --save-dev
```

Nach der Installation werden scripts in der Datei *package.json* registriert. Ein Skript kann mit
*npm run Scriptname* aufgerufen werden. Die nachfolgende *package.json* Datei legt 2 Skripts
an:

- **build** startet das CLI (command line tool) webpack und erstellt ein Bundle für den Production Einsatz.
- **serve** startet einen dev Server, um bei der Entwicklung das Projekt sofort im Browser testen
  zu können.

Ergänze nun den *scripts* Eintrag in der Datei *package.json*. Die Dependencies wurden schon von npm
hinzugefügt.

**package.json**

```javascript
{
  "main": "index.js",
  "scripts": {
    "build": "webpack --config webpack.config.js",
    "serve": "webpack serve --config webpack.config.js --mode=development"
  },
  "dependencies": {
    "base45-web": "^1.0.2"
  },
  "devDependencies": {
    "html-webpack-plugin": "^5.5.0",
    "webpack": "^5.70.0",
    "webpack-cli": "^4.9.2"
  }
}
```

### Konfiguration von Webpack

Im Skript wird auf die Datei *webpack.config.js* verwiesen. Diese muss jetzt im Hauptordner (wo
auch die Datei *package.json* ist) angelegt werden. Webpack hat eine Menge zu erledigen:

- Es muss aus unserem Code ein Bundle erstellen und als Variable bereitstellen. Wir konfigurieren
  den Namen *QrDecoder*. 
- Die Datei *public/index.html* soll gelesen und ein *script* element eingefügt werden, welches das
  Bundle lädt.
- Zusätzliche Plugins müssen ausgeführt werden, um z. B. den Code in ältere ECMAScript Versionen zu
  übersetzen (Babel).

**webpack.config.js**

```javascript
const HtmlWebpackPlugin = require('html-webpack-plugin');
const path = require('path');

module.exports = {
  mode: 'production',
  devServer: {
    static: {
      directory: path.join(__dirname, 'public'),  // Absoluten Pfad erzeugen.
    },
    compress: true,
    port: 9000,
    https: true
  },
  output: {
    filename: '[name].bundle.js',        // Kann z. B. in qrDecoder.js geändert werden.
    libraryTarget: "var",                // Das exportiere Modul als Variable global deklarieren.
    library: "QrDecoder",                // Variablenname für den HTML Export.
    clean: true                          // Ausgabeverzeichnis leeren.
  },
  plugins: [
    new HtmlWebpackPlugin({
      template: 'public/index.html',    // Wo soll script src eingefügt werden?
      inject: 'head',                   // <script src=".."> im head einfügen (nicht am Ende von <body>).
      scriptLoading: 'blocking'         // Sonst würde mit defer die Variable QrDecoder 
                                        // erst nach dem Laden des HTML Inhaltes deklariert werden.
    })],
}
```

### Starten des Servers

Starte mit folgendem Befehl in der Konsole von VS Code den dev-server. Beim ersten Start wird
das Paket *webpack-dev-server* installiert.

```
npm run serve
```

Nun kann der Browser mit der URL *https://localhost:9000/* geöffnet werden. Zertifikatsfehler
müssen ignoriert werden, da das generierte Zertifikat nicht als vertrauenswürdiges Zertifikat
im Betriebssystem installiert wurde.

Es erscheint ein leeres Fenster, da wir noch keine Ausgaben in die Datei *index.htm* geschrieben haben.
Wenn wir uns aber den Quelltext im Browser ansehen, sehen wir einen interessanten Eintrag. Webpack
hat im Header ein *script* Element eingefügt, welches das Bundle lädt.

```html

<!DOCTYPE html>
<html lang="de">
    <head>
        <!-- header elements -->
        <script src="main.bundle.js"></script>
    </head>

    <body>
    </body>
</html>
```

Sehen wir uns die Datei *main.bundle.js* an (klicke in der Quelltext Ansicht auf diese Datei),
erscheint der zusammengebündelten Code. Er bsteht aus unseren Code in der Datei *index.js* sowie
aus dem Code der eingebundenen und verwendeten Pakete.

### Aufrufen unseres Modules

Wir haben in der Datei *index.js* den *export* Befehl verwendet und in der Datei *webpack.config.js*
den library Namen als *QrDecoder* definiert. Daher können wir mit *QrDecoder.decode()* unsere
exportierte Funktion nun in HTML aufrufen:

**index.html**

```html
<!DOCTYPE html>
<html lang="de">

<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>QR Checker</title>
</head>

<body>
    <input id="encoded" type="text" value="%69 VD82EI2B.KESTC" />
    <button onclick="decode()">Decode!</button>
    <pre id="decoded"></pre>

    <script>
        function decode() {
            document.getElementById("decoded").innerText = QrDecoder.decode(document.getElementById("encoded").value);
        }
    </script>

</body>
</html>
```

## Schritt 4: Der Linter EsLint

Schreiben wir in der Datei *index.js* z. B. den Befehl *return base45.decode(undefinedVariable);*
erscheint kein Fehler. Erst in der Browserkonsole bekommen wir den Hinweis, dass die Variable
*undefinedVariable* nicht definiert wurde. Das ist natürlich sehr unangenehm.

Deswegen gibt es sogenannte *Linter*. Sie analysieren den Code und zeigen mögliche Fehler auf. In
der Konsole von VS Code installieren wir nun das Paket *eslint*. Mit *CTRL+C* beenden wir vorher den
dev-server.

Das Initialisieren der Konfiguration geschieht mit einem Assistenten. Die Antworten sind unten
angeführt:

```
npm install eslint --save-dev
npm init @eslint/config

? How would you like to use ESLint? ... 
  > To check syntax and find problems
? What type of modules does your project use? ... 
  > JavaScript modules (import/export)
? Which framework does your project use? ... 
  > None of these
? Does your project use TypeScript? » No
? Where does your code run? ...  (Press <space> to select, <a> to toggle all, <i> to invert selection)
  √ Browser
? What format do you want your config file to be in? ... 
  > JSON
```

Es wird nun eine Datei *.eslintrc.json* erzeugt. Verschiebe diese Datei in den Ordner *src*, da wir
nur Dateien innerhalb dieses Ordners prüfen wollen. Jetzt passen wir noch 2 Regeln an, indem wir
die Datei *.eslintrc.json* editieren.

- **commonjs** um *require* auch in Modulen nutzen zu können.
- **no-unused-vars** deaktivieren wir, d. h. beim Anlegen von nicht verwendeten Variablen wird kein
  Fehler angezeigt.

**.eslintrc.json**

```javascript
{
    "env": {
        "browser": true,
        "es2021": true,
        "commonjs": true
    },
    "extends": "eslint:recommended",
    "parserOptions": {
        "ecmaVersion": "latest",
        "sourceType": "module"
    },
    "rules": {
        "no-unused-vars": "off"
    }
}
```

### Extension für VS Code

Um die festgestellten Probleme auch in VS Code zu sehen, muss die Extension *ESLint* installiert
werden. Danach muss in der Datei *index.js* der Befehl *return base45.decode(undefinedVariable);* rot
unterstrichen sein.

## Schritt 5: Babel - neue ECMAScript Features übersetzen

Unser Code wird im Browser ausgeführt. Dadurch haben wir allerdings keinen Einfluss darauf, welche
Features die JavaScript Engine unterstützt. Manche verwenden noch ändere Browser, die neue
Sprachkonstrukte nicht unterstützen.

Auf der [Webseite von Babel](https://babeljs.io/repl) siehst du gleich das Funktionsprinzip.
Links ist ein Ausdruck, der die neuste ECMAScript Syntax verwendet. Rechts wird die übersetzte
ES2015 Version für den Browser angegeben.

Um Babel für unser Projekt nutzen zu können, installieren wir zuerst 3 Pakete:

- **@babel/core** Das Kernpaket von babel
- **@babel/preset-env** Vordefinierte Presets, welche Syntax in welche Version übersetzt werden muss.
- **babel-loader** für webpack, da unser Code vor dem Erstellen des Bundles von Babel verarbeitet
  werden muss.

```
npm install  @babel/core @babel/preset-env babel-loader --save-dev
```

Damit die Verarbeitung von Webpack gestartet wird, fügen wir das Modul zur Datei *webpack.config.js*
als *module* hinzu:

**webpack.config.js**

```javascript
const HtmlWebpackPlugin = require('html-webpack-plugin');
const path = require('path');

module.exports = {
  // ...
  module: {
    rules: [
      {
        test: /\.(js)$/,
        exclude: /node_modules/,
        use: ['babel-loader']
      }   
    ]
  },
 // ...
}
```

Das verwendete Set an Regeln wird in einer eigenen Datei (*babel.config.json*) definiert:

**babel.config.json**

```javascript
{
    "presets": [
        "@babel/preset-env"
    ]
}
```

Wenn wir nun mit *npm run serve* den Server wieder starten, können wir in der Quelltext
Anzeige den Inhalt von *main.bundle.js* analysieren. 

```javascript
/*!**********************!*\
  !*** ./src/index.js ***!
  \**********************/
"use strict";
eval("__webpack_require__.r(__webpack_exports__);\n/* harmony export */ __webpack_require__.d(__webpack_exports__, {\n/* harmony export */   \"decode\": () => (/* binding */ decode)\n/* harmony export */ });\n/* harmony import */ var base45_web__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! base45-web */ \"./node_modules/base45-web/lib/base45-js.js\");\n/* harmony import */ var base45_web__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(base45_web__WEBPACK_IMPORTED_MODULE_0__);\n\n\nfunction decode(base45String) {\n  var _base45String;\n\n  (_base45String = base45String) !== null && _base45String !== void 0 ? _base45String : base45String = \"\"; // Avoid null\n\n  var match = /^([ $%*+\\-./:0-9A-Z]+)$/.exec(base45String);\n\n  if (!match) {\n    throw \"Invalid base45 string.\";\n  }\n\n  return base45_web__WEBPACK_IMPORTED_MODULE_0___default().decode(base45String);\n}\n\n\n\n//# sourceURL=webpack://QrDecoder/./src/index.js?");
```

Die Anweisung *base45String ??= "";* wurde durch folgenden Block ersetzt:

```javascript
(_base45String = base45String) !== null && _base45String !== void 0 ? _base45String : base45String = "";
```

## Schritt 6: Kopieren einer CSS Datei

Möchten wir das CSS Layout z. B. in der Datei *public/main.css* anlegen, muss sie von Webpack beim
Buildvorgang mitkopiert werden. Dafür brauchen wir das Paket *copy-webpack-plugin*:

```
npm install copy-webpack-plugin --save-dev
```

Danach können wir das Paket in unserer Webpack Konfiguration einbinden und alle CSS Dateien
im Ordner *public* in das Ausgabeverzeichnis (*dist*) kopieren.

**webpack.config.js**

```javascript
// ...
const CopyPlugin = require('copy-webpack-plugin');

module.exports = {
  // ...
  plugins: [
    // ...
    new CopyPlugin({
      patterns: [
        {
          context: "public",
          from: '*.css',
          to: ''
        }
      ]
    }),
}

```