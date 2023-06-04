/* eslint-disable no-undef */
import { createApp } from 'vue'
import axios from "axios";             // npm install axios

import App from './App.vue'
import router from './router'
import store from './store.js'
import './assets/main.css'

axios.defaults.baseURL = process.env.NODE_ENV == 'production' ? "/api" : "https://localhost/api";
axios.defaults.withCredentials = true;

const app = createApp(App)
app.use(router)
app.use(store)
app.mount('#app')
