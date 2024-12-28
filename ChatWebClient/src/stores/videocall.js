import SignalingService from "@/services/SignalingService"
export default {
  namespaced: true,
  state: {
    localStream: null,
    remoteStream: null,
    peerConnection: null,
    signalingService: null,
    iceCandidates: [],
    listOffer: [],
    connectionId: null,
    isOffer: false,
    connectSuccess: false
  },
  mutations: {
    SET_CONNECTION(state, connection) {
      state.connection = connection;
    },
    ADD_MESSAGE(state, message) {
      state.messagesSocket.push(message);
    },
    SET_SERVICEVIDEO(state, service) {
      state.signalingService = service;
    },
    setLocalStream(state, stream) {
      state.localStream = stream;
    },
    setRemoteStream(state, stream) {
      state.remoteStream = stream;
    },
    setPeerConnection(state, connection) {
      state.peerConnection = connection;
    },
    addIceCandidate(state, candidate) {
      state.iceCandidates.push(candidate);
    },
    clearIceCandidates(state) {
      state.iceCandidates = [];
    },
    addOffer(state, offer) {
      state.listOffer.push(offer);
    },
    setConnectionId(state, connectionId) {
      state.connectionId = connectionId;
    },
    setIsOffer(state, isCalling) {
      state.isOffer = isCalling;
    },
    setConnectedSuccess(state, connect) {
      state.connectSuccess = connect;
    }
  },
  actions: {
    setLocalStream({ commit }, stream) {
      commit("setLocalStream", stream);
    },
    setRemoteStream({ commit }, stream) {
      commit("setRemoteStream", stream);
    },
    setPeerConnection({ commit }, connection) {
      commit("setPeerConnection", connection);
    },
    setConnectionString({ commit }, connection) {
      commit("setConnectionString", connection);
    },
    addIceCandidate({ commit }, candidate) {
      commit("addIceCandidate", candidate);
    },
    addOffer({ commit }, offer) {
      const offerParse = typeof offer === 'string' ? JSON.parse(offer) : offer;
      commit("addOffer", offerParse);
    },
    connectServiceVideoCall({ commit }) {
      commit("SET_SERVICEVIDEO", new SignalingService());
    },
    setConnectionId({ commit }, connectionId) {
      commit("setConnectionId", connectionId);  
    },
    async addGroupVideo({ state, dispatch }, groupname) {     
      state.signalingService.addGroupVideo(groupname);
    },
    async handleOffer({ state, commit, rootState, dispatch }, offer) {
      try {
        const offerParse = typeof offer === 'string' ? JSON.parse(offer) : offer;
        const offerDescription = new RTCSessionDescription(offerParse);
        commit("addOffer", offerParse);
        if (!state.peerConnection || state.peerConnection.signalingState === "closed") {
          console.log("Reinitializing PeerConnection...");
          await dispatch("initializePeerConnection");
        }

        console.log("Setting remote description...");
        await state.peerConnection.setRemoteDescription(offerDescription);
        console.log("Remote description is", state.peerConnection.remoteDescription ? "set" : "not set");//Set đã nhận được kết nối của user kia
        if (state.peerConnection.remoteDescription) {
          commit("setIsOffer", true);
        }
        // Thêm các ICE Candidate đã lưu
        if (state.iceCandidates.length > 0) {
          console.log("Adding stored ICE Candidates...");
          for (const candidate of state.iceCandidates) {
            await state.peerConnection.addIceCandidate(new RTCIceCandidate(candidate))
              .catch(error => console.error("Error adding ICE Candidate:", error));
          }
          commit("clearIceCandidates") // Xóa danh sách sau khi xử lý
        }

        // Tạo và gửi answer
        const answer = await state.peerConnection.createAnswer();
        await state.peerConnection.setLocalDescription(answer);

        console.log("Sending Answer...");
        state.signalingService.sendAnswer(answer, state.connectionId);
      } catch (error) {
        console.error("Error handling offer:", error);
      }
    },

    async handleAnswer({ state, commit, rootState, dispatch }, answer) {
      try {
        const parsedAnswer = typeof answer === 'string' ? JSON.parse(answer) : answer;

        if (!state.peerConnection) {
          console.error("PeerConnection is not initialized.");
          await  dispatch('initializePeerConnection');
        }

        const currentState = state.peerConnection.signalingState;
        console.log("Current signaling state:", currentState);

        if (currentState === "have-local-offer") {
          await state.peerConnection.setRemoteDescription(new RTCSessionDescription(parsedAnswer));
          commit("setConnectedSuccess", true);
          console.log("Answer successfully set as RemoteDescription."); //Kết nối 2 stream hoàn thành
        } else {
          console.error(
            `Cannot setRemoteDescription in state: ${currentState}. Expected state: 'have-local-offer'.`
          );
        }
      } catch (error) {
        console.error('Error handling answer:', error);
      }
    },
    initializePeerConnection({ state, dispatch }) {
      const configuration = {
        iceServers: [
          { urls: 'stun:stun.l.google.com:19302' }, // Thay bằng STUN server phù hợp
        ]
      };

      state.peerConnection = new RTCPeerConnection(configuration);

      // Lắng nghe sự kiện ICE Candidate (khi có candidate mới từ WebRTC)
      state.peerConnection.addEventListener("icecandidate", (event) => {
        dispatch("handleICECandidateEvent", event);  // Gửi candidate tới signaling server
      });

      // Lắng nghe sự kiện Track (khi có track mới từ peer)
      state.peerConnection.addEventListener("track", (event) => {
        dispatch("handleTrackEvent", event);  // Cập nhật track video/audio cho remote peer
      });

      // Theo dõi trạng thái signaling (open, closed, connecting)
      state.peerConnection.addEventListener("signalingstatechange", () => {
        console.log("Signaling state changed to:", state.peerConnection.signalingState);
      });

      // Thêm sự kiện ICE connection state change để theo dõi trạng thái kết nối ICE
      state.peerConnection.addEventListener("iceconnectionstatechange", () => {
        console.log("ICE connection state changed to:", state.peerConnection.iceConnectionState);
      });

      console.log("PeerConnection initialized.");
    },
    handleICECandidate({ state, dispatch }, candidate) {
      try {
        const parsedCandidate = typeof candidate === "string" ? JSON.parse(candidate) : candidate;
        console.log("Remote description is", state.peerConnection.remoteDescription ? "set" : "not set");
        if (!state.peerConnection || !state.peerConnection.remoteDescription) {
          console.warn("Remote description not set yet. Storing ICE Candidate.");
          state.iceCandidates.push(parsedCandidate);
          return;
        }
        if (!state.peerConnection || state.peerConnection.signalingState === 'closed') {
          console.log("Re-initializing PeerConnection...");
          dispatch("initializePeerConnection");
        }
        console.log("Adding ICE Candidate to peerConnection...");
        state.peerConnection.addIceCandidate(new RTCIceCandidate(parsedCandidate))
          .catch(error => console.error("Error adding ICE Candidate:", error));
      } catch (error) {
        console.error("Error handling ICE Candidate:", error);
      }
    },
    async startCall({ state, dispatch, rootState, commit }) {
      console.log("Star call")
      // Truy cập media của người dùng (camera và microphone)
      try {
        // Khởi tạo peerConnection nếu cần
        if (!state.peerConnection) {
          await dispatch('initializePeerConnection');
        }
        const videoElement = document.createElement('video');
        videoElement.src = '/videos/hinata-la.mp4';
        videoElement.muted = true;
        await videoElement.play();
        const stream = await navigator.mediaDevices.getUserMedia({
          video: true,
          audio: true,
        }).catch(() => videoElement.captureStream());
        commit("setLocalStream", stream); // Lưu stream vào state
        // Thêm track vào peerConnection
        if (!state.peerConnection || state.peerConnection.signalingState === "closed") {
          console.log("Reinitializing PeerConnection...");
          await dispatch("initializePeerConnection");
        }

        state.localStream.getTracks().forEach(track => {
          state.peerConnection.addTrack(track, state.localStream);
        });
        // Tạo offer
        const offer = await state.peerConnection.createOffer();
        await state.peerConnection.setLocalDescription(offer);

        // Gửi offer tới server signaling (ASP.NET Web API)
        if (rootState.getGroupChatSelectedCode == null || rootState.getGroupChatSelectedCode == "") {
          this.$router.push('/')
        }
        state.signalingService.sendOffer(offer, state.connectionId);
      } catch (error) {
        console.error('Lỗi khi bắt đầu cuộc gọi:', error);
      }
    },
    endCall({ state, commit, rootState }) {
      if (state.peerConnection && state.peerConnection.connectionState !== "closed") {
        state.peerConnection.close();
      }
      // Đóng peer connection và reset streams
      if (state.localStream) {
        state.localStream.getTracks().forEach(track => track.stop());
        commit("setLocalStream", null);
      }
      commit("setRemoteStream", null);
      commit("setIsOffer", false);
      commit("setConnectedSuccess", false);
      commit("setCallingFailue", false, {root: true});
      commit("setCalling2", rootState.getGroupChatSelectedCode, { root: true });
    },
    beforeDestroy({ state, commit }) {
      if (state.peerConnection && state.peerConnection.connectionState !== "closed") {
        state.peerConnection.close();
      }
      // Đóng peer connection và reset streams
      if (state.localStream) {
        state.localStream.getTracks().forEach(track => track.stop());
        commit("setLocalStream", null);
      }
      commit("setRemoteStream", null);
      commit("setIsOffer", false);
      commit("setConnectedSuccess", false);
      commit("setCallingFailue", false, { root: true });
      commit("setCalling2", rootState.getGroupChatSelectedCode, { root: true });
    },
    handleICECandidateEvent({ state, rootState }, event) {
      console.log("handleICECandidateEvent", event);  // Log the event object
      if (event.candidate && this.getGroupChatSelectedCode != null) {
        state.signalingService.sendICECandidate(event.candidate, rootState.getGroupChatSelectedCode);
      }
    },

    handleTrackEvent({ state, commit },event) {
      console.log("Track added to remote stream:", event.track);
      if (event.streams && event.streams[0]) {
        console.log("Remote stream found:", event.streams[0]);
        commit("setRemoteStream", event.streams[0]);
      } else {
        console.error("No remote stream available in event.");
      }
    }
    
  },
  getters: {
    getLocalStream: (state) => state.localStream,
    getRemoteStream: (state) => state.remoteStream,
    getPeerConnection: (state) => state.peerConnection,
    getIceCandidates: (state) => state.iceCandidates,
    getListOffer: (state) => state.listOffer,
    getIsOffer: (state) => state.isOffer
  }

  
}
