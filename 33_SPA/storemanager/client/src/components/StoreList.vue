<template>
<h1>Stores</h1>
{{ stores.length }} Stores geladen.
<table class="table table-sm">
    <thead>
        <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Close Date</th>
            <th>URL</th>
            <th>Manageraccount</th>
        </tr>
    </thead>
    <tbody>
        <tr v-for="s in stores" :key="s.id">
            <td>{{ s.id }}</td>
            <td>{{ s.name }}</td>
            <td>{{ s.closedate }}</td>
            <td>{{ s.managerid }}</td>
        </tr>
    </tbody>
</table>
</template>

<script>
export default {
    name: 'StoreList',
    data() {
        return {
            stores: []
        }
    },
    components: {},
    mounted() {
        fetch('http://localhost/storemanager_vue/index.php?controller=Stores&action=AllStores')
            .then((res) => res.json())
            .then((data) => (this.stores = data))
            .catch((e) => alert(e.message));
    },
};
</script>

<style>
</style>
