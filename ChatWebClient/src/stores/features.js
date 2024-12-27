export default {
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
    updateSearchUsers: ({ commit }, item) => {
      commit("updateSearchUsers", item);
    }
  },
  getters: {
    getSearchStr: (state) => state.searchStr,
    getSearchUsers: (state) => state.searchUsers
  }
}
