<script>
export default {
    props: {
        id: Number
    },
    mounted() {
        this.headline = `News mit ID ${this.id}`;
        this.url = "https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8"
    },
    data () {
        return {
            headline: "",
            url: ""
        }
    }
}
</script>

<template>
    <img :src="url" />
    Überschrift ist {{ headline }}.
</template>