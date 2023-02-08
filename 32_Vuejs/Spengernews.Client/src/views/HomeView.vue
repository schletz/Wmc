<script setup>
import ImagePanel from '../components/ImagePanel.vue';
import NewsContent from '../components/NewsContent.vue';
</script>

<template>
    <div class="homeView">
        <div class="titleRow">
            <div class="header">Spengernews</div>
            <img id="spengerlogo" src="@/assets/spengerlogo.svg" />
        </div>
        <ImagePanel class="imagePanel"></ImagePanel>
        <NewsContent v-if="$route.params.newsid" v-bind:newsid="$route.params.newsid"></NewsContent>
    </div>
</template>

<style scoped>
.titleRow {
    display: flex;
    align-items: center;
}
.titleRow .header {
    flex-grow: 1;
    font-size: 140%;
    color: white;
    font-weight: bolder;
}
.imagePanel {
    margin-top:1rem;
}
#spengerlogo {
    height: 30px;
}
</style>