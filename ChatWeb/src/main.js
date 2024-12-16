import './assets/main.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'primeflex/primeflex.css'
import { createApp } from 'vue'
import { createStore } from 'vuex';
import AuthStore from "./stores/modules/auth";
import App from './App.vue'
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';
import router from './router'; 

const app = createApp(App);
const store = createStore({
    modules: {
        AUTH: AuthStore
    }
})
app.use(PrimeVue, {
    theme: {
        preset: Aura,
        options: {
            darkModeSelector: '.app-dark'
        }
    }
  });
app.use(router);
app.use(store);
app.mount('#app');