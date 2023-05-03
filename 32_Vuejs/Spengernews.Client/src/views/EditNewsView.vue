<script setup>
import axios from 'axios';
import SpinnerComponent from '../components/SpinnerComponent.vue';
</script>

<template>
    <div class="editNewsView">
        <SpinnerComponent v-if="loading"></SpinnerComponent>
        <table>
            <thead>
                <tr>
                    <th>Headline</th>
                    <th>Content</th>
                    <th>Image URL</th>
                    <th>Category</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="n in model" v-bind:key="n.guid" v-on:change="n.changed = true">
                    <td><input type="text" v-model="n.headline" /></td>
                    <td><input type="text" v-model="n.content" /></td>
                    <td><input type="text" v-model="n.imageUrl" /></td>
                    <td>
                        <select v-model="n.categoryGuid">
                            <option value=""></option>
                            <option v-for="c in categories" v-bind:key="c.guid" v-bind:value="c.guid">{{ c.name }}</option>
                        </select>
                    </td>
                    <td><span v-if="n.changed" class="link" v-on:click="undo(n.guid)">Cancel</span></td>
                </tr>
            </tbody>
        </table>
        <div>
            {{ changeCount }} items changed.
        </div>
        <button v-if="changeCount > 0" type="button" v-on:click="saveChanged()">Speichern</button>
    </div>
</template>

<style scoped>
.link {
    cursor: pointer;
}
</style>

<script>
export default {
    data() {
        return {
            categories: [],
            news: [],
            model: [],
            loading: false
        }
    },
    async mounted() {
        try {
            this.loading = true;
            this.categories = (await axios.get("category")).data;
            const news = (await axios.get("news")).data;
            this.news = news;
            this.model = news.map(n => ({ ...n }));
        }
        catch (e) {
            alert("Error loading data.");
        }
        finally {
            this.loading = false;
        }
    },
    methods: {
        undo(guid) {
            // TODO: Ersetze das Arrayelement mit der guid aus dem Arrayelement aus news
            this.model = this.model.map(n =>
                n.guid != guid ? n : this.news.find(ne => ne.guid == guid)
            )
        },
        async saveChanged() {
            const changed = this.model.filter(m => m.changed).map(m => ({
                guid: m.guid, headline: m.headline,
                content: m.content, imageUrl: m.imageUrl, categoryGuid: m.categoryGuid
            }));
            if (!changed.length) { return; }
            try {
                this.loading = true;
                await axios.put('news/news_many', changed);
                // Entweder neu laden oder changed entfernen
                this.news = this.model.map(m => {
                    delete m.changed;
                    return m;
                });
            }
            catch (e) {
                alert("Failed to save data.");
            }
            finally {
                this.loading = false;
            }
        }
    },
    computed: {
        changeCount() {
            return this.model.filter(m => m.changed).length;
        }
    }
}
</script>