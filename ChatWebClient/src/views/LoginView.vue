<script lang="js">
import axios from 'axios';
import AuthService from '@/services/auth';
import FloatLabel from 'primevue/floatlabel';
import InputText from 'primevue/inputtext';
import Password from 'primevue/password';
import { ref } from 'vue';
import '../assets/login.css';
import { mapActions, mapState, mapGetters } from 'vuex';
import { Toast } from 'bootstrap';
export default {
    components:{
        FloatLabel,
        InputText,
        Password
    },
    computed: {
      ...mapState('login', ['username','password','fullname']),
      ...mapGetters('login', ['getActiveBtn', 'getError'])
    },
    data() {
      return {
        toastMessage: '', // Nội dung thông báo
      };
    },
    mounted() {
        const container = document.getElementById('container');
        const registerBtn = document.getElementById('register');
        const loginBtn = document.getElementById('login');

        registerBtn.addEventListener('click', () => {
            container.classList.add("active");
        });

        loginBtn.addEventListener('click', () => {
            container.classList.remove("active");
        });

    },
    methods: {
        ...mapActions('login', ['toggleActive','setError','setUsername','setPassword','setFullname']),
    async onLogin() {
        await AuthService.login({
            username: this.username,
            password: this.password,
        }).then(response => {
            const token = response.data.token;
            const user = { id: response.data.userId, username: this.username }; 
            this.$store.dispatch("login", { token, user });
            this.setError(false);
            this.showToast("");
            this.$router.push( "/").then( x => window.location.reload());
            
        }).catch(err => {
            if(err.response.status == 401){
                this.setError(true);
                this.showToast("Username or password incorrect");
            }else{
                this.setError(true);
                this.showToast("Can not connect to server now");
            }
        });
    },
      async onRegister() {
        if (!this.validateUsername(this.username)) {
          this.showToast("Username need at least 3 characters");
          return; 
        }
        if (!this.validatePassword(this.password)) {
          this.showToast('Password must be at least 6 characters long and contain at least 1 number');
          return;
        }
        if (this.fullname.trim()) {
          this.showToast("Fullname cannot be emty.");
          return; 
        }
        await AuthService.register({
            username: this.username,
            password: this.password,
            fullname: this.fullname
        }).then(response => {
            const token = response.data.token;
            const user = { id: response.data.userId, username: this.username }; 
            this.$store.dispatch("login", { token, user });
            this.setError(false);
            this.setMessage("");
            this.$router.push( "/").then( x => window.location.reload());
            
        }).catch(err => {
            if(err.response.status == 400){
                this.setError(true);
                this.showToast("Can not register. Choose another username");
              
            }else{
                this.setError(true);
                this.showToast("Can not connect to server now");
            }
        });
    },
    showToast(message) {
      this.toastMessage = message;

      const toastEl = document.getElementById('liveToast');
      const toast = new Toast(toastEl);
      toast.show();
    },
    validateUsername(username) {
      // Kiểm tra username: Ít nhất 3 ký tự
      return username && username.length >= 3;
    },
    validatePassword(password) {
      // Kiểm tra mật khẩu: Ít nhất 6 ký tự và chứa ít nhất một số
      const passwordRegex = /^(?=.*\d).{6,}$/;
      return passwordRegex.test(password);
    },
  },
}
</script>

<template>
  <div class="toast-container position-fixed top-0 start-50 translate-middle-x p-3">
    <div id="liveToast"
         class="toast  text-bg-danger "
         role="alert"
         aria-live="assertive"
         aria-atomic="true">
      <div class="d-flex">
        <div class="toast-body">
          {{ toastMessage }}
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
      </div>
    </div>
  </div>
  <div class="login-body">
    <div class="container" id="container" :class="{ active: getActiveBtn}">
        <div class="form-container sign-up">
          <form @submit.prevent="onRegister">
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
            <div class="mt-5">
              <FloatLabel  class="mb-3" variant="on">
                <InputText id="username2" @input="setUsername" :value="username" />
                <label for="username2">Username</label>
              </FloatLabel>
              <FloatLabel class="mb-3" variant="on">
                <InputText id="fullname" @input="setFullname" :value="fullname" />
                <label for="fullname">Fullname</label>
              </FloatLabel>
              <FloatLabel  class="mb-3" variant="on">
                <Password id="password2" @input="setPassword" :value="password" toggleMask />
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
            <div class="mt-5">
              <FloatLabel class="mb-3" variant="on">
                <InputText id="username" @input="setUsername" />
                <label for="username">Username</label>
              </FloatLabel>
              <FloatLabel class="mb-1" variant="on">
                <Password id="password" @input="setPassword" toggleMask  />
                <label for="password">Password</label>
              </FloatLabel>
            </div>
            <a class="#">Forget Your Password</a>
            <button>Sign In</button>
          </form>

        </div>
        <div class="toggle-container">
          <div class="toggle">
            <div class="toggle-panel toggle-left">
              <h1 style="color: #FFF;">Welcome Back!</h1>
              <p>Enter your personal details to use all of site features </p>
              <button :class="!getActiveBtn? 'hidden': ''" id="login" @click="toggleActive">Sign In</button>
            </div>
            <div class="toggle-panel toggle-right">
              <h1 style="color: #FFF;">Hello, Friend!</h1>
              <p>Register with your details to use all of site features </p>
              <button :class="getActiveBtn? 'hidden':'' " id="register" @click="toggleActive">Sign Up</button>
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
  input#username,
  input#username2,
  input#fullname,
  input#password2,
  input#password {
    width: 200px;
  }
</style>
