export default {
    namespaced: true,
    state:{
        activeBtn: false,
        error: false,
        message: "",
        username: "",
        password: "",
        fullname: ""
    }, 
    mutations: {
        setActiveBtn(state, status){
            state.activeBtn = status;
        },
        setError(state, status){
            state.error = status;
        },
        setMessage(state, message){
            state.message = message;
        },
        setUsername(state, username){
            state.username = username;
        },
        setPassword(state, password){
            state.password = password;
        },
        setFullname(state, fullname){
            state.fullname = fullname;
        }
    },
    actions: {
        toggleActive({commit, state}){
            commit("setActiveBtn", !state.activeBtn);
            commit("setError", false);
            commit("setMessage","");
            commit("setUsername", "");
            commit("setPassword","");
            commit("setFullname", "");
        },
        setError({commit}, status){
            commit("setError", status);
        },
        setMessage({commit}, message){
            commit("setMessage",message);
        },
        setUsername({commit}, event){
            commit("setUsername", event.target.value);
        },
        setPassword({commit}, event){
            commit("setPassword", event.target.value);
        },
        setFullname({commit}, event){
            commit("setFullname", event.target.value);
        }
    },
    getters: {
        getActiveBtn:(state)=> state.activeBtn,
        getError: (state) => state.error,
        getMessage: (state) => state.message
    }
}