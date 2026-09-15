<template>
  <nav
    class="navbar navbar-expand navbar-light bg-light"
    :style="{ paddingLeft: '1rem', paddingRight: '1rem' }"
  >
    <a class="navbar-brand" href="/"><ActionIcon action="home" />{{ appendVersion(pageTitle) }}</a>
    <ul class="nav nav-pills">
      <li>
        <router-link class="nav-link" to="/" active-class="active" exact><ActionIcon action="home" />Home</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/settings" active-class="active"><ActionIcon action="settings" />Settings</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/files" active-class="active"><ActionIcon action="files" />Files</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/products" active-class="active"><ActionIcon action="products" />Products</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/users" active-class="active"><ActionIcon action="users" />Users</router-link>
      </li>
      <li>
        <router-link class="nav-link" to="/auditlogs" active-class="active"><ActionIcon action="audit" />Audit Logs</router-link>
      </li>
      <li v-if="!isAuthenticated">
        <a class="nav-link" @click="login" href="javascript:void(0)"><ActionIcon action="login" />Login</a>
      </li>
      <li v-if="isAuthenticated">
        <a class="nav-link" @click="logout" href="javascript:void(0)"><ActionIcon action="logout" />Logout</a>
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
