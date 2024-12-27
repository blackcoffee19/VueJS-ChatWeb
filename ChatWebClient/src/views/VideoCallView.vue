<script lang="js">
  import { Button} from 'primevue';
  import SimplePeer from 'simple-peer';
  import connection from '@/services/video_call_connection';
 export default {
    components: {
     Button
    },
    data() {
      return {
        localStream: null,
        peers: {},
      };
    },
    async mounted() {
      connection.on('UserJoined', this.handleUserJoined);
      connection.on('UserLeft', this.handleUserLeft);
      connection.start();
      this.initLocalStream();
    },
    methods: {
      async initLocalStream() {
        if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
          navigator.mediaDevices.getUserMedia({ video: true, audio: true })
            .then(stream => {
              this.localStream = stream;
              this.$refs.localVideo.srcObject = this.localStream;
            })
            .catch(error => {
              console.error('Error accessing media devices.', error);
            });
        } else {
          console.error('getUserMedia is not supported in this browser.');
        }
      },
      async startCall() {
        connection.invoke('JoinGroup', 'group1');
      },
      handleUserJoined(userId) {
        const peer = new SimplePeer({ initiator: true, stream: this.localStream });
        peer.on('signal', signal => {
          connection.invoke('SendMessage', 'group1', JSON.stringify({ to: userId, signal }));
        });
        peer.on('stream', stream => {
          this.$refs[`peerVideo-${userId}`].srcObject = stream;
        });
        this.peers[userId] = peer;
      },
      handleUserLeft(userId) {
        delete this.peers[userId];
      },
    },
  };
</script>
<template>
  <div class=" w-100 h-100" >
    <div>
      <div>
        <video ref="localVideo" autoplay playsinline muted></video>
        <div v-for="(peer, id) in peers" :key="id">
          <video :ref="'peerVideo-' + id" autoplay playsinline></video>
        </div>
      </div>
      <button @click="startCall">Start Call</button>
    </div>
  </div>
</template>
