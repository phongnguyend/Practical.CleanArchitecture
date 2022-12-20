import { createApp } from "vue";
import axios from "axios";
import Vuelidate from "@vuelidate/core";
import { BootstrapVue } from "bootstrap-vue";
import VueToastr from "vue-toastr";

import App from "./App.vue";
import "./registerServiceWorker";
import router from "./router";
import store from "./store";
import authService from "./auth/authService";
import addAuthInterceptors from "./auth/authInterceptors";

import "bootstrap/dist/css/bootstrap.min.css";
import "font-awesome/css/font-awesome.min.css";
import "bootstrap-vue/dist/bootstrap-vue.css";

addAuthInterceptors(axios);

authService.loadUser().then((user) => {
  store.dispatch("tryAutoLogin", authService);
  if (authService.isAuthenticated()) {
  }
  createApp(App)
    .use(router)
    .use(store)
    .use(Vuelidate)
    .use(BootstrapVue)
    .use(VueToastr)
    .mount("#app");
});
