<script>
export default {
    props: {
        id: Number,
        headline: String,
        imageUrl: String
    }
};
</script>

<template>
    <div class="imageContainer">
        <img :src="imageUrl" />
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