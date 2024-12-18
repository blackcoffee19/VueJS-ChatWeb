// import Api from "./../index.js";
import request from "axios";

export default{
    login(crendentials){
        console.log(crendentials.username, crendentials.password);
        return request.post("https://localhost:7223/api/Auth"+"/login",{
        username: crendentials.username,
        password: crendentials.password
    }).then(result => console.log(result))},
    register(){return Api.post('/register')}
}