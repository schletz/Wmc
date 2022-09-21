<script>
export default {
    props: {
        id: Number
    },
    mounted() {
        this.headline = `News mit ID ${this.id}`;
        this.url = "https://www.spengergasse.at/wp-content/uploads/2020/04/SpengerLogo_720px_weiss_neu-e1586334374714.png"
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