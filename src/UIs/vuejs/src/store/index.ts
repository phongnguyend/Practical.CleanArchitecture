import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    authService: {}
  },
  mutations: {
    setAuthService(state, authService) {
      state.authService = authService;
    }
  },
  actions: {
    tryAutoLogin({ commit }, authService) {
      commit('setAuthService', authService)
    }
  },
  modules: {
  }
})
