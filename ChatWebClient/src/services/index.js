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
  postAddFriendActions(id, isUserId, isAccept = true) {
    return api.post(`/Users/AddFriendActions?reqId=${id}&isUserId=${isUserId?1:0}&actions=${isAccept ? 1 : 0}`);
  },
  postGetConnectionId(idGroup) {
    return api.post(`/Connection/GetUserConnection?id=${idGroup}`);
  },
  postGetUserProfile() {
    return api.post(`/Users/profie`);
  },
  postUnFriend(userId) {
    return api.post(`/Users/UnfriendAction?userId=${userId}`);
  }
}
