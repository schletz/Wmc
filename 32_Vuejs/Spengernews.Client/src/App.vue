<script setup>
import NavigationBar from './components/NavigationBar.vue';
</script>

<template>
    <!-- Template needs to have one root element -->
    <div id="appContainer">
        <nav id="header">
            <NavigationBar></NavigationBar>
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
            return this.$store.state.user.isLoggedIn;
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

</style>
