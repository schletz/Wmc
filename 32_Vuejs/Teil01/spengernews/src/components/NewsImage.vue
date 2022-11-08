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
