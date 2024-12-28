<script lang="js">
import { ref, onMounted } from "vue";
import UserService from '@/services';
import ChatService from '@/services/chat';
import router from "@/router";
import { mapActions, mapState, mapGetters, useStore } from 'vuex';
export default {
    name: 'ItemList',
    computed: {
      ...mapState(['groupChatSelected']),
      ...mapGetters(["getUserId", "getLastestChats",'getListGroup']),
    },
   setup() {
    const store = useStore();
    const items = ref([]); 
    const loading = ref(true); 
    const error = ref(null);
    const fetchItems = async () => {
      try {
       
        const response = await UserService.getListGroupChat();
        let data = await response.data;
        
        items.value = response.data;
        for (let i = 0; i < data.length; i++) {
          let members = data[i].memeberGroup;
          let listMem = [];
          members.forEach(e => { listMem.push(e.userId); });
          store.dispatch("addGroupChat", { groupName: data[i].code, users: listMem, name: data[i].name});
          if (data[i].chatsSend != null) {
            let chat = { groupId: data[i].grId, text: data[i].chatsSend.textMessage, createdAt: data[i].chatsSend.createdDate, userId: data[i].chatsSend.userId };
            store.dispatch("loadLastesChats", chat);
          }
        }
      } catch (err) {
        error.value = err.message;
        router.push('/login').then(e => window.location.reload());
      } finally {
        loading.value = false;
      }
    };
    onMounted(fetchItems);

    return { items, loading, error };
    },
  methods: {
    ...mapActions(['updateGroupChat', "updateChats", "updateGroupChatCode", "loadLastesChats"]),
    handleChooseGroup(id, code) {
      ChatService.GetListChats(id);
      this.updateGroupChatCode(code);
    },
    getLastestChatText(groupId) {
      let chat = Object.assign({}, this.getLastestChats.find((x) => x.groupId === groupId));
      return chat?.text || 'No text available';
    },
    getTimeSpanLastestChat(grID) {
      let chat = Object.assign({}, this.getLastestChats.find((x) => x.groupId === grID));
      if (chat != undefined && chat.createdAt != undefined) {
        let date = new Date(new Date(chat.createdAt).getTime());
        // Tính chênh lệch thời gian (milliseconds)
        const differenceInMilliseconds = new Date() - date;
        // Chuyển đổi milliseconds sang các đơn vị thời gian
        const days = Math.floor(differenceInMilliseconds / (1000 * 60 * 60 * 24));
        const hours = Math.floor((differenceInMilliseconds % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((differenceInMilliseconds % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((differenceInMilliseconds % (1000 * 60)) / 1000);
        return days > 0 ? days + ' days' : (hours > 0 ? hours + ' hours' : (minutes > 0 ? minutes + ' minutes' : seconds + ' seconds')) + ' ago';

      }
    },
    getNotification(grouId) {
      let chat = Object.assign({}, this.getLastestChats.find((x) => x.groupId === grouId));
      return chat.id != 0 && !chat.isSeen && chat.userId != this.getUserId;
    },
    checkCalling(groupCode) {
      const group = this.getListGroup.find((g) => g.groupCode === groupCode);
      return group ? group.isCalling : false;
    }
   },
  watch: {
    getListGroup: {
      deep: true,
      handler(newListGroup) {
        newListGroup.forEach(group => {
          if (group.isCalling) {
            console.log(`Group ${group.groupCode} is calling`);
          }
        });
      },
    },
  },
};

</script>
<style lang="css" module>
.myinput {
    border-radius: 2rem;
    padding: 1rem 2rem;
    border-width: 2px;
}
</style>
<template>
    <aside class=" overflow-auto lg:w-5 xl:w-4 md:w-2">
        <div class="tablist" role="tablist">
            <!-- Chats -->
            <div class="tab-pane fade show active h-screen bg-blue-200" id="tab-content-chats" role="tabpanel">
                <div class="flex flex-column">
                    <div class="chats hide-scrollbar">
                        <div class="container ">
                            <!-- Title -->
                            <div class="flex flex-column mb-3 ">
                              <h2>Chats </h2>
                                <div v-if="loading"><div class="spinner-border text-info" role="status">
                                    <span class="visually-hidden">Loading...</span>
                                  </div></div>
                                <div v-if="error">{{ error }}</div>
                            </div>
                            <!-- Chats -->
                            
                            <DataView :value="items" layout="list">
                                <template #list="slotProps">
                                    <div class="p-col-12"  v-for="(item, index) in slotProps.items" :key="index">
                                      <div class="mb-2 card position-relative" @click="handleChooseGroup(item.grId, item.code)">
                                        <div class="flex flex-col sm:items-center p-1 gap-2 overflow-hidden align-items-center" :class="{ 'border-t border-surface-500 dark:border-surface-700': index !== 0 } "  style="height: 110px;">
                                          <span class="position-absolute top-0 start-0 translate-middle p-2 bg-danger border border-light rounded-circle" v-if=" getNotification(item.grId)">
                                            <span class="visually-hidden">New alerts</span>
                                          </span>
                                          <div class="w-3relative">
                                            <img :src="'/images/'+item.image" class="rounded-circle flex items-center justify-center mr-2 md:w-4rem md:h-4rem" width="80" height="80" :alt="item.name" />
                                          </div>
                                          <div class="flex flex-column justify-around items-start w-9 md:hidden lg:block">
                                            <div class="flex-grow-1 line-height-1 ">
                                              <span class="font-medium text-surface-500 dark:text-surface-400 text-sm">{{ item.subName }}</span>
                                              <div class="text-lg font-medium mt-2">{{ item.name }}</div>
                                            </div>
                                            <div class="text-left" v-if="item.chatsSend!=null && item.chatsSend.id != 0">
                                              <span class="fw-normal text-sm" v-if="!checkCalling(item.code)">{{ getLastestChatText(item.grId) }}</span>
                                              <span class="fw-normal text-sm text-danger fw-bold" v-else >Calling...</span>
                                            </div>
                                            <div class="text-left" v-if="item.chatsSend==null">
                                              <span class="fw-normal text-sm text-primary">Start the conversation</span>
                                            </div>
                                            <div class="text-left " v-if="item.chatsSend!=null && item.chatsSend.id != 0">
                                              <span class="fw-light text-sm">{{ getTimeSpanLastestChat(item.grId)}}</span>
                                            </div>
                                          </div>
                                        </div>
                                      </div>
                                    </div>
                                </template>
                            </DataView>
                        <!-- Chats -->
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </aside>
</template>
