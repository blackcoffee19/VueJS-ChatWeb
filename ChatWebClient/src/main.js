import './assets/main.css'
//import '@/assets/layout/layout.scss'
import '@/assets/demo/demo.scss'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'primeflex/primeflex.css'
import 'primeicons/primeicons.css';

import { createApp } from 'vue'
import stores from './stores'
import App from './App.vue'
import PrimeVue from 'primevue/config';
import Aura from '@primevue/themes/aura';
import router from './router';
import { Avatar, Button, Card, DataView, InputGroup, InputGroupAddon, InputText, OverlayBadge } from 'primevue';

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
app.component('DataView', DataView);
app.component('Card', Card);
app.component('Avatar', Avatar);
app.component('InputGroup', InputGroup);
app.component('InputGroupAddon', InputGroupAddon);
app.component('Button', Button);
app.component('InputText', InputText);
app.component('OverlayBadge', OverlayBadge);
app.mount('#app');
