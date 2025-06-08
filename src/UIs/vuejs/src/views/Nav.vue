<template>
  <nav
    class="navbar navbar-expand navbar-light bg-light"
    :style="{ paddingLeft: '1rem', paddingRight: '1rem' }"
  >
    <a class="navbar-brand" href="/">{{ appendVersion(pageTitle) }}</a>
    <ul class="nav nav-pills">
      <li>
        <router-link class="nav-link" to="/" active-class="active" exact>Home</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/settings" active-class="active">Settings</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/files" active-class="active">Files</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/products" active-class="active">Products</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/users" active-class="active">Users</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/auditlogs" active-class="active">Audit Logs</router-link>
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

<script setup lang="ts">
import { ref, computed, version } from 'vue'
import { useStore } from 'vuex'

const store = useStore()
const pageTitle = ref('ClassifiedAds.Vue')

const isAuthenticated = computed((): boolean => {
  return store.state.authService.isAuthenticated()
})

const login = () => {
  store.state.authService.login('')
}

const logout = () => {
  store.state.authService.logout()
}

const appendVersion = (value: string) => {
  return value + ' ' + version
}
</script>

<style scoped>
.nav-link {
  font-size: large;
}
</style>
