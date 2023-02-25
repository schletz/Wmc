<script setup>
import axios from 'axios';
</script>

<template>
    <div class="loginForm">
        <template v-if="!authenticated">
            <label>Username: <input type="text" v-model="model.username" /></label>
            <label>Password: <input type="password" v-model="model.password" /></label>
            <button type="button" v-on:click="sendLoginData">Submit</button>
            <small>(Hint: Use admin, password 1111 or user, password 1111)</small>
        </template>
        <template v-else>
            Angemeldet als {{ username }}
            <a href="javascript:void(0)" v-on:click="deleteToken">Abmelden</a>
        </template>
    </div>
</template>
<style scoped>
.loginForm {
    display: flex;
    gap:1rem;
}
a {
    text-decoration: none;

}
a:hover {
    color: lightgray;
    text-decoration: underline;
}
</style>

<script>
export default {
    data() {
        return {
            model: {
                username: '',
                password: '',
            },
        };
    },
    methods: {
        deleteToken() {
            delete axios.defaults.headers.common['Authorization'];
            this.$store.commit('authenticate', null);
        },
        async sendLoginData() {
            try {
                const userdata = (await axios.post('user/login', this.model)).data;
                axios.defaults.headers.common['Authorization'] = `Bearer ${userdata.token}`;
                this.$store.commit('authenticate', userdata);
            } catch (e) {
                if (e.response.status == 401) {
                    alert('Login failed. Invalid credentials.');
                }
            }
        },
    },
    computed: {
        authenticated() {
            return this.$store.state.user.isLoggedIn;
        },
        username() {
            return this.$store.state.user.name;
        }
    },
};
</script>
