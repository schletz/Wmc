<script setup>
import NewsImage from './NewsImage.vue';
</script>

<template>
    <div class="newsImages">
        <NewsImage
            id="12"
            headline="News mit ID 12"
            imageUrl="https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8"
        ></NewsImage>
        <NewsImage
            id="13"
            headline="News mit ID 13"
            imageUrl="https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8"
        ></NewsImage>
        <NewsImage
            id="14"
            headline="News mit ID 14"
            imageUrl="https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8"
        ></NewsImage>
        <NewsImage
            id="15"
            headline="News mit ID 15"
            imageUrl="https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8"
        ></NewsImage>
        <NewsImage
            id="16"
            headline="News mit ID 16"
            imageUrl="https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8"
        ></NewsImage>
    </div>
</template>

<style scoped>
.newsImages {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}
</style>