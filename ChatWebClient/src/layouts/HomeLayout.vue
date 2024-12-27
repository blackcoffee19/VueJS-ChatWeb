<script >
  import Navigation from '@/components/Navigation.vue';

  export default {
    components: {
      Navigation,
    },
    mounted() {
      // Kết nối WebSocket khi component được mount
      this.$store.dispatch("connectWebSocket");
    },
    beforeDestroy() {
      if (this.websocket) {
        this.websocket.close(); // Đóng WebSocket khi component bị hủy
      }
    },
  }
</script>

<template>
    <div class="layout overflow-hidden">
        <!-- Navigation -->
        <Navigation  />
        <!-- Navigation -->
        <router-view></router-view>
    </div>
</template>
<style>
.layout{
    display: flex;
    flex-direction: row;
    height: 100%;
}
</style>
