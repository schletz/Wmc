<script setup>
import axios from 'axios';
</script>

<template>
    <div class="addCategoryView">
        <input type="text" v-model="model.name" />
        <button v-on:click="save()">Save</button>
        <div class="validationError">{{ validation.name }}</div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            model: {},
            validation: {}
        }
    },
    methods: {
        async save() {
            try {
                // POST https://localhost:5001/api/category
                await axios.post('category', this.model);
                this.model = {};
                this.validation = {};
                this.message = "Deine Kategorie wurde gespeichert.";
            } catch (e) {
                if (e.response?.status == 400) {
                    if (typeof e.response.data === "object")
                    {
                        this.validation = Object.keys(e.response.data.errors).reduce((prev, key) => {
                            const newKey = key.charAt(0).toLowerCase() + key.slice(1);
                            prev[newKey] = e.response.data.errors[key][0];
                            return prev;
                        }, {});
                    }
                    else {
                        alert(e.response.data);
                    }
                }
                else if (e.response?.status) {
                    alert(`HTTP Status ${e.response.status}`);
                }
                else {
                    alert("Server nicht erreichbar.");
                }
            }
        }
    }
}
</script>