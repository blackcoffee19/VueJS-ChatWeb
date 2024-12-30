import * as signalR from "@microsoft/signalr";
import store from "@/stores";
export default class SignalingService {

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7223/videocallhub", {
        accessTokenFactory: () => store.state.token 
      }).withAutomaticReconnect()
      .build();
    if (!window.RTCPeerConnection) {
      console.error("WebRTC không được hỗ trợ trong trình duyệt này.");
      return;
    }
    this.connection.start().catch(err => console.error("Kết nối thất bại: ", err));
  }
  addGroupVideo(groupName) {
    this.connection.invoke("AddToGroup", groupName).catch((err) => {
      console.error("Add to Group Chat Failed:", err);
    });
  }
  sendOffer(offer, targetConnection) {
    this.connection.invoke("SendOffer", JSON.stringify(offer), targetConnection);
  }

  sendAnswer(answer, targetConnection) {
    this.connection.invoke("SendAnswer", JSON.stringify(answer), targetConnection);
  }

  sendICECandidate(candidate, targetConnection) {
    console.log("sendICECandidate " + candidate);
    this.connection.invoke("SendICECandidate", JSON.stringify(candidate), targetConnection);
  }

  onOfferReceived(callback) {
    this.connection.on("offer", callback);
  }

  onAnswerReceived(callback) {
    this.connection.on("answer", callback);
  }

  onICECandidateReceived(callback) {
    this.connection.on("ice-candidate", callback);
  }
}
