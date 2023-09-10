<script setup>
import axios from "axios";
</script>

<template>
    <div class="loginView">
        <div class="formrow">
            <div class="label">Username</div>
            <div class="control"><input type="text" v-model="model.username"></div>
        </div>
        <div class="formrow">
            <div class="label">Password</div>
            <div class="control"><input type="password" v-model="model.password"></div>
        </div>
        <div class="formrow">
            <button type="button" v-on:click="login()">Login</button>
        </div>
    </div>
</template>

<script>
export default {
    props: {
    },
    data() {
        return {
            model: {
                username: "admin",
                password: "1234"
            }
        };
    },
    methods: {
        async login() {
            try {
                const userdata = (await axios.post('user/login', this.model)).data;
                console.log(userdata);
                this.$store.commit('authenticate', userdata);
            }
            catch (e) {
                if (e.response.status == 401)
                    alert('Login failed. Invalid credentials.');
                else
                    throw (e);
            }
        }
    },
    async mounted() {
    },

};
</script>
