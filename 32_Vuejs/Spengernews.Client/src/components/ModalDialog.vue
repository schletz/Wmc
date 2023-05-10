<template>
    <div class="modalDialog" v-if="active">
        <div class="dialogWindow">
            <div class="title">{{ title }}</div>
            <div class="content">
                {{ message }}
                <slot></slot>
            </div>
            <div class="buttons">
                <div v-for="b in buttons" v-bind:key="b">
                    <button v-on:click="handleClick(b)">{{ b }}</button>
                </div>
            </div>
        </div>
    </div>
    A modal dialog.
</template>

<style scoped>
.modalDialog {
    position: fixed;
    /* Stay in place */
    left: 0;
    top: 0;
    height: 100%;
    width: 100%;
    z-index: 100;
    /* Sit on top */
    background-color: rgba(0, 0, 0, 0.8);
    /* Black w/opacity */
    overflow-x: hidden;
    /* Disable horizontal scroll */
    display: flex;
    align-items: center;
    justify-content: center;
}

.dialogWindow {
    width: 100%;
    padding: 0.5rem 1rem;
    background-color: blueviolet;
    display: flex;
    flex-direction: column;
    gap: 1rem;

}

@media (min-width: 768px) {
    .dialogWindow {
        max-width: 50em;
    }
}
</style>

<script>
export default {
    props: {
        title: String,
        message: String,
        buttons: Array
    },
    mounted() {
    },
    data() {
        return {
            active: false
        }
    },
    methods: {
        show() {
            this.active = true;
            const self = this;
            return new Promise(resolve => self.resolveFunc = resolve);        
        },
        handleClick(button) {
            this.active = false;
            this.resolveFunc(button);
        }
    }
}
</script>

