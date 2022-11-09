<template>
    <!-- Template needs to have one root element -->
    <div id="appContainer">
        <div id="header">
            <nav>
                <h1>Spengernews</h1>
                <div><router-link to="/">&#128279 Home</router-link></div>
                <div><router-link to="/about">&#128279 About</router-link></div>
            </nav>
        </div>
        <main>
            <router-view></router-view>
        </main>
    </div>
</template>

<style>
html,
body {
    margin: 0;
    padding: 0;
    width: 100%;
    background-color: beige;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
#appContainer {
    margin-left: 1em;
    margin-right: 1em;
    margin-top: 1em;
    width: 100%;
    display: flex;
    flex-direction: column;
}
#header {
    flex: 0 0 4em;
}
#main {
    flex-grow: 1;
}
nav {
    display: flex;
    align-items: center;
    gap: 1em;
}
nav h1 {
    margin: 0;
}
nav a {
    text-decoration: none;    
}

</style>
