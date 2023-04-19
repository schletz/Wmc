<script setup>
import LoginForm from './LoginForm.vue';
</script>
<template>
    <div class="navigationBar">
        <div class="navRow">
            <div><router-link to="/">Home</router-link></div>
            <div><router-link to="/about">About</router-link></div>
            <div v-if="authenticated"><router-link to="/write">Write an article</router-link></div>
            <div v-if="authenticated"><router-link to="/addcategory">Add a category</router-link></div>
            <div v-if="authenticated"><router-link to="/editnews">Edit all news</router-link></div>
        </div>
        <div class="navRow">
            <LoginForm></LoginForm>
        </div>
    </div>
</template>
<style scoped>
.navigationBar {
    display: flex;
    flex-direction: column;
    gap:0.5rem;
    background-color: #4371a3;
    padding: 0.5rem 0.5rem;
}
.navRow {
    display: flex;
    align-items: center;
    gap: 1em;
}
a {
    text-decoration: none;
    color:white;
}
a:hover {
    color: lightgray;
    text-decoration: underline;
}
</style>
<script>
export default {
    computed: {
        authenticated() {
            return this.$store.state.user.isLoggedIn;
        }
    },
};
</script>