<script lang="js">
  import Menu from 'primevue/menu';
  import {  mapGetters } from 'vuex';
import { RouterLink } from 'vue-router';
import stores from '@/stores';
import router from '@/router';
  export default {
    components: {
      Menu,
      RouterLink
    },
    setup() {
      const menuitems = [
        {
          items: [
            {
              label: 'New Post',
              icon: 'pi pi-fw  pi-pen-to-square',
              route: '/'
            },
            {
              label: 'People',
              icon: 'pi pi-fw pi-users',
              route: '/search'
            },
            {
              label: 'Chat',
              icon: 'pi pi-fw pi-comment',
              route: '/'
            },
            {
              label: 'Notification',
              icon: 'pi pi-fw pi-bell',
              route: '/notification'
            },
            {
              label: 'App',
              icon: 'pi pi-fw pi-th-large',
              route: '/'
            },
            {
              label: 'Theme',
              icon: 'pi pi-fw pi-moon',
              route: '/'
            }
          ]
        }
      ];
      return { menuitems }

    },
    computed: {
      ...mapGetters(['getRequirements'])
    },
    methods: {
      logout()  {
        stores.dispatch("logout"); 
        router.push("/login");   
      }
    }

  }
</script>
<style>
.p-menu{
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: start;
    height: 100%;
    padding: 1rem 0;
    min-width: 1rem !important;
}
.navigation{
    width: 120px;
    border: 0;
}
.icon_branch{
    display: block;
    margin-bottom: 1.25rem;
}
.p-menu-item-link>span{
    font-size: 30px;
    margin-bottom: 30px;
}
.p-menu-start{
    margin-top: 20px;
}
.p-menu-list{
    flex-grow: 1;
    justify-content: center;
}

</style>
<template>
    <nav class="navigation ">
        <!-- Nav items -->
        <Menu :model="menuitems"  class="w-full md:w-100 h-full " >
            <template #start>
                <a href="index.html" title="Messenger" class="icon_branch">
                    <svg version="1.1" width="46px" height="46px" fill="currentColor" xmlns="http://www.w3.org/2000/svg"
                        xmlns:xlink="http://www.w3.org/1999/xlink" x="0px" y="0px" viewBox="0 0 46 46"
                        enable-background="new 0 0 46 46" xml:space="preserve">
                        <polygon opacity="0.7" points="45,11 36,11 35.5,1 " />
                        <polygon points="35.5,1 25.4,14.1 39,21 " />
                        <polygon opacity="0.4" points="17,9.8 39,21 17,26 " />
                        <polygon opacity="0.7" points="2,12 17,26 17,9.8 " />
                        <polygon opacity="0.7" points="17,26 39,21 28,36 " />
                        <polygon points="28,36 4.5,44 17,26 " />
                        <polygon points="17,26 1,26 10.8,20.1 " />
                    </svg>
                </a>
            </template>
            <template #item="{ item, props }">
              <router-link v-if="item.route" v-slot="{ href, navigate }" :to="item.route" custom>
                <OverlayBadge :value="getRequirements.length" severity="danger" v-if="getRequirements.length>0 && item.label == 'Notification'">
                  <a :href="href" v-bind="props.action" @click="navigate">
                    <span :class="item.icon"></span>
                  </a>
                </OverlayBadge>
                <a :href="href" v-bind="props.action" @click="navigate"  v-if="getRequirements.length==0 || item.label != 'Notification'">
                  <span :class="item.icon"></span>
                </a>
              </router-link>
                <a v-else :href="item.url" :target="item.target" v-bind="props.action">
                    <span :class="item.icon" ></span>
                </a>
            </template>
            <template #end>
                <a @click="logout"  class="p-menu-item-link">
                    <span class="pi pi-user" ></span>
                </a>
            </template>
        </Menu>
    </nav>
</template>
