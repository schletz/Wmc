<script setup>
import axios from "axios";
</script>

<template>
    You logged out.
</template>

<script>
export default {
    async mounted() {
        this.$store.commit('authenticate', null);
        await axios.get('user/logout');
    },
};
</script>
