# Funktionen 2: Prototype, this und new

## this

> When a function is called as a method of an object, the object is passed to the function as its
> this value.
> <sup>https://262.ecma-international.org/6.0/#sec-method</sup>

```javascript
const person = {
    firstname: "Max",
    lastname: "Mustermann",
    getLongname() {
        return this.firstname + " " + this.lastname;
    }
}

console.log(person.getLongname());  // Max Mustermann
```

## Object.create() und der Prototype

```javascript
const person = {
    firstname: "Max",
    lastname: "Mustermann"
};
const student = Object.create(person);
student.firstname = "Sophie";
student.class = "4EHIF";

console.log(person.firstname, person.lastname);                  // Max Mustermann
console.log(student.firstname, student.lastname, student.class); // Sophie Mustermann 4EHIF
```

## Der Prototype bei Funktionen

```javascript
function point(x, y) {
    return {
        x: x,
        y: y,
        distanceToOrigin() {
            return Math.sqrt(x * x + y * y);
        },
        distanceTo(point) {
            return Math.sqrt((point.x - x) * (point.x - x) + (point.y - y) * (point.y - y));
        }
    }
}

const p1 = point(3, 4);
const p2 = point(5, 12);

console.log(p1.distanceToOrigin());   // 5
console.log(p2.distanceToOrigin());   // 13
console.log(p1.distanceTo(p2));       // 8.246
```

```javascript
const pointMethods = {
    distanceToOrigin() {
        return Math.sqrt(this.x * this.x + this.y * this.y);
    },
    distanceTo(point) {
        return Math.sqrt((point.x - this.x) * (point.x - this.x) + (point.y - this.y) * (point.y - this.y));
    }
};

function point(x, y) {
    let pointData = Object.create(pointMethods)
    pointData.x = x;
    pointData.y = y;
    return pointData;
}

const p1 = point(3, 4);
const p2 = point(5, 12);

console.log(p1.distanceToOrigin());   // 5
console.log(p2.distanceToOrigin());   // 13
console.log(p1.distanceTo(p2));       // 8.246
```

> Even though ECMAScript includes syntax for class definitions, ECMAScript objects are not fundamentally class-
> based such as those in C++, Smalltalk, or Java. Instead objects may be created in various ways including via a
> literal notation or via constructors which create objects and then execute code that initializes all or part of them
> by assigning initial values to their properties. Each constructor is a function that has a property named
> "prototype" that is used to implement prototype-based inheritance and shared properties. Objects are
> created by using constructors in new expressions; for example, new Date(2009,11) creates a new Date
> object. Invoking a constructor without using new has consequences that depend on the constructor. For
> example, Date() produces a string representation of the current date and time rather than an object.
> <sup>https://262.ecma-international.org/6.0/#sec-objects</sup>

```javascript
function Point(x, y) {
    this.x = x;
    this.y = y;
}

Point.prototype.distanceToOrigin = function () {
    return Math.sqrt(this.x * this.x + this.y * this.y);
};

Point.prototype.distanceTo = function (point) {
    return Math.sqrt((point.x - this.x) * (point.x - this.x) + (point.y - this.y) * (point.y - this.y));
};

const p1 = new Point(3, 4);
const p2 = new Point(5, 12);
console.log(p1.distanceToOrigin());   // 5
console.log(p2.distanceToOrigin());   // 13
console.log(p2.distanceTo(p1));       // 8.246
```

