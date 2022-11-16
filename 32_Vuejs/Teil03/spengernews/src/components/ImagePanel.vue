<script setup>
import NewsImage from './NewsImage.vue';
</script>

<template>
    <div class="newsImages">
        <h3>
            {{ newsCount }} von {{ newsItems.length }} News.&nbsp;
            <span @click="newsCount++">more</span>&nbsp;
            <span @click="newsCount--">less</span>
        </h3>
        <template v-if="newsItems.length">
            <NewsImage
                v-for="item in displayNews"
                v-bind:key="item.id"
                :id="item.id"
                :headline="item.headline"
                :imageUrl="item.imageUrl"
            ></NewsImage>
        </template>
    </div>
</template>

<script>
export default {
    data() {
        return {
            newsCount: 3,
            newsItems: [],
        };
    },
    computed: {
        displayNews: function () {
            return this.newsItems.slice(0, this.newsCount);
        },
    },
    mounted: async function () {
        const res = await fetch('https://localhost:5001/api/news');
        if (!res.ok) {
            alert('Problem beim Laden der Daten.');
        }
        this.newsItems = await res.json();
        this.count = 10;
    },
    methods: {
        incrementCounter: function (step) {
            this.count += step;
        },
    },
};
</script>

<style scoped>
.newsImages {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}
</style>