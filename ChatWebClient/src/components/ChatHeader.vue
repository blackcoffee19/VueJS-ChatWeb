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
      ...mapGetters(['getGroupChatSelected', 'getListGroup','getGroupChatSelectedCode']),
    },
    methods: {
      getGroupChatName() {
        const group = this.getListGroup.find((g) => g.groupCode === this.getGroupChatSelectedCode);
        return group ? group.groupName:"";
      }
    }
  };
</script>
<template>
  <div class="shadow mb-3 px-4 pt-2 position-fixed top-0 z-1" style="height: 60px; width: -webkit-fill-available">
    <div class="flex flex-row px-5 ">
      <div class="flex-grow-1 fw-bold fs-4 font-monospace pt-1">
        {{getGroupChatName()}}
      </div>
      <div >
        <router-link to="/videocall" >
          <a>
            <span class="pi pi-video" style="font-size: 30px;"></span>
          </a>
         </router-link>
      </div>
    </div>
  </div>
</template>
