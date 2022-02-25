import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router';
import App from './App.vue'
import HelloWorld from './components/HelloWorld.vue'
import StoreList from './components/StoreList.vue'

const routes = [
    { path: '/', component: HelloWorld },
    { path: '/stores', component: StoreList },
]

const router = createRouter({
    history: createWebHistory(),
    routes: routes
});

const app = createApp(App)
app.use(router)
app.mount('#app')
