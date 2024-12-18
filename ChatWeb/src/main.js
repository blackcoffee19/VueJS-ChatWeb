import './assets/main.css'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'primeflex/primeflex.css'
import { createApp } from 'vue'
import stores from './stores'
import App from './App.vue'
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';
import router from './router'; 

const app = createApp(App);
app.use(PrimeVue, {
    theme: {
        preset: Aura,
        options: {
            darkModeSelector: '.app-dark'
        }
    }
  });
app.use(router);
app.use(stores);
app.mount('#app');