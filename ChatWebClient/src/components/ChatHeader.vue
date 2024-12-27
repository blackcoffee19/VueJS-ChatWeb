<script lang="js">
  import { mapGetters, useStore, mapActions } from 'vuex';
  import { Button} from 'primevue';
  import UserService from '@/services';
 export default {
    //props: {
    //  groupChat: {
    //    type: String, // Kiểu dữ liệu
    //    required: true, // Bắt buộc phải truyền prop này
    //  },
    //},
    components: {
     Button
    },
    computed: {
      ...mapGetters(['getGroupChatSelected', 'isLoading', 'getUserId']),
    },
  };
</script>
<template>
  <div class=" w-100 shadow mb-3 px-4 pt-2" style="height: 60px">
    <router-link to="/videocall" >
      <a v-ripple >
        <span class="pi  pi-video" style="font-size: 30px;"></span>
      </a>
     </router-link>
  </div>
</template>
