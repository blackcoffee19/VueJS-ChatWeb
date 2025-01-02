import AuthService from "../../services/auth";

const state = {};
const getters = {};
const mutations = {
    DO_SOMETHING(){
        console.log("function from mutations...");
    }
};
const actions = {
    login({commit}, crendentials){
        AuthService.login(crendentials);
        console.log("Login from Store");
        console.log(crendentials.username , crendentials.password);
        commit("DO_SOMETHING");
    }
}; 


export default {
    namespaced : true,
    state,
    getters,
    mutations,
    actions
}