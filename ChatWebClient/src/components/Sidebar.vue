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
      ...mapGetters(["getUserId"])
    },
   setup() {
    const store = useStore();
    const items = ref([]); 
    const loading = ref(true); 
    const error = ref(null); 
    const fetchItems = async () => {
      try {
        const response = await UserService.getListGroupChat();
        items.value = response.data;
        let data = await response.data;
        for (let i = 0; i < data.length; i++) {
          store.dispatch("addGroupChat", { groupName: data[i].code });
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
      ...mapActions(['updateGroupChat', "updateChats","updateGroupChatCode"]),
      handleChooseGroup(id, code) {
        ChatService.GetListChats(id);
        this.updateGroupChatCode(code);
      }
    }
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
                                          <span class="position-absolute top-0 start-0 translate-middle p-2 bg-danger border border-light rounded-circle" v-if="item.chatsSend.id != 0 && !item.chatsSend.isSeen && item.chatsSend.userId != getUserId">
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
                                            <div class="text-left" v-if="item.chatsSend.id != 0"  >
                                                <span class="fw-normal text-sm">{{ item.chatsSend.textMessage }}</span>
                                            </div>
                                            <div class="text-left " >
                                              <span class="fw-light text-sm">{{ item.chatsSend.timeSpan }}</span>
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
