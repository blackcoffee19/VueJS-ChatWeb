import request from "axios";

export default{
    login(crendentials){
        return request.post("https://localhost:7223/api/Auth/login", crendentials);
    },
    register(crendentials){
        return request.post("https://localhost:7223/api/Auth/register", crendentials);
    }
}
