<script>
export default {
    props: {
        id: Number,
    },
    mounted() {
        this.headline = `News mit ID ${this.id}`;
        this.url =
            "https://assets.orf.at/mims/2022/40/23/crops/w=171,q=90,r=1/1514150_1k_550823_wirtschaft_erfolgreiche_laender_zko.jpg?s=8c6cb33bc7f34b3ae5eacb249b8e75a0831a89b8";
    },
    data() {
        return {
            headline: "",
            url: "",
        };
    },
};
</script>

<template>
    <div class="imageContainer">
        <img :src="url" />
        <div class="overlayText">{{ headline }}</div>
    </div>
</template>

<style scoped>
.imageContainer {
    position: relative;
}
.imageContainer img {
    opacity: 0.3;
}

.overlayText {
    position: absolute;
    bottom: 8px;
    left: 8px;
    font-size: 14pt;
    font-weight: bold;
    color: black;
}
.imageContainer:hover .overlayText {
    color: white;
}
.imageContainer:hover img {
    opacity: 1;
}
</style>