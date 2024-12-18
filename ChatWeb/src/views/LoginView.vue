<script setup lang="js">
import axios from 'axios';
import FloatLabel from 'primevue/floatlabel';
import InputText from 'primevue/inputtext';
import { ref } from 'vue';
const activeBtn = ref(false);
function register(){
    activeBtn.value=!activeBtn.value;
}

</script>
<script lang="js">
import '../assets/login.css'
import Password from 'primevue/password';
export default {
    data(){
        return {
            username: "",
            password: ""
        }
    },
    methods: {
    async onLogin() {
      try {
        const response = await axios.post("https://localhost:7223/api/Auth/login", {
          username: this.username,
          password: this.password,
        });
        const token = response.data.token; // JWT Token từ API
        const user = { id: response.data.userId, username: this.username }; // User info
        // Gọi action login từ Vuex store
        this.$store.dispatch("login", { token, user });

        // Chuyển hướng sau khi login thành công
        this.$router.push("/");
      } catch (error) {
        console.error("Login failed:", error);
      }
    },
  },
}
</script>

<template>
    <div class="login-body">
        <div class="container" id="container" :class="{ active: activeBtn}">
            <div class="form-container sign-up" >
                <form>
                    <h1>Create Account</h1>
                    <div class="social-icons ">
                        <a href="#" class="icon">
                            <i class="pi pi-google"></i>
                        </a>
                        <a href="#" class="icon">
                            <i class="pi pi-facebook"></i>
                        </a>
                        <a href="#" class="icon">
                            <i class="pi pi-github"></i>
                        </a>
                        <a href="#" class="icon">
                            <i class="pi pi-twitter"></i>
                        </a>
                    </div>
                    <span>or use your email for registeration</span>
                    <div>
                        <FloatLabel>
                            <InputText id="username2"  />
                            <label for="username2">Username</label>
                        </FloatLabel>
                        <FloatLabel>
                            <InputText id="email2"  />
                            <label for="email2">Email</label>
                        </FloatLabel>
                        <FloatLabel>
                            <Password id="password2" v-model="password" />
                            <label for="password2">Password</label>
                        </FloatLabel>
                    </div>
                    <button>Sign Up</button>
                </form>
    
            </div>
            <div class="form-container sign-in">
                <form @submit.prevent="onLogin">
                    <h1>Sign In</h1>
                    <div class="social-icons ">
                        <a href="#" class="icon">
                            <i class="pi pi-facebook"></i>
                        </a>
                        <a href="#" class="icon">
                            <i class="pi pi-google"></i>
                        </a>
                        <a href="#" class="icon">
                            <i class="pi pi-github"></i>
                        </a>
                        <a href="#" class="icon">
                            <i class="pi pi-twitter"></i>
                        </a>
                    </div>
                    <span>or use your username for registeration</span>
                    <div>
                        <FloatLabel>
                            <InputText id="username" v-model="username"  />
                            <label for="username">Username</label>
                        </FloatLabel>
                        <FloatLabel>
                            <Password id="password" v-model="password" />
                            <label for="password">Password</label>
                        </FloatLabel>
                    </div>
                    <a class="#">Forget Your Password</a>
                    <button>Sign Up</button>
                </form>
    
            </div>
            <div class="toggle-container">
                <div class="toggle">
                    <div class="toggle-panel toggle-left">
                        <h1 style="color: #FFF;">Welcome Back!</h1>
                        <p>Enter your personal details to use all of site features </p>
                        <button class="hidden" id="login" @click="register">Sign In</button>
                    </div>
                    <div class="toggle-panel toggle-right">
                        <h1 style="color: #FFF;">Hello, Friend!</h1>
                        <p>Register with your details to use all of site features </p>
                        <button class="hidden" id="register"  @click="register">Sign Up</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<style>
.login-body{
  display: flex;
  align-items: center !important;
  justify-content: center !important;
  flex-direction: column;
  height: 100%;
}
</style>