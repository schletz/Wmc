import './assets/main.css'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import axios from "axios";
import store from "./store.js";

axios.defaults.baseURL =
  process.env.NODE_ENV == "production" ? "/api" : "https://localhost:5001/api";
axios.defaults.withCredentials = true;

axios
  .get("user/userinfo")
  .then((response) => {
    store.commit("authenticate", response.data);
  })
  .finally(() => {
    const app = createApp(App);
    app.use(router)
    app.use(store);
    app.mount('#app');
  })


