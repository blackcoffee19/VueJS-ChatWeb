<script lang="js">
  import { Button, DataView, InputGroup, InputText, Tag } from 'primevue';
  import { mapActions, mapState, mapGetters } from 'vuex';
  import UserService from '@/services';
  export default {
    components: {
      DataView,
      Button,
      Tag
    },
    data() {
      return {
        searchStr: '',
      };
    },
    computed: {
      ...mapGetters(['getSearchStr', 'getSearchUsers', 'getNotifications'])
    },
    mounted() {
      UserService.getNotifications().then(response => {
        let res = Object.assign({}, response);
        this.loadNotifications(res.data)
      }).catch(error => console.log(error));
    },
    methods: {
      ...mapActions(['loadNotifications', 'updateNotifications','sendRequestSocket']),
      getType(type) {
        return type == 1 ? "Add Friend Request" : "???";
      },
      handleAccept(id, userId) {
        UserService.postAddFriendActions(id,false).then(response => {
          let res = Object.assign({}, response);
          if (res.status == 200) {
            this.$store.dispatch("minusNotification");
            this.updateNotifications(id);
            this.sendRequestSocket({ userId: userId, requirementId: id });
          }
        }).catch(error => console.log(error));
      },
      handleCancel(id, userId) {
        UserService.postAddFriendActions(id,false,false).then(response => {
          let res = Object.assign({}, response);
          if (res.status == 200 && res.data == 2) {
            this.$store.dispatch("minusNotification");
            this.updateNotifications(id);
            this.sendRequestSocket({ userId: userId, requirementId: id });
          }
        }).catch(error => console.log(error));
      }
    },
  }
</script>

<template>
  <div class="flex flex-column w-10 mx-auto h-100">
    <div class="flex flex-column h-100" style="overflow-y:scroll">
      <h3 class="my-4">Notifications</h3>
      <hr/>
      <p v-if="getNotifications.length == 0">You have 0 notification</p>
      <DataView :value="getNotifications" v-else>
        <template #list="slotProps">
          <div class="flex flex-column flex-grow-1">
            <div v-for="(item, index) in slotProps.items" :key="index">
              <div class="flex flex-columnl sm:flex-row sm:items-center p-3 gap-4" :class="{ 'border-t border-surface-200 dark:border-surface-700': index !== 0 }">
                <div class="md:w-40 relative" style="width:100px; height:100px">
                  <img :src="'/images/'+ item.image" class="block xl:block mx-auto rounded w-full" :alt="item.fullname" style="width: 100%; height: 100%;object-fit: cover " />
                </div>
                <div class="flex flex-col md:flex-row justify-content-between md:items-center flex-1 gap-6">
                  <div class="flex flex-column md:flex-col items-start gap-2">
                    <div class=" bg-black/70 rounded-border" >
                      <Tag severity="success" :value="getType(item.type)"></Tag>
                    </div>
                    <div>
                      <div class="text-lg font-medium mt-2">{{item.fullname}}</div>
                    </div>
                  </div>
                  <div class="flex flex-col md:items-end pt-5">
                    <div class="flex flex-row-reverse md:flex-row gap-2 h-50">
                      <Button icon="pi pi-check" severity="success" aria-label="Accept" label="Accept" class="flex-auto md:flex-initial whitespace-nowrap p-3" @click="handleAccept(item.rId, item.userId)" />
                      <Button icon="pi pi-times" severity="danger" aria-label="Cancel" label="Cancel" class="flex-auto md:flex-initial whitespace-nowrap p-3"  @click="handleCancel(item.rId,item.userId)" />
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
