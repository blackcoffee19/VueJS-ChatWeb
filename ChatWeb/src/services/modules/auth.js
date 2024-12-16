// import Api from "./../index.js";
import request from "axios";

export default{
    login(crendentials){
        console.log(crendentials.username, crendentials.password);
        return request.post("http://localhost:6000"+"/login",{
        username: crendentials.username,
        password: crendentials.password
    })},
    register(){return Api.post('/register')}
}