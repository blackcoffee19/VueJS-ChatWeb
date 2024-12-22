import * as signalR from '@microsoft/signalr';

const state = {
  connection: null,
  messagesSocket: [],
};

const mutations = {
  SET_CONNECTION(state, connection) {
    state.connection = connection;
  },
  ADD_MESSAGE(state, message) {
    state.messagesSocket.push(message);
  },
};

const actions = {
  async connectWebSocket({ commit }) {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7223/chatHub") // Đường dẫn WebSocket
      .withAutomaticReconnect()
      .build();

    // Kết nối WebSocket
    connection.on("ReceiveMessage", (obj) => {
      commit("ADD_MESSAGE", JSON.parse(obj));
      dispatch("updateChat", JSON.parse(obj), { root: true }).then(() => console.log("Dispatch successful"))
        .catch((error) => console.error("Dispatch failed:", error));;
    });

    connection.onclose(() => {
      console.log("WebSocket disconnected");
    });

    try {
      await connection.start();
      console.log("WebSocket connected");
      commit("SET_CONNECTION", connection);
    } catch (error) {
      console.error("WebSocket connection failed:", error);
    }
  },
  addGroupChat({ state }, { groupName }) {
    if (state.connection) {
      state.connection.invoke("AddToGroup", groupName).catch((err) => {
        console.error("Add to Group Chat Failed:", err);
      });
    } 
  },
  sendMessage({ state }, { message, user, group , groupName}) {
    if (state.connection) {
      state.connection.invoke("SendMessage", message, user, group, groupName).catch((err) => {
        console.error("Send message failed:", err);
      });
    }
  },
};
const getters = {
  getMessagesSocket: () => state.messagesSocket
}
export default {
  namespaced: true,
  state,
  mutations,
  actions,
  getters
};
