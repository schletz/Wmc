<script setup>
import axios from "axios";
</script>
<template>
    <div class="loginView">
        <div class="formRow">
            <div class="label">Username:</div>
            <div class="control"><input type="text" v-model="model.username"></div>
        </div>
        <div class="formRow">
            <div class="label">Password:</div>
            <div class="control"><input type="password" v-model="model.password"></div>
        </div>        
        <div class="formRow">
            <button v-on:click="sendLogin()">Login</button>
        </div>
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
        async sendLogin() {
            try {
                const userdata = await axios.post("?controller=user&method=login", this.model);
                this.$store.commit('authenticate', userdata);
                this.success = true;
            }
            catch (e) {
                alert(JSON.stringify(e));
            }
        }
    }
}
</script>