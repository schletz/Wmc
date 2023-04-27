<script setup>
import axios from "axios";
</script>
<template>
    <div class="profileView">
        <p>Data from /api/?controller=user&method=getProfile:</p>
        <ul>
            <li>Username: {{ profile.user }}</li>
            <li>Firstname: {{ profile.firstname }}</li>
            <li>Lastname: {{ profile.lastname }}</li>
            <li>E-Mail: {{ profile.email }}</li>
        </ul>
    </div>
</template>
<style scoped></style>
<script>
export default {
    data() {
        return {
            profile: {}
        }
    },
    async mounted() {
        try {
            this.profile = (await axios.get("?controller=user&method=getProfile")).data;
        }
        catch (e) {
            alert(JSON.stringify(e));
        }
    }
}
</script>