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
    isWaiting: false,
    connectSuccess: false,
    isVideoCallView: false
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
    clearOffer(state) {
      state.listOffer = [];
    },
    setConnectionId(state, connectionId) {
      state.connectionId = connectionId;
    },
    setIsOffer(state, isCalling) {
      state.isOffer = isCalling;
    },
    setConnectedSuccess(state, connect) {
      state.connectSuccess = connect;
    },
    setIsVideoCallView(state, status) {
      state.isVideoCallView = status;
    },
    setIsWaiting(state, status) {
      state.isWaiting = status;
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
    setIsVideoCallView({ commit }, status) {
      commit("setIsVideoCallView", status);
    },
    setIsOffer({ commit }, status) {
      commit("setIsOffer", status);
    },
    async addGroupVideo({ state, dispatch }, groupname) {     
      state.signalingService.addGroupVideo(groupname);
    },
    async handleOffer({ state, commit, rootState, dispatch }, offer) {
      if (state.isVideoCallView) {
        try {
          const offerParse = typeof offer === 'string' ? JSON.parse(offer) : offer;
          const offerDescription = new RTCSessionDescription(offerParse);
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

      } else {
        console.log("Handle Offer but this is not Video Call View");
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

        // Chờ signalingState về 'stable' nếu không ở trạng thái hợp lệ
        if (!["stable", "have-local-offer"].includes(currentState)) {
          console.log("Waiting for signaling state to stabilize...");
          await new Promise(resolve => {
            const listener = () => {
              if (state.peerConnection.signalingState === "stable" ||
                state.peerConnection.signalingState === "have-local-offer") {
                state.peerConnection.removeEventListener("signalingstatechange", listener);
                resolve();
              }
            };
            state.peerConnection.addEventListener("signalingstatechange", listener);
          });
        }
        if (currentState === "have-local-offer"  ) {
          await state.peerConnection.setRemoteDescription(new RTCSessionDescription(parsedAnswer));
          commit("setConnectedSuccess", true);
          commit("setIsWaiting", false);
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
    initializePeerConnection({ state, dispatch, commit }) {
      //      Kết nối nằm trong mạng cục bộ hoặc dùng STUN / TURN 
      //  Nếu cả hai thiết bị nằm trong cùng mạng cục bộ(LAN), trình duyệt có thể tự động kết nối mà không cần nhiều ICE Candidates.
      //  Hoặc STUN / TURN server đã cung cấp đủ thông tin cho kết nối.
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
        if (state.peerConnection.iceConnectionState == "disconnected") {
          commit("setConnectedSuccess", false);
          commit("setIsOffer", false);
          commit("setRemoteStream", null);
        }
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
      console.log("Start call");
      commit("setIsWaiting", true);
      try {
        // Khởi tạo peerConnection nếu cần
        if (!state.peerConnection || state.peerConnection.connectionState === "closed") {
          console.log("Initializing PeerConnection...");
          await dispatch("initializePeerConnection");
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
       
        if (!state.peerConnection || state.peerConnection.signalingState === "closed") {
          console.log("Reinitializing PeerConnection...");
          await dispatch("initializePeerConnection");
        }

        //Thêm các track (dữ liệu phương tiện như audio hoặc video) từ luồng phương tiện cục bộ (state.localStream) vào kết nối peerConnection
        //Mục đích:
        //  _ Trình duyệt sẽ tự động thiết lập các kênh truyền thông (RTP) để gửi các track này đến peer (thiết bị từ xa) qua mạng.
        //  _ Đầu xa(remote peer) có thể nhận các track đó và hiển thị video hoặc phát âm thanh.
        state.localStream.getTracks().forEach(track => {
          state.peerConnection.addTrack(track, state.localStream);
        });

        // Tạo Offer
        const offer = await state.peerConnection.createOffer();
        await state.peerConnection.setLocalDescription(offer);

        if (state.connectionId == null || state.connectionId == 0 || state.connectionId == "") {
          this.$router.push('/')
        }

        // Gửi offer tới server signaling 
        state.signalingService.sendOffer(offer, state.connectionId);

        //Kiểm tra icecandidate có được gửi chưa
        //state.peerConnection.onicecandidate = (event) => {
        //  if (event.candidate) {
        //    console.log('ICE Candidate:', event.candidate);
        //  } else {
        //    console.log('All ICE Candidates have been sent');
        //  }
        //};
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
      //commit("clearOffer");
      commit("setRemoteStream", null);
      commit("setIsOffer", false);
      commit("setConnectedSuccess", false);
      commit("setCallingFailue", false, {root: true});
      commit("setCalling2", rootState.getGroupChatSelectedCode, { root: true });
    },
    beforeDestroy({ state, commit, rootState }) {
      if (state.peerConnection ) {
        state.peerConnection.close();
        commit("setPeerConnection", null); // Đảm bảo peerConnection được reset
      }
      if (state.localStream) {
        state.localStream.getTracks().forEach(track => track.stop());
        commit("setLocalStream", null);
      }
      commit("setRemoteStream", null);
      commit("setConnectedSuccess", false);

      commit("setCallingFailue", false, { root: true });
      commit("setCalling2", rootState.getGroupChatSelectedCode, { root: true });
    },
    handleICECandidateEvent({ state, rootState }, event) {
      if (event.candidate && this.getGroupChatSelectedCode != null) {
        console.log("Send ICE Candidate ",event);
        state.signalingService.sendICECandidate(event.candidate, rootState.getGroupChatSelectedCode);
      }
    },

    handleTrackEvent({ state, commit },event) {
      if (event.streams && event.streams[0]) {
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
    getIsOffer: (state) => state.isOffer,
    getIsWaiting: (state) => state.isWaiting
  }

  
}
