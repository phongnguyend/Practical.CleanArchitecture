import { createApp } from 'vue'
import axios from 'axios'
import { createBootstrap } from 'bootstrap-vue-next'
import Toast from 'vue-toastification'
import 'vue-toastification/dist/index.css'
import { VueDatePicker } from '@vuepic/vue-datepicker'

import App from './App.vue'
import router from './router'
import store from './store'
import authService from './auth/authService'
import addAuthInterceptors from './auth/authInterceptors'

import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap-vue-next/dist/bootstrap-vue-next.css'
import 'font-awesome/css/font-awesome.min.css'
import '@vuepic/vue-datepicker/dist/main.css'

addAuthInterceptors(axios)

authService.loadUser().then((user) => {
  store.dispatch('tryAutoLogin', authService)
  if (authService.isAuthenticated()) {
  }
  createApp(App)
    .use(router)
    .use(store)
    .use(createBootstrap())
    .use(Toast)
    .component('VueDatePicker', VueDatePicker)
    .mount('#app')
})
