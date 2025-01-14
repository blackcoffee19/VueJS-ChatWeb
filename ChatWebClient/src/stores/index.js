import { createStore } from 'vuex';
//import websocket from './modules/websocket';
import * as signalR from '@microsoft/signalr';
import search from './search';
import videocall from './videocall';
import login from './login';
import profile from './profile';
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
    lastestChats: [],
    requirements: [],
    notifications: [],
    countNotification: 0,
    listGroup: [],
    callingFailue: false
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
    },
    loadLastesChats(state, chat) {
      state.lastestChats.push(chat);
    },
    updateLastestChat(state, chat) {
      state.lastestChats.find(x => x.groupId == chat.groupId).text = chat.text;
      state.lastestChats.find(x => x.groupId == chat.groupId).createdAt = chat.createdAt;
      state.lastestChats.find(x => x.groupId == chat.groupId).userId = chat.userId;
    },
    loadRequirements(state, requirements) {
      state.requirements.push(requirements);
    },
    clearRequirement(state) {
      state.requirements = [];
    },
    loadNotifications(state, noti) {
      state.notifications= noti;
    },
    setCountNotification(state, num) {
      state.countNotification = num;
    },
    setListGroup(state, group) {
      let ind = state.listGroup.findIndex(x => x.groupCode == group.groupCode);
      if(ind == -1) state.listGroup.push(group);
    },
    setCalling(state, id) {
      const group = state.listGroup.find(group => group.users.includes(id));
      if (group) {
        group.isCalling = true;
      }
    },
    setCalling2(state, code) {
      const group = state.listGroup.find(group => group.groupCode == code);
      if (group) {
        group.isCalling = false;
      }
    },
    setCallingFailue(state, status) {
      state.callingFailue = status
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
    async connectWebSocket({ dispatch,commit, state }) {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7223/chatHub", {
          accessTokenFactory: () => state.token // Truyền token hoặc thông tin xác thực tại đây
        }) // Đường dẫn WebSocket
        .withAutomaticReconnect()
        .build();

      // Kết nối WebSocket
      connection.on("ReceiveMessage", (obj) => {
        let chat = JSON.parse(obj);
        commit("ADD_MESSAGE", chat);
        if (chat.groupId != undefined && chat.textMessage != undefined && chat.groupId != null && chat.textMessage != null) {
          commit("updateChat", chat);
          commit("updateLastestChat", { groupId: chat.groupId, text: chat.textMessage, createdAt: chat.createdDate, userId: chat.userId });
        }
      });
      connection.on("ReceiveRequest", (obj) => {
        let requirements = JSON.parse(obj);
        let check = true;
        let listReq = Object.assign([], state.requirements);
        for (let i = 0; i < listReq.length; i++) {
          let req = Object.assign({}, listReq[i]);
          if (req.RequesterId == requirements.RequesterId && req.AssigneeId == requirements.AssigneeId) {
            check = false;
          }
        }
        if (check && requirements.Type == 1) commit("loadRequirements", requirements);
        if (check && requirements.Type == 2) {
          //User Accept
           dispatch('search/updateSearchUserStatus', requirements.AssigneeId, { root: true });
        };
        if (check && requirements.Type == 3) {
          dispatch('search/updateSearchUserCancelRequest', requirements.AssigneeId, { root: true });
        };
      });
      connection.on("Calling", (obj) => {
        let requirement = JSON.parse(obj);
        if (requirement.Type == 3 && requirement.Status) {
          commit("setCalling", requirement.RequesterId);
        } else {
          commit("setCallingFailue", true);
        }
      });
      connection.onclose(() => {
        console.log("WebSocket disconnected");
      });

      try {
        await connection.start();
        commit("SET_CONNECTION", connection);
      } catch (error) {
        console.error("WebSocket connection failed:", error);
      }
    },
    addGroupChat({ state, commit }, { groupName , users, name }) {
      if (state.connection) {
        state.connection.invoke("AddToGroup", groupName).catch((err) => {
          console.error("Add to Group Chat Failed:", err);
        });
      }
      commit("setListGroup", { groupCode: groupName, users: users,groupName: name, isCalling: false });
    },
    sendMessage({ state }, { message, user, group, groupName }) {
      if (state.connection) {
        state.connection.invoke("SendMessage", message, user, group, groupName).catch((err) => {
          console.error("Send message failed:", err);
        });
      }
    },
    loadLastesChats({ commit }, chat) {
      commit("loadLastesChats", chat);
    },
    sendRequestSocket({ state }, { userId, requirementId }) {
      if (state.connection) {
        state.connection.invoke("SendRequest", userId, requirementId).catch((err) => {
          console.error("Send message failed:", err);
        });
      }
    },
    sendRequestCalling({ state }) {
      if (state.connection) {
        state.connection.invoke("SendRequestCalling", state.getGroupChatSelectedCode).catch((err) => {
          console.error("Send Request Calling Error: " + err);
        });
      }
    },
    loadNotifications({ commit }, list) {
      commit("loadNotifications", list);
      commit("clearRequirement");
    },
    setCountNotification({ commit }, num) {
      commit("setCountNotification", num);
    },
    minusNotification({ commit, state }) {
      let num = (state.countNotification - 1) >= 0 ? state.countNotification - 1 : 0;
      commit("setCountNotification", num);
    },
    updateNotifications({ state }, { reqId }) {
      let ind = state.notifications.findIndex(x => x.id == reqId);
      if (ind != -1) {
        state.notifications.splice(ind, 1);
      }
    },
    getTimeSpanChat(time) {
      if (time != undefined ) {
        let date = new Date(new Date(time).getTime());
        // Tính chênh lệch thời gian (milliseconds)
        const differenceInMilliseconds = new Date() - date;
        // Chuyển đổi milliseconds sang các đơn vị thời gian
        const days = Math.floor(differenceInMilliseconds / (1000 * 60 * 60 * 24));
        const hours = Math.floor((differenceInMilliseconds % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((differenceInMilliseconds % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((differenceInMilliseconds % (1000 * 60)) / 1000);
        //console.log(`days: ${days}\nhours: ${hours}\nminutes: ${minutes}\nseconds: ${seconds}`);
        return days > 0 ? days + ' days' : (hours > 0 ? hours + ' hours' : (minutes > 0 ? minutes + ' minutes' : seconds + ' seconds')) + ' ago';
      } else {
        return chat.timeSpan;
      }
    }
  },
  getters: {
    isAuthenticated: (state) => !!state.token,
    getUser: (state) => state.user,
    getUserId: (state) => { let userJ = JSON.parse(state.user); return userJ.id; },
    getToken: (state) => state.token,
    getGroupChatSelected: (state) => state.groupChatSelected,
    getChats: (state) => state.chats,
    isLoading: (state) => state.loading,
    getMessage: (state) => state.message,
    getGroupChatSelectedCode: (state) => state.getGroupChatSelectedCode,
    getMessagesSocket: (state) => state.messagesSocket,
    getLastestChats: (state) => state.lastestChats,
    getRequirements: (state) => state.requirements,
    getCountNotification: (state) => {
      return state.requirements.length + state.countNotification;
    },
    getNotifications: (state) => state.notifications,
    getConnection: (state) => state.connection,
    getListGroup: (state) => state.listGroup,
    getCallingFailue: (state) => state.callingFailue
  },
   modules: {
    search, // Đăng ký module search
    videocall,
    login,
    profile
  },
})
