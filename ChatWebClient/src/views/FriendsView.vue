<script lang="js">
  import { Button, DataView, InputGroup, InputText, ProgressSpinner, Divider } from 'primevue';
  import { mapActions, mapState, mapGetters } from 'vuex';
  import router from "@/router";
  import UserService from '@/services';
  import HeartProgress from '@/components/HeartProgress.vue';
  export default {
    components: {
      InputGroup,
      InputText,
      DataView,
      Button,
      ProgressSpinner,
      Divider,
      HeartProgress
    },
    data() {
      return {
        ListFriend: [],
        ErrorMessage: "",
        isLoading: true
      }
    },
    computed: {
      
    },
    mounted() {
      this.fetchFriends();
    },
    methods: {
      async fetchFriends() {
        this.isLoading = true;
        await UserService.getListFriend().then(response => {
          if (response.status == 200) {
            let data = response.data;
            this.ListFriend = data;
          } else {
            router.push('/login').then(e => window.location.reload());
          }

        }).catch(err => console.log(err)).finally(() => this.isLoading = false);

      },
      handleUnfriend(userid) {
        UserService.postUnFriend(userid).then(resp => {
          let res = Object.assign({}, resp);
          if (res.status == 200) {
            this.fetchFriends()
          }
        })
      }
    },
  }
</script>

<template>
  <div class="flex flex-column w-10 " style="height: 100%">
    <div class="w-100 mt-4 text-start">
      <h3>Friends ({{this.ListFriend.length}})</h3>
      <Divider />
    </div>
    <div class="flex flex-column flex-grow-1 w-100" :class="isLoading?'justify-content-center text-center align-items-center':''" v-if="isLoading">
      <ProgressSpinner style="width: 50px; height: 50px" strokeWidth="8" fill="transparent"
                       animationDuration=".5s" aria-label="Custom ProgressSpinner" />

    </div>
    <div class="flex flex-column flex-grow-1  w-lg-75 w-md-100 mt-2" v-if="!isLoading" style="overflow-y: auto; max-height: 620px;">

      <DataView :value="ListFriend">
        <template #list="slotProps">
          <div class="flex flex-column flex-grow-1" >
            <div v-for="(item, index) in slotProps.items" :key="index">
              <div class="flex  sm:flex-row sm:items-center p-3 gap-4" :class="{ 'border-t border-surface-200 dark:border-surface-700': index !== 0 }">
                <div class="md:w-40 relative" style="width:150px; height:150px">
                  <router-link :to="{ name: 'profile', params: { username: item.user.username } }" >
                    <img :src="'/images/'+ item.user.avatar" class="block xl:block mx-auto rounded w-full" :alt="item.user.fullname" style="width: 100%; height: 100%;object-fit: cover " />
                  </router-link>
                </div>
                <div class="flex md:flex-row justify-content-between md:items-center flex-1 gap-6">
                  <div class="flex flex-row md:flex-col justify-content-between items-start gap-2">
                    <div class="flex flex-column align-items-start">
                      <div class="text-lg font-medium mt-2">
                        <router-link :to="{ name: 'profile', params: { username: item.user.username } }" class="text-decoration-none text-black">
                          {{ item.user.fullname }}
                          </router-link>
                      </div>

                      <HeartProgress :value="item.counting" />
                    </div>
                  </div>
                  <div class="flex flex-col md:items-end pt-5">
                    <div class="flex flex-row-reverse md:flex-row gap-2 h-25">
                      <Button icon="pi pi-times" severity="danger" class="flex-auto md:flex-initial whitespace-nowrap p-3" variant="outlined" aria-label="Cancel" label="Unfriend" @click="handleUnfriend(item.user.usID)" />
                      <Button icon="pi pi-user" severity="help" class="flex-auto md:flex-initial whitespace-nowrap p-3" label="View" />

                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </template>
      </DataView>
    </div>
  </div>
</template>
