export default {
  namespaced: true,
  state: {
    searchStr: "",
    searchUsers: []
  },
  mutations: {
    updateSearchStr: (state, string) => {
      state.searchStr = string;
    },
    updateSearchUsers: (state, item) => {
      state.searchUsers = item;
    }
  },
  actions: {
    inputSearch: ({ commit }, string) => {
      commit("updateSearchStr", string);
    },
    updateSearchUsers: ({ commit }, item) =>{
      commit("updateSearchUsers", item);
    },
    updateSearchUserCancelRequest: ({ state }, id) => {
      console.log(id);
      for (let i = 0; i < state.searchUsers.length; i++) {
        if (state.searchUsers[i].usID == id) {
          state.searchUsers[i].isSentRequest = false;
          break;
        }
      }
    },
    updateSearchUserStatus: ({ state }, id) => {
      for (let i = 0; i < state.searchUsers.length; i++) {
        if (state.searchUsers[i].usID == id) {
          state.searchUsers[i].isFriend = true;
          break;
        }
      }
    },
  },
  getters: {
    getSearchStr: (state) => state.searchStr,
    getSearchUsers: (state) => state.searchUsers
  }
}
