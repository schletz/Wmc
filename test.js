Array.prototype.groupBy = function (keys) {
    return this.reduce((prev, current) => {
        let ref = prev;
        let i = keys.length;
        while (i-- > 0) {
            const key = keys[i];
            const keyVal = current[key];
            if (!ref[keyVal]) { ref[keyVal] = {}; }
            ref = ref[keyVal];
        }
        const key = keys[0];
        const keyVal = current[key];        
        if (!ref[keyVal]) { ref[keyVal] = []; }
        ref[keyVal].push(current);
        return prev;
    }, {});
};


const results = [
    { name: "Max", points: 16, grade: 1, test: "A" },
    { name: "Tobias", points: 14, grade: 1, test: "A" },
    { name: "Sophie", points: 16, grade: 1, test: "A" },
    { name: "Laura", points: 14, grade: 1, test: "A" },
    { name: "Bernhard", points: 13, grade: 2, test: "A" },
    { name: "Nina", points: 13, grade: 2, test: "A" },
];

console.log(results.groupBy(["points", "grade", "test"]));

