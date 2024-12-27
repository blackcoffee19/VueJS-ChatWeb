<script lang="js">
  import {  Button,DataView, InputGroup, InputText } from 'primevue';
  import { mapActions, mapState, mapGetters } from 'vuex';
  import UserService from '@/services';
  export default {
    components: {
      InputGroup,
      InputText,
      DataView,
      Button
    },
    computed: {
      ...mapGetters('search', ['getSearchStr', 'getSearchUsers', 'getNotifications']),
      searchStr: {
        get() {
          // Get the value from Vuex state or getter
          return this.$store.getters['search/getSearchStr'];
        },
        set(value) {
          // Update the value in Vuex state via mutation or action
          this.$store.dispatch('search/inputSearch', value);
        },
      },
    },
    methods: {
      //...mapActions('search',['inputSearch', 'updateSearchUsers','sendRequestSocket']),
      updateInput(event) {
        //this.$store.dispatch('search/inputSearch', event.target.value);  // Cập nhật giá trị vào store
        //this.$store.commit('search/updateSearchStr', event.target.value);
        this.$store.dispatch('search/inputSearch', event.target.value);
      },
      startSearch() {
        if (this.getSearchStr != "") {
          UserService.getSearchUsers().then(response => {
            let res = Object.assign({}, response);
            this.$store.dispatch('search/updateSearchUsers', res.data);
          }).catch(error => console.log(error));
        }
      },
      sendRequest(id) {
        if (typeof id == "number") {
          UserService.postAddRequest(id).then(response => {
            if (response.status == 200) {
              if (response.data !=null) {
                this.$store.dispatch('sendRequestSocket',{ userId: id, requirementId: response.data });
                for (let i = 0; i < this.getSearchUsers.length; i++) {
                  if (this.getSearchUsers[i].usID == id) {
                    this.getSearchUsers[i].isSentRequest = true;
                    break;
                  }
                }
              }
            }
          }).catch(error => console.log(error));
        }
      },
      sendCancelRequest(id) {
        if (typeof id == "number") {
          UserService.sendCancelRequest(id).then(response => {
            if (response.status == 200) {
              for (let i = 0; i < this.getSearchUsers.length; i++) {
                if (this.getSearchUsers[i].usID == id) {
                  this.getSearchUsers[i].isSentRequest = false;
                  break;
                }
              }
            }
           }).catch(error => console.log(error));
        }
      }
    }
  }
</script>

<template>
  <div class="flex flex-column w-10 mx-auto">
    <div class="w-12 mt-4 mb-3">
      <InputGroup >
        <Button label="Search" @click="startSearch" />
        <InputText placeholder="Keyword"
                   v-model="searchStr" 
                   @input="updateInput"  />
      </InputGroup>
    </div>
    <div class="flex flex-column" style="overflow-y:scroll">
      <DataView :value="getSearchUsers">
        <template #list="slotProps">
          <div class="flex flex-column flex-grow-1">
            <div v-for="(item, index) in slotProps.items" :key="index">
              <div class="flex flex-columnl sm:flex-row sm:items-center p-3 gap-4" :class="{ 'border-t border-surface-200 dark:border-surface-700': index !== 0 }">
                <div class="md:w-40 relative" style="width:150px; height:150px">
                  <img :src="'/images/'+ item.avatar" class="block xl:block mx-auto rounded w-full" :alt="item.fullname" style="width: 100%; height: 100%;object-fit: cover " />
                </div>
                <div class="flex flex-col md:flex-row justify-content-between md:items-center flex-1 gap-6">
                  <div class="flex flex-row md:flex-col justify-content-between items-start gap-2">
                    <div>
                      <div class="text-lg font-medium mt-2">{{ item.fullname }}</div>
                    </div>
                  </div>
                  <div class="flex flex-col md:items-end pt-5">
                    <div class="flex flex-row-reverse md:flex-row gap-2 h-25">
                      <Button icon="pi pi-heart" outlined></Button>
                      <Button icon="pi pi-plus" label="Add Friend" v-if="!item.isFriend && !item.isSentRequest" class="flex-auto md:flex-initial whitespace-nowrap" @click="sendRequest(item.usID)"></Button>
                      <Button icon="pi pi-times" severity="danger" v-if="item.isFriend && !item.isSentRequest" variant="outlined" aria-label="Cancel" />
                      <Button icon="pi pi-times" severity="warn" v-if="!item.isFriend && item.isSentRequest" variant="outlined" label="Cancel Request"  @click="sendCancelRequest(item.usID)" />
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
