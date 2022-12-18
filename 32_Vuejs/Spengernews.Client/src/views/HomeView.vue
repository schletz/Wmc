<script setup>
import ImagePanel from "../components/ImagePanel.vue";
import NewsContent from "../components/NewsContent.vue";
</script>

<template>
    <div>
        <ImagePanel></ImagePanel>
        <NewsContent v-bind:newsid="$route.params.newsid"></NewsContent>
    </div>
</template>
