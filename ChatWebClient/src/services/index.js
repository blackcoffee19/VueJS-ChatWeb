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
    return api.get(`/Chat/${stores.getters.getUserId}/${id}`)
  },
  postChat(obj) {
    return api.post('/Chat', obj);
  }
}
