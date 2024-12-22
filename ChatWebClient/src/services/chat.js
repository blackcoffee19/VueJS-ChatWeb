import api from './index';
import store from '@/stores';
export default {
  async GetListChats(id) {
    store.dispatch("updateLoadingChat", true);
    try {
      let response = await api.getListChats(id);
      store.dispatch("updateGroupChat", id);
      store.dispatch("updateChats", response.data);
    } catch (err) {
      console.log(err);
      store.dispatch("updateLoadingChat", true);
    } finally {
      store.dispatch("updateLoadingChat", false);
      store.dispatch("updateMessage", '');
    }
  },
  async SendMessage() {
    let text = store.getters.getMessage;
    let userId = store.getters.getUserId;
    let groupId = store.getters.getGroupChatSelected;
    if (text != null && text != "" && userId != 0 && groupId != 0) {
      try {
        let response = await api.postChat({ TextMessage: text, UserId: userId, GroupId: groupId });
        if (response.status == 200) {
          return 1;
        }
      } catch (err) {
        console.log(err);
      }
      return 0;
    }
    return 2;
  }
}
