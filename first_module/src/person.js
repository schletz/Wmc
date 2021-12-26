class Person {
    constructor(firstname, lastname) {
        this.firstname = firstname;
        this.lastname = lastname;
        
    }
    get longname() {
        return this.firstname + " " + this.lastname;
    }
}

export default Person;