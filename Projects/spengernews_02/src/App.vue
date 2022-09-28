<script setup>
import ImagePanel from './components/ImagePanel.vue'
import NewsFlash from './components/NewsFlash.vue'
</script>

<template>
  <div id="header">
    <h1>Spengernews</h1>
  </div>
  <div id="main">
    <ImagePanel></ImagePanel>
    <NewsFlash></NewsFlash>  
  </div>
</template>

<style>
html, body {
  margin:0;
  padding:0;
  width:100%;
  height: 100%;
  background-color:beige;
}
#app {
  width: 100%;
  height: 100%;
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
