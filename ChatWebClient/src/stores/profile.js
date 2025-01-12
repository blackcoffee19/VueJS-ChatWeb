export default {
  namespaced: true,
  state: {
    user: null,
    isOwnAccount: false
  },
  mutations: {
    setUser: (state, userInfo)=> {
      state.user = userInfo
    },
    setIsOwnAccount: (state, check) => {
      state.isOwnAccount = check
    }
  },
  actions: {
    setUser: ({ commit }, user) => {
      commit("setUser", user);
    },
    setIsOwnAccount: ({ commit }, check) => {
      commit("setIsOwnAccount", check);
    },
  },
  getters: {
    getUser: (state) => state.user,
    getIsOwnAccount: (state) => state.isOwnAccount,
    getUserImg: (state) => {
      return state.user.avatar ?? "tuong.png";
    },
  }
}
