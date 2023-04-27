<script setup>
import axios from "axios";
</script>
<template>
    <div class="articlesView">
        <p>Data from /api/?controller=article:</p>
        <table>
            <thead>
                <tr>
                    <th>GUID</th>
                    <th>Content</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="a in articles" v-bind:key="a.Guid">
                    <td>{{ a.Guid }}</td>
                    <td>{{ a.Content }}</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>
<style scoped></style>
<script>
export default {
    data() {
        return {
            articles: {}
        }
    },
    async mounted() {
        try {
            this.articles = (await axios.get("?controller=article")).data;
        }
        catch (e) {
            alert(JSON.stringify(e));
        }
    }
}
</script>