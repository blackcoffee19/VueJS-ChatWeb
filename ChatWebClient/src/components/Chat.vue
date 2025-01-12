<script>
  import { mapGetters, useStore, mapActions } from 'vuex';
  import UserService from '@/services';
  import ChatService from '@/services/chat';
  import chat from '../services/chat';
  import ChatHeader from './ChatHeader.vue';
  export default {
    components: {
      ChatHeader
    },
    setup() {
      
    },
    computed: {
      ...mapGetters(['getGroupChatSelected', 'getGroupChatSelectedCode', 'getChats', 'isLoading', 'getUserId', 'getMessage', 'getMessagesSocket']),
      message: {
        get() {
          return this.getMessage; // Lấy giá trị từ getter
        },
        set(value) {
          this.updateMessage(value); // Gọi action để cập nhật store
        },
      },
    },
    methods: {
      ...mapActions(['updateMessage','getTimeSpanLastestChat']),
      sendMessage() {
        if (this.getMessage == null || this.getMessage == "") return;
        this.$store.dispatch("sendMessage", {
          message: this.getMessage,
          user: this.getUserId,
          group: this.getGroupChatSelected,
          groupName: this.getGroupChatSelectedCode,
        });
        this.$store.dispatch("updateMessage", "");
      },
      addNewMessage() {
        ChatService.SendMessage().then(x => {
          if (x == 1) {
            ChatService.GetListChats(this.getGroupChatSelected);
          }
        });   
      },
      getTimeSpanChat(chatp) {
        let chat = Object.assign({}, chatp);
        if (chat != undefined && chat.createdAt != undefined) {
        let date = new Date(new Date(chat.createdDate).getTime());
          // Tính chênh lệch thời gian (milliseconds)
          const differenceInMilliseconds = new Date() - date;
          // Chuyển đổi milliseconds sang các đơn vị thời gian
          const days = Math.floor(differenceInMilliseconds / (1000 * 60 * 60 * 24));
          const hours = Math.floor((differenceInMilliseconds % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
          const minutes = Math.floor((differenceInMilliseconds % (1000 * 60 * 60)) / (1000 * 60));
          const seconds = Math.floor((differenceInMilliseconds % (1000 * 60)) / 1000);
          //console.log(`days: ${days}\nhours: ${hours}\nminutes: ${minutes}\nseconds: ${seconds}`);
          return days > 0 ? days + ' days' : (hours > 0 ? hours + ' hours' : (minutes > 0 ? minutes + ' minutes' : seconds + ' seconds')) + ' ago';
        } else {
          return chat.timeSpan;
        }
      }
  },
  
};
</script>
<template>
    <main class="main">
      <div class="container position-relative h-100">
        <ChatHeader  v-if="getGroupChatSelected !=0 && !isLoading" />
        <div class="d-flex flex-column h-100 justify-content-center text-center" v-if="getGroupChatSelected ==0 && isLoading" >
          <div class="mb-6">
            <span class="icon icon-xl text-muted">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-message-square"><path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path></svg>
            </span>
          </div>
          <p class="text-muted">Pick a person from left menu , <br> and start your conversation.</p>
        </div>
        <div class="d-flex flex-column h-100 justify-content-center text-center align-items-center" v-if="isLoading">
          <div class="spinner-border text-black" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
        </div>
        <div class="d-flex flex-column h-100 align-items-between"  v-if="!isLoading && getChats">
          <div class="d-flex flex-column flex-grow-1" style="height: 500px; overflow-y:auto; overflow-x:hidden; padding-top:60px">
            <div v-if="getChats.length==0">


            </div>
            <DataView :value="getChats" v-else>
              <template #list="slotProps">
                  <div class="p-col-12 p-md-2" v-for="(item, index) in slotProps.items" :key="index">
                    <div class="d-flex flex-row" :class="{ 'flex-row-reverse' : getUserId==item.userId }">
                      <div class="mx-2">
                        <Avatar :image="'/images/'+ item.userImg" class="mr-2" style="background-color: #dee9fc; color: #1a2551" shape="circle"  size="large" />
                      </div>
                      <div class="card px-3 py-1 d-flex flex-column"  >
                        <div class="fw-normal fs-5">{{  item.textMessage }}</div>
                        <div>
                          <span class="fw-light">{{getTimeSpanChat(item)}}</span>
                        </div>
                      </div>
                    </div>
                  </div>
              </template>
            </DataView>
          </div>
          <div class="mb-3">
            <InputGroup style="height:50px">
              <InputText placeholder="Add Text" v-model="message" />
              <InputGroupAddon>
                <Button severity="secondary" variant="text" @click="sendMessage" >
                  <i class="pi pi-send" style="color: slateblue; font-size: 2rem"></i>
                </Button>
              </InputGroupAddon>
            </InputGroup>

          </div>
        </div>
      </div>
    </main>
</template>
