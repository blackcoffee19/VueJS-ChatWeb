<script lang="js">
  import { mapGetters, mapActions, mapState } from 'vuex';
  import { Button} from 'primevue';
  import UserService from '@/services';
  import store from '@/stores';
 export default {
    components: {
     Button
    },
    computed: {
      ...mapGetters(['getGroupChatSelected', 'getCallingFailue']),
      ...mapState('videocall', ['peerConnection', 'localStream', 'remoteStream', 'signalingService', 'isOffer', 'connectSuccess', 'listOffer','isWaiting']),
    },
    watch: {
      // Khi stream thay đổi, cập nhật video element
      localStream(newStream) {
        console.log("LocalStream set");
        if (this.$refs.localVideo) {
          this.$refs.localVideo.srcObject = newStream;
        }
      },
      remoteStream(newStream) {
        console.log("RemoteStream set");
        if (this.$refs.remoteVideo) {
          this.$refs.remoteVideo.srcObject = newStream;
        }
      }
    },
    mounted() {
      this.setIsVideoCallView(true);
      UserService.postGetConnectionId(this.getGroupChatSelected).then((response) => {
        if (response.status == 200) {
          store.dispatch('videocall/setConnectionId', response.data.connectionId);
        }
      }).catch(err => {
        console.log(err);
        this.$router.push('/').then(x => window.location.reload());
      });
      let listOfferRecived = Object.assign([], this.listOffer);
      if (listOfferRecived.length > 0) {
        let offer = Object.assign({}, listOfferRecived[listOfferRecived.length-1]);
        this.handleOffer(offer);
      }
      this.signalingService.onOfferReceived(this.handleOffer);
      this.signalingService.onAnswerReceived(this.handleAnswer);
      this.signalingService.onICECandidateReceived(this.handleICECandidate);
    },
    methods: {
      ...mapActions(['sendRequestCalling']),
      ...mapActions('videocall', ['handleOffer', 'handleAnswer', 'handleICECandidate', 'startCall', 'endCall', 'beforeDestroy', 'setIsVideoCallView','setIsOffer']),
      handleStartCall() {
        this.startCall();
        this.sendRequestCalling();
      }
    },
    beforeUnmount() {
      this.beforeDestroy();
      this.setIsVideoCallView(false);
      this.setIsOffer(false);
    },
  };
</script>
<style scoped>
  video {
    width: 300px;
    height: 200px;
  }
</style>
<template>
  <div class=" w-100 h-100">
    <div class=" h-100">
      <h1>Video Call</h1>
      <!--<div v-if="this.peerConnection && this.peerConnection.connectionState === 'connected'">
        <p>Call is active!</p>
      </div>
      <div v-else>
        <p>Waiting for connection...</p>
      </div>
      <div v-if="this.peerConnection">this.peerConnection.connectionState</div>-->
      <div v-if="getCallingFailue">
        <p>User is offline. You can not call them now!</p>
      </div>
      <div class="flex flex-column h-100">
        <div class="flex flex-row w-25 px-4 py-1 justify-content-around align-items-center">
          <div :class="connectSuccess? 'd-none': 'd-block'">
            <Button @click="handleStartCall" v-if="isOffer && !isWaiting">
              <i class="pi pi-phone"></i>Accept
            </Button>
            <Button v-if="isWaiting" severity="warn">
              Calling...
            </Button>
            <Button @click="handleStartCall" v-if="!isOffer && !isWaiting">
              <i class="pi pi-phone"></i>Start
            </Button>

          </div>
          <Button icon="pi pi-power-off" @click="endCall" severity="danger" label="End" />
        </div>
        <div class="flex flex-row h-75">
          <div class="w-50 h-100 p-3 mb-2 bg-black bg-gradient">
            <video ref="localVideo" id="localVideo" style="height:100%; width: 100%" autoplay muted></video>
          </div>
          <div class="w-50 h-100 p-3 mb-2 bg-black bg-gradient">
            <video ref="remoteVideo" id="remoteVideo" style="height:100%; width: 100%" autoplay></video>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>
