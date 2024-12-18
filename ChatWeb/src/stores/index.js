import { createStore } from 'vuex';

export default createStore({
  state: {
    token: null, // JWT Token
    user: null,  // User information (e.g., id, username)
  },
  mutations: {
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
  },
  getters: {
    // Lấy trạng thái đăng nhập
    isAuthenticated: (state) => !!state.token,
    // Lấy thông tin user
    getUser: (state) => state.user,
    // Lấy token
    getToken: (state) => state.token,
  },
})