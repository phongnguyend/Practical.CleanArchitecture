<template>
  <nav class="navbar navbar-expand navbar-light bg-light">
    <a class="navbar-brand" href="/">{{ pageTitle | appendVersion }}</a>
    <ul class="nav nav-pills">
      <li>
        <router-link class="nav-link" to="/" active-class="active" exact
          >Home</router-link
        >
      </li>
      <li>
        <router-link class="nav-link" to="/settings" active-class="active"
          >Settings</router-link
        >
      </li>
      <li>
        <router-link class="nav-link" to="/files" active-class="active"
          >Files</router-link
        >
      </li>
      <li>
        <router-link class="nav-link" to="/products" active-class="active"
          >Products</router-link
        >
      </li>
      <li>
        <router-link class="nav-link" to="/users" active-class="active"
          >Users</router-link
        >
      </li>
      <li>
        <router-link class="nav-link" to="/auditlogs" active-class="active"
          >Audit Logs</router-link
        >
      </li>
      <li v-if="!isAuthenticated">
        <a class="nav-link" @click="login" href="javascript:void(0)">Login</a>
      </li>
      <li v-if="isAuthenticated">
        <a class="nav-link" @click="logout" href="javascript:void(0)">Logout</a>
      </li>
    </ul>
  </nav>
</template>

<script lang="ts">
import Vue from "vue";
export default Vue.extend({
  data() {
    return { pageTitle: "ClassifiedAds.Vue" };
  },
  computed: {
    isAuthenticated(): boolean {
      return this.$store.state.authService.isAuthenticated();
    }
  },
  methods: {
    login() {
      this.$store.state.authService.login("");
    },
    logout() {
      this.$store.state.authService.logout();
    }
  }
});
</script>

<style scoped>
.nav-link {
  font-size: large;
}
</style>
