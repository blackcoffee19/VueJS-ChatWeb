<script lang="js">
  import { Button, DataView, InputGroup, InputText, ButtonGroup } from 'primevue';
  import { mapActions, mapState, mapGetters } from 'vuex';
  import UserService from '@/services';
  export default {
    components: {
      InputGroup,
      InputText,
      DataView,
      Button,
      ButtonGroup
    },
    computed: {
      ...mapGetters('search', ['getSearchStr', 'getSearchUsers', 'getNotifications']),
      searchStr: {
        get() {
          return this.$store.getters['search/getSearchStr'];
        },
        set(value) {
          this.$store.dispatch('search/inputSearch', value);
        },
      },
    },
    methods: {
      updateInput(event) {
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
      },
      handleAccept(userId) {
        UserService.postAddFriendActions(userId, true).then(response => {
          let res = Object.assign({}, response);
          if (res.status == 200 && res.data == 1) {
            this.$store.dispatch("minusNotification");
            for (let i = 0; i < this.getSearchUsers.length; i++) {
              if (this.getSearchUsers[i].usID == userId) {
                this.getSearchUsers[i].isSentRequest = false;
                this.getSearchUsers[i].isFriend = true;
                this.getSearchUsers[i].isReceivedRequest = false;
                break;
              }
            }
          }
        }).catch(error => console.log(error));
      },
      handleCancel(userId) {
        UserService.postAddFriendActions(userId,true, false).then(response => {
          let res = Object.assign({}, response);
          if (res.status == 200 && res.data == 2) {
            this.$store.dispatch("minusNotification");
            for (let i = 0; i < this.getSearchUsers.length; i++) {
              if (this.getSearchUsers[i].usID == userId) {
                this.getSearchUsers[i].isSentRequest = false;
                this.getSearchUsers[i].isReceivedRequest = false;
                break;
              }
            }
          }
        }).catch(error => console.log(error));
      },
      handleUnfriend(userid) {
        UserService.postUnFriend(userid).then(resp => {
          let res = Object.assign({}, resp);
          if (res.status == 200) {
            for (let i = 0; i < this.getSearchUsers.length; i++) {
              if (this.getSearchUsers[i].usID == userid) {
                this.getSearchUsers[i].isSentRequest = false;
                this.getSearchUsers[i].isFriend = false;
                this.getSearchUsers[i].isReceivedRequest = false;
                break;
              }
            }
          }
        })
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
    <div class="flex flex-column" style="overflow-y:scroll" v-if="getSearchUsers.length>0">
      <DataView :value="getSearchUsers">
        <template #list="slotProps">
          <div class="flex flex-column flex-grow-1">
            <div v-for="(item, index) in slotProps.items" :key="index">
              <div class="flex  sm:flex-row sm:items-center p-3 gap-4" :class="{ 'border-t border-surface-200 dark:border-surface-700': index !== 0 }">
                <div class="md:w-40 relative" style="width:150px; height:150px">
                  <router-link :to="{ name: 'profile', params: { username: item.username } }" >
                    <img :src="'/images/'+ item.avatar" class="block xl:block mx-auto rounded w-full" :alt="item.fullname" style="width: 100%; height: 100%;object-fit: cover " />
                   </router-link>
                </div>
                <div class="flex md:flex-row justify-content-between md:items-center flex-1 gap-6">
                  <div class="flex flex-row md:flex-col justify-content-between items-start gap-2">
                    <div>
                      <div class="text-lg font-medium mt-2">
                        <router-link :to="{ name: 'profile', params: { username: item.username } }" class="text-decoration-none text-black">
                          {{ item.fullname }}
                        </router-link>
                      </div>
                    </div>
                  </div>
                  <div class="flex flex-col md:items-end pt-5">
                    <div class="flex flex-row-reverse md:flex-row gap-2 h-25">
                      <ButtonGroup>
                        <Button icon="pi pi-heart" class="flex-auto md:flex-initial whitespace-nowrap p-3" variant="outlined" severity="help" raised></Button>
                        <Button icon="pi pi-plus" severity="success" label="Add Friend" v-if="!item.isFriend && !item.isSentRequest && !item.isReceivedRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3" @click="sendRequest(item.usID)" raised></Button>
                        <Button icon="pi pi-times" severity="danger" v-if="item.isFriend && !item.isSentRequest && !item.isReceivedRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3" variant="outlined" aria-label="Cancel" @click="handleUnfriend(item.usID)" raised/>
                        <Button icon="pi pi-times" severity="warn" v-if="!item.isFriend && item.isSentRequest && !item.isReceivedRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3" variant="outlined" label="Cancel Request" @click="sendCancelRequest(item.usID)" raised/>
                        <Button icon="pi pi-check" severity="success"  v-if="!item.isFriend && item.isReceivedRequest &&  !item.isSentRequest " class="flex-auto md:flex-initial whitespace-nowrap p-3" aria-label="Accept" label="Accept" @click="handleAccept(item.usID)" raised/>
                        <Button icon="pi pi-times" severity="danger"  v-if="!item.isFriend && item.isReceivedRequest && !item.isSentRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3"   aria-label="Cancel" label="Cancel" @click="handleCancel(item.usID)" raised />
                      </ButtonGroup>
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
