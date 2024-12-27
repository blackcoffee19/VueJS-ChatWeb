import request from "axios";
import stores from '@/stores';
const api = request.create({
    baseURL: "https://localhost:7223/api",
    headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer '+ stores.getters.getToken
    },
});
export default {
  getListGroupChat (){
      return api.get('/Users/ListGroupChat');
  },
  getListChats(id) {
    return api.get(`/Chat/${id}`)
  },
  postChat(obj) {
    return api.post('/Chat', obj);
  },
  getSearchUsers() {
    return api.post(`/Users/SearchUser?search=${stores.getters['search/getSearchStr']}`);
  },
  postAddRequest(idUser) {
    var obj = {
      "AssigneeId": idUser,
      "Type": 1,
      "Status": true
    }
    return api.post('/Users/AddRequest', obj);
  },
  sendCancelRequest(idUser) {
    var obj = {
      "AssigneeId": idUser,
      "Type": 1,
      "Status": false
    }
    return api.post('/Users/CancelRequest', obj);
  },
  getNotifications() {
    return api.get('/Users/ListNotifications');
  },
  postAddFriendActions(id, isAccept = true) {
    return api.post(`/Users/AddFriendActions?reqId=${id}&actions=${isAccept ? 1 : 0}`);
  }
}
