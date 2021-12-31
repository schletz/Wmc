# Module

## Motivation

Wir haben im vorigen Beispiel 2 Klassen (*User* und *Person*) in einer Datei definiert. Wenn Projekte
größer werden, muss der Code natürlich auf mehrere Dateien aufgeteilt werden. Meist wird
JavaScript Code im Browser genutzt. Würden wir unsere Klassen aufteilen, müssen wir beide
Dateien einbinden:

```html
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <script src="user.js"></script>
    <script src="person.js"></script>
    <title>JS Test</title>
</head>
<body>
    <script>
        const post = new Post(new User("Max", "Mustermann", generateSchoolMail("mustermann")));
        console.log(post);
    </script>
</body>
</html>
```

Vertauschen wir die Dateien, entsteht ein Laufzeitfehler. In der Klasse Person wird nämlich schon
der User z. B. bei der Abfrage des Typs mit *instanceof* verwendet. In der Datei *person.js* gab
es vor den Modulen keine offizielle Technik, um eine andere JavaScript Datei einzubinden. Bei
größeren Projekten wird die Einbindung aller Dateien in der richtigen Reihenfolge natürlich immer
schwieriger.

Erschwerend kommt noch hinzu dass die gleiche Variable in mehreren Dateien deklariert werden
könnte. Dadurch entstehen Laufzeitfehler.

Mit ECMAScript 6 wurden sogenannte *Module* eingeführt. Module sind JavaScript Dateien, die
- Objekte von anderen Modulen mit *import* laden können und
- Objekte für andere Module mit *export* exportieren können.

Alle Objekte, die in einem Modul deklariert werden, sind *private*, d. h. nicht von außen
sichtbar. Das vermindert auch die Probleme von mehrfach deklarierten Variablen.

Damit Node.js eine Datei als Modul verwenden kann, braucht es entweder eine Konfiguration oder
die Dateiendung *mjs*. Nun können wir die Dateien aufteilen und das Programm mit *node index.mjs*
ausführen:

**user.mjs**
```javascript
"use strict";
function generateSchoolMail(lastname) {
    return lastname.substring(0, 3) + "@spengergasse.at";
}

class User {
    constructor(firstname, lastname, email) {
        this.firstname = firstname;
        this.lastname = lastname;
        this.email = email;
    }
    
    get id() { return this.email; }
}

export { User, generateSchoolMail };
```

**post.mjs**
```javascript
"use strict";
import { User } from "./user.mjs";

export default class Post {
    constructor(user, title) {
        this.user = user;
        this.title = title;
        this.ratings = [];
        this.comments = [];
    }

    addComment(user, comment) {
        // Important: operator precedence. Write !(a instanceof class)
        if (!(user instanceof User)) { throw new TypeError('user is not an instance of User.'); }
        if (typeof comment !== "string") { throw new TypeError('comment is not a string of User.'); }

        this.comments.push({ date: new Date(), user: user, comment: comment });
    }

    tryAddRating(user, rating) {
        if (!(user instanceof User)) { throw new TypeError('user is not an instance of User.'); }
        rating = Number(rating);
        if (!isFinite(rating)) { throw new TypeError('rating is not a number.'); }

        if (this.ratings.find(r => r.user.id == user.id)) { return false; }
        this.ratings.push({ user: user, rating: rating });
        return true;
    }

    get commentsCount() {
        return this.comments.length;
    }

    get averageRating() {
        if (!this.ratings.length) { return; }
        return this.ratings.reduce((prev, current) => prev + current.rating, 0) / this.ratings.length;
    }

    getCommentsFromUser(user) {
        if (!(user instanceof User)) { throw new TypeError('user is not an instance of User.'); }

        return this.comments.filter(c => c.user.id == user.id).length;
    }
}
```


**index.mjs**
```javascript
"use strict";
import Post from "./post.mjs"
import { User, generateSchoolMail } from "./user.mjs"

const post = new Post(new User("Max", "Mustermann", generateSchoolMail("mustermann")));
const commentator1 = new User("Sophie", "Musterfrau", "sophie@mail.at");
const commentator2 = new User("Tobias", "Eifrig", "tobias@mail.at");

post.addComment(commentator1, "First comment.");
post.addComment(commentator1, "Second comment.");
post.addComment(commentator2, "Third comment.");
post.tryAddRating(commentator1, 4);
post.tryAddRating(commentator1, 3);
post.tryAddRating(commentator2, 3);
console.log(`Number of comments: ${post.commentsCount}`);
console.log(`Average rating: ${post.averageRating}`);
console.log(`${commentator1.id} has written ${post.getCommentsFromUser(commentator1)} comments.`);
```

## Das export Statement

Es werden 2 Versionen des *export* Statements verwendet:
- *export default* und
- *export { obj1, obj2, ... }*

*export default* wird verwendet, um ein einzelnes Objekt gleich bei der Deklaration zu exportieren.
Diese Anweisung darf nur 1x vorkommen. Da die Klasse *Person* das einzige Objekt ist, das wir
aus *person.mjs* exportieren wollen, verwenden wir diese Methode.

Im Modul *user.mjs* wollen wir eine Klasse und eine Funktion exportieren. Daher verwenden wir die
zweite Form, um mehrere Objekte angeben zu können.

## Das import Statement

Der große Vorteil ist, dass wir im Modul *person.mjs* die Abhängigkeit zu *user.mjs* in Form eines
*import* Statements angeben können. In diesem Fall wird mittels `import { User } from "./user.mjs";`
die Deklaration von *User* importiert.

> **Wichtig:** Achte beim Import von selbst erstellen Dateien auf die Pfadangabe mit *./*. Sonst
> würden die installierten Pakete in *node_modules* durchsucht werden.

In der Datei *index.mjs* importieren wir die Personenklasse ohne `{}`, da sie mit *export default*
exportiert wurde. Vom Usermodul können brauchen wir beide Objekte. Wir können also die Deklaration
von *User* in verschiedenen Dateien (*person.mjs* und *index.mjs*) nutzen, ohne dass es zu
Konflikten kommt.

