# Functions und this

## Definition einer Funktion

Funktionen können mit den Schlüsselwort *function* definiert werden. Oberflächlich betrachtet
erscheint an der nachfolgenden Deklaration nichts ungewöhnliches:
```javascript
function add(x, y) {
    return x + y;
}

console.log(add(1, 2));    // 3
```

Beim genaueren Hinsehen erkennen wir allerdings Unterschiede zu anderen Sprachen mit static
Typing wie Java oder C#.
- Es wird kein Typ für den Rückgabewert definiert. Ob eine Funktion etwas zurückliefert oder nicht
  ist nicht erkennbar. Es gibt keine expliziten *void* Funktionen.
- x und y haben keinen definierten Datentyp.

Der nachfolgende Code wird ohne Fehler ausgeführt, liefert aber ungewöhnliche Ergebnisse:

```javascript
console.log(add(1, "2"));       // 12
console.log(add(1, 2, 3, 4));   // 3, no runtime exception
console.log(add(1));            // no runtime exception, result is NaN
console.log(add());             // no runtime exception, result is NaN
```

Wir können uns bei einer Funktion nicht darauf verlassen, dass der Aufrufer numerische Werte oder
überhaupt Werte angibt. Daher ist es oft notwendig, die Argumente zu prüfen. Mit dem *typeof*
Operator kann der Typ bestimmt werden. Fehlende Argumente haben den typ *undefined*.

Wir erkennen auch, dass die Funktion einmal nichts (im Fehlerfall) und einmal einen numerischen
Wert zurückgeben kann.

```javascript
function add(x, y) {
    if (typeof x !== "number") { return; }
    if (typeof y !== "number") { return; }
    return x + y;
}

console.log(add(1, 2));     // 3
console.log(add(1, "2"));  // undefined
console.log(add(1));       // undefined
console.log(add());        // undefined
```

Möchten wir die Argumente wenn möglich umwandeln, können die Umwandlungsfunktionen wir *Number()*
oder *Boolean()* in ECMAScript verwendet werden. Das Ergebnis im Fehlerfall ist NaN. Es ist ein
spezieller numerischer Wert. Mit *isFinite()* kann geprüft werden, ob eine "echte" Zahl zurückgegeben
wurde.

```javascript
function add(x, y) {
    x = Number(x);
    y = Number(y);
    return x + y;
}

const result = add("A", 1);
if (!isFinite(result)) {
    console.log("Invalid number.")
}
```

## Funktionen als Objekte

Etwas seltsam wirkt die nachfolgende Definition der add Funktion. Sie wird nun ohne Namen definiert,
aber in eine Variable geschrieben. In JavaScript sind Funktionen nichts anderes als Objekte, die
über eine Referenz gespeichert werden können. Bei der Definition erfolgt noch kein Aufruf. Erst
beim Aufruf von *add()* wird die Funktion durchlaufen.

```javascript
const add = function (x, y) {
    x = Number(x);
    y = Number(y);
    return x + y;
};

console.log(typeof add);     // "function"
console.log(add(1, 2));      // 3
```

### Anwendung: Callback Funktion

Wenn Funktionen nichts anderes als Objekte sind, dann können wir sie auch einfach als Argument
übergeben. Das nachfolgende Beispiel zeigt ein sehr fundamentales Konzept in JavaScript: Callback
Funktionen. 

Unsere *add()* Funktion soll im Fehlerfall Aktionen durchführen. Anfänger würden das Fehlerverhalten
direkt in die Funktion implementieren:

```javascript
function add(x, y, logger) {
    x = Number(x);
    y = Number(y);
    const result = x + y;
    if (!isFinite(result)) {
        console.log("Invalid arguments.");
    }
    return result;
};
```

Wenn nun ein anderer Aufbau des Logeintrages erforderlich ist, müssten wir die add Funktion ändern.
Dies widerspricht den sogenannten *open–closed principle*: 
*"software entities (classes, modules, functions, etc.) should be open for extension, but closed
for modification"*.

Das Logging hat nichts mit der Aufgabe der Funktion zu tun, daher übergeben wir einfach eine
Logmethode. Natürlich sollte geprüft werden, ob das Argument eine Funktion ist. Falls nicht,
weisen wir eine leere Funktion zu, damit der Aufruf des Loggers in add keinen Laufzeitfehler verursacht.

```javascript
function consoleLogger(message) {
    console.log(`${message}`);
}

function dateLogger(message) {
    console.log(`${new Date().toISOString()} ${message}`);
}

function add(x, y, logger) {
    x = Number(x);
    y = Number(y);
    logger = typeof (logger) === "function" ? logger : function () { };
    const result = x + y;
    if (!isFinite(result)) {
        logger("Invalid arguments.");
    }
    return result;
};

const result = add(1, "x", consoleLogger);  // Invalid arguments.
const result2 = add(1, "x", dateLogger);    // 2021-01-14T10:36:03.864Z Invalid arguments.
const result3 = add(1, "x", "a string");    // no logging, but no runtine exception
```

## Closures

Mit dem Wissen, dass Funktionen lediglich Objekte sind, können wir natürlich Funktionen auch als
Rückgabewert verwenden. Diese zurückgegebenen Funktionen haben eine Besonderheit: Sie können auf
Variablen, die in der übergeordneten Funktion vorhanden sind, zugreifen. Somit ist folgendes
Beispiel umsetzbar:

```javascript
function getAgeCalculator(now) {
    now = new Date(now);
    return function (date) {
        return (now - new Date(date)) / 86_400_000;
    };
};

const ageCalculator = getAgeCalculator("2021-12-15");
console.log(ageCalculator("2021-12-11"));  // 4
console.log(ageCalculator("2021-12-10"));  // 5
```

Natürlich können wir auch Funktionen als Properties eines JSON Objektes definieren. Somit können
wir mehrere Methoden zurückgeben, die auf *now* zugreifen können.

```javascript
function getAgeCalculator(now) {
    now = new Date(now);
    return {
        calculateAge(date) {
            return (now - new Date(date)) / 86_400_000;
        },
        isFullAged(date) {
            date = new Date(date);
            date.setFullYear(date.getFullYear() + 18);
            return now > date;
        }
    };
};

const ageCalculator = getAgeCalculator("2021-12-15");
console.log(ageCalculator.calculateAge("2002-12-10"));  // 6945
console.log(ageCalculator.isFullAged("2002-12-10"));    // true
console.log(ageCalculator.isFullAged("2004-12-10"));    // false
```