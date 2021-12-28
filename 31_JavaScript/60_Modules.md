# Module

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

