<script setup>
import NewsImage from './NewsImage.vue';
</script>

<template>
    <div>
        <h3>
            {{ newsCount }} von {{ newsItems.length }} News.&nbsp;
            <button @click="newsCount++">more</button>&nbsp;
            <button @click="newsCount--">less</button>
        </h3>
        <div class="newsImages">
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
        try {
            const res = await fetch('https://localhost:5001/api/news');
            if (!res.ok) {
                alert('Problem beim Laden der Daten.');
            }
            this.newsItems = await res.json();
            this.count = 10;
        } catch (e) {
            alert('Der Server ist nicht erreichbar.');
        }
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