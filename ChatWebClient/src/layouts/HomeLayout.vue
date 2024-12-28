<script >
  import Navigation from '@/components/Navigation.vue';
  import SignalingService from '@/services/SignalingService';
  import { mapActions } from 'vuex';
  export default {
    components: {
      Navigation,
    },
    mounted() {
      // Kết nối WebSocket khi component được mount
      this.$store.dispatch("connectWebSocket");
      this.$store.dispatch("videocall/connectServiceVideoCall");
      //this.$store.state.videocall.signalingService.onOfferReceived(this.$store.dispatch("videocall/addOffer"));
      this.$store.state.videocall.signalingService.onOfferReceived(this.onOfferReceived);
    },
    beforeDestroy() {
      if (this.websocket) {
        this.websocket.close(); // Đóng WebSocket khi component bị hủy
      }
    },
    methods: {
      ...mapActions("videocall", ["addOffer"]),
      onOfferReceived(offer) {
        this.addOffer(offer);
      },
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
