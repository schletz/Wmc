<script setup>
import axios from 'axios';
</script>

<template>
    <div id="createArticle">
        <h3>Neuer Artikel</h3>
        <div class="form">
            <div class="form-row">
                <div class="label">Headline</div>
                <div class="control">
                    <input type="text" v-model="model.headline" />
                    <div v-if="validation.headline" class="error">{{ validation.headline }}</div>
                </div>
            </div>
            <div class="form-row">
                <div class="label">Content</div>
                <div class="control">
                    <textarea v-model="model.content"></textarea>
                    <div v-if="validation.content" class="error">{{ validation.content }}</div>
                </div>
            </div>
            <div class="form-row">
                <div class="label">Image URL</div>
                <div class="control">
                    <input type="text" v-model="model.imageUrl" />
                    <div v-if="validation.imageUrl" class="error">{{ validation.imageUrl }}</div>
                </div>
            </div>
            <div class="form-row">
                <div class="label">Category</div>
                <div class="control">
                    <select v-model="model.categoryGuid">
                        <option v-for="c in categories" v-bind:key="c.guid" v-bind:value="c.guid">{{ c.name }}</option>
                    </select>
                    <div v-if="validation.categoryGuid" class="error">{{ validation.categoryGuid }}</div>
                </div>
            </div>
            <div class="form-row">
                <div class="label"></div>
                <div class="control">
                    <button type="submit" v-on:click="sendData()">Senden</button>
                    <div v-if="message">{{ message }}</div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
export default {
    data() {
        return {
            categories: [],
            model: {},
            validation: {},
            message: ""
        };
    },
    async mounted() {
        try {
            this.categories = (await axios.get('category')).data;
        } catch (e) {
            alert('Fehler beim Laden der Daten.');
        }
    },
    methods: {
        async sendData() {
            this.validation = {};
            this.model.authorGuid = this.$store.state.user.guid;
            try {
                await axios.post('news', this.model);
                this.model = {};
                this.validation = {};
                this.message = "Dein Artikel wurde gespeichert.";
            } catch (e) {
                if (e.response.status == 400) {
                    this.validation = Object.keys(e.response.data.errors).reduce((prev, key) => {
                        const newKey = key.charAt(0).toLowerCase() + key.slice(1);
                        prev[newKey] = e.response.data.errors[key][0];
                        return prev;
                    }, {});
                    console.log(this.validation);
                }
            }
        },
    },
};
</script>

<style scoped>
h3 {
    color:white;
    margin-top:0;
    margin-bottom: 1rem;
}

input, textarea, select {
    background-color:rgba(255,255,255,70%);
    padding: 0.3rem 0rem;
}
.form {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    max-width: 40em;
}
.form-row {
    display: flex;
    flex-wrap: wrap;
}
.form-row .label {
    flex: 0 0 8em;
    color:white;

}
.form-row .control {
    flex-grow: 1;
}
.form-row .control input,
.form-row .control textarea {
    width: 100%;
}

</style>
