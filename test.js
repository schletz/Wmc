const mySet = new Set();
const setFromArray = new Set([1, 2, 3, 1]);
const json1 = {x: 1};
const json2 = {x: 1};

mySet.add("1");
mySet.add(1);
mySet.add(1);              // Not added
mySet.add(json1);
mySet.add(json1);          // Not added
mySet.add(json2);          // Added
console.log(mySet);        // { '1', 1 }
console.log(setFromArray)  // { 1, 2, 3 }
mySet.delete("1");

