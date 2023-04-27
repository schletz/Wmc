<script setup>

</script>

<template>
    <!-- Template needs to have one root element -->
    <div id="appContainer">
        <nav id="header">
            <button><router-link to="/">Home</router-link></button>
            <button><router-link to="/register">Register</router-link></button>
            <button><router-link to="/login">Login</router-link></button>
            <button v-if="authenticated"><router-link to="/logout">Logout</router-link></button>
            <button v-if="authenticated"><router-link to="/profile">Your Profile</router-link></button>
            <button><router-link to="/articles">List articles</router-link></button>
        </nav>
        <main>
            <router-view></router-view>
        </main>
    </div>
</template>

<script>
export default {
    computed: {
        authenticated() {
            return this.$store.state.user ? true : false;
        },
    },
};
</script>

<style scoped>
#appContainer {
    width: 875px;
    margin: auto;
    display: flex;
    flex-direction: column;
    gap:0.5rem;
}
main {
    padding: 0rem 0.5rem;
}

#header {
    display: flex;
    gap: 1em;
}


#header a {
    color: inherit;
    text-decoration: inherit;
}
</style>
