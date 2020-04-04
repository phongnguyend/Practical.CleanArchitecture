import Vue from 'vue'
import axios from 'axios'
import Vuelidate from 'vuelidate'
import { BootstrapVue } from 'bootstrap-vue'

import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import store from './store'
import authService from "./auth/authService";
import addAuthInterceptors from "./auth/authInterceptors";

import "bootstrap/dist/css/bootstrap.min.css";
import "font-awesome/css/font-awesome.min.css";
import 'bootstrap-vue/dist/bootstrap-vue.css'

Vue.config.productionTip = false;

Vue.filter("appendVersion", function (value) {
  return value + " " + Vue.version;
});
Vue.filter("appendCurrentDateTime", function (value) {
  return value + " " + new Date();
});
Vue.filter("uppercase", function (value) {
  return value?.toUpperCase();
});

Vue.use(Vuelidate)
Vue.use(BootstrapVue)

addAuthInterceptors(axios);

authService.loadUser().then(user => {
  store.dispatch("tryAutoLogin", authService);
  if (authService.isAuthenticated()) {

  }
  new Vue({
    router,
    store,
    render: h => h(App)
  }).$mount('#app')
});
