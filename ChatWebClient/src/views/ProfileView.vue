<script lang="js">
  import { mapGetters, mapActions, mapState } from 'vuex';
  import { Button, ProgressSpinner, FileUpload, InputText, Message, DataView, Card, ButtonGroup } from 'primevue';
  import UserService from '@/services';
  import ChatService from '@/services/chat';
  import store from '@/stores';
  import { Tab } from 'bootstrap';
  import { Form } from '@primevue/forms';
  import { Toast } from 'bootstrap';
  import { ref } from 'vue';
  import HeartProgress from '@/components/HeartProgress.vue';
  import Chat from '@/components/Chat.vue';
  export default {
    components: {
      Button,
      ProgressSpinner,
      FileUpload,
      Form,
      InputText,
      Message,
      DataView,
      HeartProgress,
      Chat,
      ButtonGroup,
      Card
    },
    data() {
      const src = ref(null);

      const initialValues = ref({
        userID:0,
        username: "",
        fullname: "",
        isFriend: false,
        isSentRequest: false,
        isReceivedRequest: false
      });
      const resolver = ({ values }) => {
        const errors = {};

        if (!values.username) {
          errors.username = [{ message: 'Username is required.' }];
        }

        if (!values.fullname) {
          errors.fullname = [{ message: 'Fullname is required.' }];
        }

        return {
          errors
        };
      };
      return {
        isError: false,
        errorMessage: "",
        isLoading: true,
        src,
        toastMessage: "",
        initialValues,
        resolver,
        isListFriends: true,
        isOwnProfile: false,
        username: "",
        groupchat: "",
        group: null
      };
    },
    computed: {
      ...mapGetters(['getUserId']),
      ...mapGetters('profile', ['getUser', 'getUserImg']),
    },
    methods: {
      ...mapActions(["updateGroupChatCode",'updateGroupChat']),
      ...mapActions('profile', ['setUser']),
      initializeTabs() {
        const triggerTabList = document.querySelectorAll('#myTab .list-group-item button');
        triggerTabList.forEach((triggerEl) => {
          const tabTrigger = new Tab(triggerEl);
          triggerEl.addEventListener('click', (event) => {
            event.preventDefault();
            tabTrigger.show();
          });
        });
      },
      onFileSelect(event) {
        const file = event.files[0];
        const reader = new FileReader();

        reader.onload = async (e) => {
          this.src = e.target.result;
        };

        reader.readAsDataURL(file);
      },
      onFormSubmit({ valid }) {
        
      },
      showToast(message) {
        this.toastMessage = message;

        const toastEl = document.getElementById('liveToast');
        const toast = new Toast(toastEl);
        toast.show();
      },
      toggleListFriend() {
        this.isListFriends = true; 
      },
      toggleListBlocked() {
        this.isListFriends = false; 

      },
      async fetchtUser() {
        if (this.username  && this.username != "") {
          this.isOwnProfile = false;
        } else {
          this.isOwnProfile = true;
        }
        await UserService.postGetUserProfile(this.username).then(res => {
          if (res.status == 200) {
            this.setUser(res.data);
            this.initialValues = {
              userID: res.data.usID,
              username: res.data.username,
              fullname: res.data.fullname,
              isFriend: res.data.isFriend,
              isSentRequest: res.data.isSentRequest,
              isReceivedRequest: res.data.isReceivedRequest
            };
            if (res.data.usID == this.getUserId) {
              this.isOwnProfile = true;
            }
            if (res.data.group != null && res.data.group.grId !=0) {
              ChatService.GetListChats(res.data.group.grId);
              this.updateGroupChatCode(res.data.group.code);
              this.updateGroupChat(res.data.group.grId);
              this.groupchat = res.data.group.name ?? res.data.fullname;
              this.group = res.data.group;
            }
          } else {
            this.isError = true;
            this.errorMessage = "Error! Bad Request";
          }
        }).catch(err => {
          this.isError = true;
          this.errorMessage = err.message;
        }).finally(() => {
          this.isLoading = false;
          this.$nextTick(() => {
            this.initializeTabs(); // Initialize tabs only after DOM updates
          });
        });
      },
      sendRequest(id) {
        if (typeof id == "number") {
          UserService.postAddRequest(id).then(response => {
            if (response.status == 200) {
              if (response.data != null) {
                this.$store.dispatch('sendRequestSocket', { userId: id, requirementId: response.data });
                //cập nhật action
                this.initialValues.isSentRequest = true;
              }
            }
          }).catch(error => console.log(error));
        }
      },
      sendCancelRequest(id) {
        if (typeof id == "number") {
          UserService.sendCancelRequest(id).then(response => {
            if (response.status == 200) {
              //cập nhật action
              this.initialValues.isSentRequest = false;
            }
          }).catch(error => console.log(error));
        }
      },
      handleAccept(userId) {
        UserService.postAddFriendActions(userId, true).then(response => {
          let res = Object.assign({}, response);
          if (res.status == 200 && res.data == 1) {
            this.$store.dispatch("minusNotification");
            //cập nhật action
            this.fetchtUser();
          }
        }).catch(error => console.log(error));
      },
      handleCancel(userId) {
        UserService.postAddFriendActions(userId, true, false).then(response => {
          let res = Object.assign({}, response);
          if (res.status == 200 && res.data == 2) {
            this.$store.dispatch("minusNotification");
            //cập nhật action
            this.initialValues.isFriend = false;
            this.initialValues.isReceivedRequest = false;
          } 
        }).catch(error => console.log(error));
      },
      handleUnfriend(userid) {
        UserService.postUnFriend(userid).then(resp => {
          let res = Object.assign({}, resp);
          if (res.status == 200) {
            //cập nhật action
            this.fetchtUser();
          }
        })
      }
    },
    mounted() {
      const username = this.$route.params.username;
      this.username = username;
      this.fetchtUser();
     
    },
    watch: {
      '$route.params.username': {
        immediate: false, // Gọi ngay lập tức khi component được mount
        handler(newUsername, oldUsername) {
          this.username = newUsername;
          this.fetchtUser();
          const triggerFirstTabEl = document.querySelector('#myTab li:first-child button')
          Tab.getInstance(triggerFirstTabEl).show() 
        }
      }
    }
  };
</script>
<template>
  <div class="container-fluid">
    <div class="row" style="min-height: -webkit-fill-available;">
      <div class="col-12 h-100 flex flex-column align-items-center justify-content-center" v-if="isLoading">
        <ProgressSpinner />
      </div>
      <div class="col-12 h-100 flex flex-column align-items-center justify-content-center mt-5 text-danger" v-if="!isLoading && isError">
        <h1>{{errorMessage}}</h1>
      </div>
      <div class="col-lg-3 col-md-4 col-sm-4 card" v-if="!isLoading && !isError && (this.initialValues.isFriend != 0 || isOwnProfile)">
        <div class="card-header ">
          <div class='flex flex-column align-items-center justify-content-center pt-4'>
            <img :src="'/images/'+getUserImg" class="rounded flex items-center justify-center mr-2 md:w-10rem md:h-10rem sm:w-8rem sm:h-8rem" :alt="getUser.fullname" />
            <p class="mt-3 name-style">
              {{getUser.fullname}}
            </p>
            <div>
              <Button icon="pi pi-times" severity="danger" v-if="this.initialValues.isFriend && !this.initialValues.isSentRequest && !this.initialValues.isReceivedRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3" aria-label="Cancel" variant="text"  label="Unfriend" @click="handleUnfriend(this.initialValues.userID)" />
            </div>
          </div>

        </div>
        <div class="card-body">
          <ul class="list-group list-group-flush" id="myTab" role="tablist" >
            <li class="list-group-item">
              <button class="nav-link active" id="profile-tab" data-bs-toggle="tab" data-bs-target="#profile" type="button" role="tab" aria-controls="profile" aria-selected="true">Account</button>
            </li>
            <li class="list-group-item">
              <button class="nav-link" id="friends-tab" data-bs-toggle="tab" data-bs-target="#friends" type="button" role="tab" aria-controls="friends" aria-selected="false">Friends</button>
            </li>
            <li class="list-group-item">
              <button class="nav-link" id="messages-tab" data-bs-toggle="tab" data-bs-target="#messages" type="button" role="tab" aria-controls="messages" aria-selected="false" v-if="isOwnProfile">Settings</button>
            </li>
            <li class="list-group-item">
              <button class="nav-link" id="settings-tab" data-bs-toggle="tab" data-bs-target="#settings" type="button" role="tab" aria-controls="settings" aria-selected="false" v-if="isOwnProfile">Change Password</button>
            </li>
          </ul>
        </div>
      </div>
      <div class="col-lg-9 col-md-8 col-sm-8 " :class="!isOwnProfile && (!this.initialValues.isFriend || this.group.grId==0)? 'mx-auto':'' "  v-if="!isLoading && !isError">
        <div class="tab-content">
          <div class="tab-pane active" id="profile" role="tabpanel" aria-labelledby="profile-tab" tabindex="0">
            <div class="d-flex flex-column mt-3" v-if="isOwnProfile ">
              <div class="mb-3">
                <div class=" border-1 rounded p-3 border border-secondary-subtle flex flex-column align-items-center justify-content-center"  >
                  <div class="mb-3">
                    <img :src="src ==null? '/images/'+getUserImg : src"  class="rounded flex items-center justify-center mr-2 md:w-14rem md:h-14rem sm:w-12rem sm:h-12rem" id="user_image" :alt="getUser.fullname" style="object-fit:cover" />
                  </div>
                  <div class=" flex flex-row justify-content-center align-items-center flex-grow-1 " >
                    <FileUpload mode="basic" @select="onFileSelect" customUpload auto severity="secondary" class="p-button-outlined" />
                    <Button icon="pi pi-check" severity="success" class="ms-4" v-if="src"  aria-label="Save" label="Save" @click="handleAccept(item.usID)" />
                  </div>
                </div>
              </div>
              <div class="mb-3">
                <div class=" border-1 rounded p-3 border border-secondary-subtle d-flex flex-row">
                  <Form v-slot="$form" :initialValues :resolver :validateOnValueUpdate="false" :validateOnBlur="true" :validateOnMount="['fullName']" @submit="onFormSubmit" class="flex flex-column gap-4 w-full sm:w-56">
                    <div class="grid">
                      <div class="col-2 flex flex-column align-items-center justify-content-center">
                        <label class="form-label mx-auto">Username</label>
                      </div>
                      <div class="col-10 flex flex-column  ">
                        <InputText name="username" type="text" placeholder="Username" fluid />
                        <Message v-if="$form.username?.invalid" severity="error" size="small" variant="simple">{{ $form.username.error.message }}</Message>
                      </div>
                    </div>
                    <div class="grid">
                      <div class="col-2 flex flex-column align-items-center justify-content-center">
                        <label class="form-label mx-auto">Fullname</label>
                      </div>
                      <div class="col-10 flex flex-column ">
                        <InputText name="fullname" type="text" placeholder="Fullname" fluid :formControl="{ validateOnValueUpdate: true }" />
                        <Message v-if="$form.fullname?.invalid" severity="error" size="small" variant="simple">{{ $form.fullname.error.message }}</Message>
                      </div>
                     </div>

                     <Button type="submit" severity="secondary" label="Save" />
                    </Form>
                </div>
              </div>
            </div>
            <div class="flex flex-column align-items-center justify-content-center" v-else>
              <!--<h1>Hien Message voi user</h1>-->
              <Chat :groupName="this.groupchat" v-if="this.initialValues.isFriend" />
              <div class="h-50 card mt-5" style="width: 25rem; overflow: hidden" v-else>
                <img alt="user header" :src="'/images/'+getUserImg" class="card-img-top"  />
                
                <div class="card-body">
                  <div class="card-title name-style text-center">{{getUser.fullname}}</div>
                </div>
                <div class="card-footer">
                  <div class="flex flex-row-reverse align-items-center justify-content-center md:flex-row gap-2 h-25">
                    <ButtonGroup>
                      <Button icon="pi pi-heart" class="flex-auto md:flex-initial whitespace-nowrap p-3"  variant="outlined" severity="help" raised></Button>
                      <Button icon="pi pi-plus" label="Add Friend" v-if="!this.initialValues.isFriend && !this.initialValues.isSentRequest && !this.initialValues.isReceivedRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3" @click="sendRequest(this.initialValues.userID)" raised></Button>

                      <Button icon="pi pi-times" severity="warn" v-if="!this.initialValues.isFriend && this.initialValues.isSentRequest && !this.initialValues.isReceivedRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3" variant="outlined" label="Cancel Request" @click="sendCancelRequest(this.initialValues.userID)" raised />
                      <Button icon="pi pi-check" severity="success" v-if="!this.initialValues.isFriend && this.initialValues.isReceivedRequest &&  !this.initialValues.isSentRequest " class="flex-auto md:flex-initial whitespace-nowrap p-3" aria-label="Accept" label="Accept" @click="handleAccept(this.initialValues.userID)" raised />
                      <Button icon="pi pi-times" severity="danger" v-if="!this.initialValues.isFriend && this.initialValues.isReceivedRequest && !this.initialValues.isSentRequest" class="flex-auto md:flex-initial whitespace-nowrap p-3" aria-label="Cancel" label="Cancel" @click="handleCancel(this.initialValues.userID)" raised />

                    </ButtonGroup>
                  </div>
                </div>
              </div>

            </div>
          </div>
          <div class="tab-pane" id="friends" role="tabpanel" aria-labelledby="friends-tab" tabindex="0">
            <div class="grid h-100 w-full">
              <div :class="isOwnProfile ? 'col-lg-8 col-md-10': 'col-lg-8 col-md-12'">
                <div class="" v-if="getUser.relationships.length==0"></div>
                <div v-else style="overflow-y: auto; max-height: 720px;">
                  <DataView :value="getUser.relationships">
                    <template #list="slotProps">
                      <div class="flex flex-column flex-grow-1">
                        <div class="flex flex-column flex-grow-1">
                          <div v-for="(item, index) in slotProps.items" :key="index">
                            <div class="card my-3" v-if="(isListFriends && item.user.isFriend )|| (!isListFriends && !item.user.isFriend ) || !isOwnProfile">
                              <div class="flex flex-row sm:flex-row sm:items-center p-3 gap-4  card-body">
                                <div class="md:w-40 relative" style="width:150px; height:150px">
                                  <router-link :to="{ name: 'profile', params: { username: item.user.username }}">
                                  <img :src="'/images/'+ item.user.avatar" class="block xl:block mx-auto rounded w-full" :alt="item.user.fullname" style="width: 100%; height: 100%;object-fit: cover " />
                                  </router-link>
                                </div>
                                <div class="flex flex-col md:flex-row justify-content-between md:items-center flex-1 gap-6">
                                  <div >
                                    <div class="text-lg font-medium mt-2">
                                      <router-link :to="{ name: 'profile', params: { username: item.user.username } }" class="text-decoration-none text-black" >
                                        {{ item.user.fullname }}
                                      </router-link>
                                    </div>
                                    <HeartProgress v-if="isOwnProfile" :value="item.counting" />
                                  </div>
                                  <div>

                                  </div>
                                </div>
                              </div>
                             </div>
                          </div>
                        </div>
                      </div>
                    </template>
                </DataView>
                </div>
              </div>
              <div class="col-lg-4 col-md-2"  v-if="isOwnProfile">
                <ul class="list-group list-group-flush" id="myTab2" role="tablist">
                  <li class="list-group-item">
                    <button class="nav-link active" id="listfriend-tab" data-bs-toggle="tab" data-bs-target="#listfriend" type="button" role="tab" aria-controls="listfriend" aria-selected="true" @click="toggleListFriend">Friends</button>
                  </li>
                  <li class="list-group-item">
                    <button class="nav-link" id="listblock-tab" data-bs-toggle="tab" data-bs-target="#listblock" type="button" role="tab" aria-controls="listblock" aria-selected="false" @click="toggleListBlocked" >Blocked</button>
                  </li>
                </ul>
              </div>
            </div>
          </div>
          <div class="tab-pane" id="messages" role="tabpanel" aria-labelledby="messages-tab" tabindex="0">Message</div>
          <div class="tab-pane" id="settings" role="tabpanel" aria-labelledby="settings-tab" tabindex="0">Settings</div>
        </div>

      </div>
    </div>
  </div>
</template>
<style scoped>
  .name-style {
    font-family: "Dosis", serif;
    font-weight: 700;
    font-optical-sizing: auto;
    font-size: 1.5rem;
  }

  .list-group-item .nav-link.active {
    font-weight: bold;
    color: #512da8;
  }

</style>
