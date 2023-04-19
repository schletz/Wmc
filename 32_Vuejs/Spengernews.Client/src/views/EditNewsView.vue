<script setup>
import axios from 'axios';
import SpinnerComponent from '../components/SpinnerComponent.vue';
</script>

<template>
    <div class="editNewsView">
        <SpinnerComponent v-if="loading"></SpinnerComponent>
    </div>
</template>

<script>
export default {
    data()  {
        return {
            authors: [],
            categories: [],
            news: [],
            loading: false
        }
    },
    async mounted() {
        try {
            this.loading = true;
            this.authors = (await axios.get("authors")).data;
            this.categories = (await axios.get("category")).data;
            this.news = (await axios.get("news")).data;
        }
        catch (e) {
            alert("Error loading data.");
        }
        finally {
            this.loading = false;
        }
    },
    methods: {

    }
}
</script>