<script setup>
import ImagePanel from "./components/ImagePanel.vue";
import NewsFlash from "./components/NewsFlash.vue";
</script>

<template>
    <!-- Template needs to have one root element -->
    <div>
        <div id="header">
            <h1>Spengernews</h1>
        </div>
        <div id="main">
            <ImagePanel></ImagePanel>
            <NewsFlash></NewsFlash>
        </div>
    </div>
</template>

<style>
html,
body {
    margin: 0;
    padding: 0;
    width: 100%;
    background-color: beige;
    font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}
#app {
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
</style>
