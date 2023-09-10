<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <header>
    <nav>
      <RouterLink to="/">Home</RouterLink>
      <RouterLink v-if="authenticated" to="/customers">Customers</RouterLink>
      <RouterLink v-if="!authenticated" to="/login">Login</RouterLink>
      <RouterLink v-if="authenticated" to="/logout">Logout</RouterLink>
    </nav>
  </header>

  <RouterView />
</template>

<script>
export default {
    computed: {
        authenticated() {
            return this.$store.state.userdata ? true : false;
        }
    },
};
</script>

<style scoped>
  nav {
    display: flex;
    column-gap: 1em;
  }
</style>