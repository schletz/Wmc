<script setup>
import axios from "axios";
</script>
<template>
    <div class="logoutView">
        <p>Click the button to logout:</p>
        <button v-if="authenticated" v-on:click="sendLogout()">Logout</button>
        <p v-if="!authenticated">Not logged in.</p>
    </div>
    <p v-if="success">Login erfolgreich als User {{ model.username }}.</p>
</template>
<style scoped></style>
<script>
export default {
    data() {
        return {
            profile: {},
            model: {},
            success: false
        }
    },
    methods: {
        async sendLogout() {
            try {
                await axios.get("?controller=user&method=logout");
                this.$store.commit('authenticate', null);
            }
            catch (e) {
                alert(JSON.stringify(e));
            }
        }
    },
    computed: {
        authenticated() {
            return this.$store.state.user ? true : false;
        },
    }
}
</script>