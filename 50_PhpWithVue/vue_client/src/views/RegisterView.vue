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
            <div class="label">Firstname:</div>
            <div class="control"><input type="text" v-model="model.firstname"></div>
        </div>
        <div class="formRow">
            <div class="label">Lastname:</div>
            <div class="control"><input type="text" v-model="model.lastname"></div>
        </div>
        <div class="formRow">
            <div class="label">Email:</div>
            <div class="control"><input type="text" v-model="model.email"></div>
        </div>                        
        <div class="formRow">
            <div class="label">Password:</div>
            <div class="control"><input type="password" v-model="model.password"></div>
        </div>        
        <div class="formRow">
            <button v-on:click="registerUser()">Register</button>
        </div>
    </div>
</template>
<style scoped></style>
<script>
export default {
    data() {
        return {
            model: {},
        }
    },
    methods: {
        async registerUser() {
            try {
                await axios.post("?controller=user&method=addUser", this.model);
                this.model = {};
            }
            catch (e) {
                alert(JSON.stringify(e));
            }
        }
    }
}
</script>