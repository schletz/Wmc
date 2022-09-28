<script setup>
import ImagePanel from './components/ImagePanel.vue'
import NewsFlash from './components/NewsFlash.vue'
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

