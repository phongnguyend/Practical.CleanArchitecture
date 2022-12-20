import { createStore } from "vuex";

export default createStore({
  state: {
    authService: {},
  },
  mutations: {
    setAuthService(state, authService) {
      state.authService = authService;
    },
  },
  actions: {
    tryAutoLogin({ commit }, authService) {
      commit("setAuthService", authService);
    },
  },
  modules: {},
});
