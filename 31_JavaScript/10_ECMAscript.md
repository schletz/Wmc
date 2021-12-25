# ECMAScript

Sucht man nach Informationen zu JavaScript entsteht oft Verwirrung, da die Begriffe JavaScript
und ECMAScript austauschbar verwendet werden.

*ECMAScript* definiert einen Sprachstandard. Es ist keine Programmiersprache, sondern lediglich
ein Dokument welches Operatoren, Statements, ... und deren Verhalten beschreibt. JavaScript
ist die *Implementierung* dieses Standards in Form einer konkreten Programmiersprache.

In den frühen Zeiten des Web war JavaScript stark browserabhängig. Einige Browser implementierten
Methoden, die andere Browser nicht kannten. Für uns ist der Standard *ECMAScript 6 (2015)* von
besonderem Interesse. Hier wurden erstmals Konzepte zur Entwicklung größerer Applikationen wie
Klassen und Module vorgestellt. Es ist der Standard, den wir für die heutige Webentwicklung
verwenden.

Natürlich gab es in der Zwischenzeit weitere Fortschritte, wie die folgende Aufstellung zeigt:

| Version       | Jahr | Dokumentation                                                                      | Neuerungen                                                 |
| ------------- | ---- | ---------------------------------------------------------------------------------- | ---------------------------------------------------------- |
| ECMAScript 6  | 2015 | https://262.ecma-international.org/6.0 [PDF](ECMA-262_6th_edition_june_2015.pdf)   |                                                            |
| ECMAScript 7  | 2016 | https://262.ecma-international.org/7.0                                             |                                                            |
| ECMAScript 8  | 2017 | https://262.ecma-international.org/8.0                                             | await/async                                                |
| ECMAScript 9  | 2018 | https://262.ecma-international.org/9.0                                             | spread operator ...                                        |
| ECMAScript 10 | 2019 | https://262.ecma-international.org/10.0                                            |                                                            |
| ECMAScript 11 | 2020 | https://262.ecma-international.org/11.0                                            | nullish coalescing operator (`??`), optional chains (`?.`) |
| ECMAScript 12 | 2021 | https://262.ecma-international.org/12.0 [PDF](ECMA-262_12th_edition_june_2021.pdf) | logical assignment operators (`??=`, `&&=`, `\|\|=`)       |

Zudem definiert ECMAScript noch ein *Global Object*. Es beinhaltet Funktionen und Objekte, die immer
zur Verfügung stehen (ECMAScript 6):

- **Funktionen:** eval, isFinite, isNaN, parseFloat, parseInt
- **Konstruktoren:** Array, ArrayBuffer, Boolean, DataView, Date, Error, EvalError, Float32Array,
Float64Array, Function, Int8Array, Int16Array, Int32Array, Map, Number, Object, Proxy, Promise,
RangeError, ReferenceError, RegExp, Set, String, Symbol, SyntaxError, TypeError, Uint8Array,
Uint8ClampedArray, Uint16Array, Uint32Array, URIError, WeakMap, WeakSet, 
- **Statische Objekte:** JSON, Math, Reflect

## Polyfill

Wenn wir eine neue Applikation entwickeln, wollen wir natürlich auch neue Features verwenden.
So können wir z. B. folgenden Code schreiben, um val den Wert 0 zuzuordnen, wenn val *undefined*
oder *null* ist:
```javascript
let val;
val ??= 0;
console.log(val);
```

Der Operator `??=` wurde jedoch erst mit ECMAScript 12 (2021) eingeführt. Um eine Kompatibilität
zu älteren Browsern herzustellen gibt es Bibliotheken, die den Code automatisch umschreiben. Auf
https://babeljs.io/repl kann dieser Code eingefügt werden. Die Ausgabe liefert:

```javascript
"use strict";

var _val;

let val;
(_val = val) !== null && _val !== void 0 ? _val : val = 0;
console.log(val);
```

Der `??=` Operator wurde also durch Code, den ältere Browser verstehen, ergänzt. Diesen Vorgang
bezeichnen wir als *polyfill* ("Spachtelmasse"). In der Buildchain von Node.js Applikationen kann
dieser Schritt automatisch durchgeführt werden.

## JSON als Austauschformat

Das Dokument [The JSON Data Interchange Syntax](ECMA-404_2nd_edition_december_2017.pdf)
beschreibt den Aufbau von JSON Dokumenten zum Datenaustausch z. B. zwischen einer API und dem
Client.

