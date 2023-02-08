<script setup>
import NewsImage from './NewsImage.vue';
import axios from 'axios';
</script>

<template>
    <div class="imagePanel">
        <div class="newsCounter">
            <div class="newsCount">{{ newsCount }} von {{ newsItems.length }} News</div>
            <div class="newsButtons">
                <button v-on:click="incrementCounter(1)">more</button>&nbsp;
                <button v-on:click="incrementCounter(-1)">less</button>
            </div>
        </div>
        <div class="newsImages">
            <template v-if="newsItems.length">
                <div v-for="item in displayNews" v-bind:key="item.id">
                    <router-link v-bind:to="`/news/${item.guid}`">
                        <NewsImage v-bind:id="item.id" v-bind:headline="item.headline" v-bind:imageUrl="item.imageUrl"></NewsImage>
                    </router-link>
                </div>
            </template>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            newsCount: 0,
            newsItems: [],
        };
    },
    computed: {
        displayNews: function () {
            return this.newsItems.slice(0, this.newsCount);
        },
    },
    async mounted() {
        try {
            this.newsItems = (await axios.get('news')).data;
            this.newsCount = Math.min(this.newsItems.length, 3);
        } catch (e) {
            alert('Der Server ist nicht erreichbar.');
        }
    },
    methods: {
        incrementCounter: function (step) {
            this.newsCount = Math.min(this.newsItems.length, Math.max(0, this.newsCount + step));
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
.newsCounter {
    display: flex;
    gap:2rem;
    margin-bottom: 0.5rem;
}
.newsCount {
    font-weight: bolder;
}
</style>