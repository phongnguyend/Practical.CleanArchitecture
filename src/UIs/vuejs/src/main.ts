import Vue from "vue";
import axios from "axios";
import Vuelidate from "vuelidate";
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

Vue.config.productionTip = false;

Vue.filter("appendVersion", function(value: string) {
  return value + " " + Vue.version;
});
Vue.filter("appendCurrentDateTime", function(value: string) {
  return value + " " + new Date();
});
Vue.filter("uppercase", function(value: string) {
  return value?.toUpperCase();
});

Vue.use(Vuelidate);
Vue.use(BootstrapVue);
Vue.use(VueToastr);

addAuthInterceptors(axios);

authService.loadUser().then(user => {
  store.dispatch("tryAutoLogin", authService);
  if (authService.isAuthenticated()) {
  }
  new Vue({
    router,
    store,
    render: h => h(App)
  }).$mount("#app");
});
