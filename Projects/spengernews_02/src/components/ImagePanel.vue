<script setup>
import NewsImage from './NewsImage.vue'
</script>

<template>
    <div class="newsImages">
        <NewsImage id="12"></NewsImage>
        <NewsImage id="13"></NewsImage>
        <NewsImage id="13"></NewsImage>
        <NewsImage id="13"></NewsImage>
        <NewsImage id="13"></NewsImage>
        <NewsImage id="13"></NewsImage>
        <NewsImage id="13"></NewsImage>
    </div>
</template>

<style scoped>
.newsImages {
    display: flex;
    flex-wrap: wrap;
    gap:1rem;
}
</style>