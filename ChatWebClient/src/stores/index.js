import { createStore } from 'vuex';
//import websocket from './modules/websocket';
import * as signalR from '@microsoft/signalr';
export default createStore({
  state: {
    token: localStorage.getItem('token') || null, // JWT Token
    user: localStorage.getItem('user') || null,  // User information (e.g., id, username),
    groupChatSelected: 0,
    getGroupChatSelectedCode: "",
    chats: [],
    loading: true,
    message: "",
    connection: null,
    messagesSocket: [],
  },
  mutations: {
    SET_CONNECTION(state, connection) {
      state.connection = connection;
    },
    ADD_MESSAGE(state, message) {
      state.messagesSocket.push(message);
    },
    // Lưu JWT token vào state
    setToken(state, token) {
      state.token = token;
    },
    // Lưu thông tin người dùng vào state
    setUser(state, user) {
      state.user = user;
    },
    // Xóa thông tin khi logout
    clearAuth(state) {
      state.token = null;
      state.user = null;
    },
    updateGroupChat(state, id) {
      state.groupChatSelected = id;
    },
    loadChats(state, list) {
      state.chats = list;
    },
    updateChat(state, chat) {
      state.chats.push(chat);
    },
    isLoading(state, load) {
      state.loading = load;
    },
    updateMessage(state, message) {
      state.message = message;
    },
    updateGroupChatCode(state, code) {
      state.getGroupChatSelectedCode = code;
    }
  },
  actions: {
    // Action để login và lưu token vào state
    login({ commit }, { token, user }) {
      // Lưu token và user vào state
      commit("setToken", token);
      commit("setUser", user);

      // Lưu token vào localStorage để giữ trạng thái sau khi refresh trang
      localStorage.setItem("token", token);
      localStorage.setItem("user", JSON.stringify(user));
    },
    // Action để logout
    logout({ commit }) {
      commit("clearAuth");

      // Xóa token và user từ localStorage
      localStorage.removeItem("token");
      localStorage.removeItem("user");
    },
    // Kiểm tra trạng thái khi refresh trang
    checkAuth({ commit }) {
      const token = localStorage.getItem("token");
      const user = JSON.parse(localStorage.getItem("user"));

      if (token && user) {
        commit("setToken", token);
        commit("setUser", user);
      }
    },
    updateGroupChat({ commit }, id) {
      commit('updateGroupChat', id);
    },
    updateLoadingChat({ commit }, load) {
      commit("isLoading", load);
    },
    updateMessage({ commit }, message) {
      commit("updateMessage", message);
    },
    updateGroupChatCode({ commit }, code) {
      commit("updateGroupChatCode", code);
    },
    updateChats({ commit }, chats) {
      commit("loadChats", chats);
    },
    async connectWebSocket({ commit }) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7223/chatHub") // Đường dẫn WebSocket
        .withAutomaticReconnect()
        .build();

      // Kết nối WebSocket
      connection.on("ReceiveMessage", (obj) => {
        commit("ADD_MESSAGE", JSON.parse(obj));
        commit("updateChat", JSON.parse(obj));
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
    sendMessage({ state }, { message, user, group, groupName }) {
      if (state.connection) {
        state.connection.invoke("SendMessage", message, user, group, groupName).catch((err) => {
          console.error("Send message failed:", err);
        });
      }
    },
  },
  getters: {
    // Lấy trạng thái đăng nhập
    isAuthenticated: (state) => !!state.token,
    // Lấy thông tin user
    getUser: (state) => state.user,
    getUserId: (state) => { let userJ = JSON.parse(state.user); return userJ.id; },
    // Lấy token
    getToken: (state) => state.token,
    getGroupChatSelected: (state) => state.groupChatSelected,
    getChats: (state) => state.chats,
    isLoading: (state) => state.loading,
    getMessage: (state) => state.message,
    getGroupChatSelectedCode: (state) => state.getGroupChatSelectedCode,
    getMessagesSocket: (state) => state.messagesSocket
  }
  //modules: {
  //  websocket,
  //}
})
