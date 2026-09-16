<template>
  <nav
    class="navbar navbar-expand navbar-light bg-light"
    :style="{ paddingLeft: '1rem', paddingRight: '1rem' }"
  >
    <a class="navbar-brand" href="/"><ActionIcon action="home" />{{ appendVersion(pageTitle) }}</a>
    <ul class="nav nav-tabs flex-grow-1">
      <li>
        <router-link class="nav-link" :class="{ active: isActive('/') }" to="/"><ActionIcon action="home" />Home</router-link>
      </li>
      <li>
        <router-link class="nav-link" :class="{ active: isActive('/settings') }" to="/settings"><ActionIcon action="settings" />Settings</router-link>
      </li>
      <li>
        <router-link class="nav-link" :class="{ active: isActive('/files') }" to="/files"><ActionIcon action="files" />Files</router-link>
      </li>
      <li>
        <router-link class="nav-link" :class="{ active: isActive('/products') }" to="/products"><ActionIcon action="products" />Products</router-link>
      </li>
      <li>
        <router-link class="nav-link" :class="{ active: isActive('/users') }" to="/users"><ActionIcon action="users" />Users</router-link>
      </li>
      <li><router-link class="nav-link" :class="{ active: isActive('/roles') }" to="/roles"><ActionIcon action="roles" />Roles</router-link></li>
      <li>
        <router-link class="nav-link" :class="{ active: isActive('/auditlogs') }" to="/auditlogs"><ActionIcon action="audit" />Audit Logs</router-link>
      </li>
      <li v-if="!isAuthenticated" class="ms-auto">
        <a class="nav-link" @click="login" href="javascript:void(0)"><ActionIcon action="login" />Login</a>
      </li>
      <li v-if="isAuthenticated" class="ms-auto">
        <a class="nav-link" @click="logout" href="javascript:void(0)"><ActionIcon action="logout" />Logout</a>
      </li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
import { ref, computed, version } from 'vue'
import { useStore } from 'vuex'
import { useRoute } from 'vue-router'

const store = useStore()
const route = useRoute()
const pageTitle = ref('ClassifiedAds.Vue')

const isActive = (path: string) => path === '/'
  ? route.path === '/'
  : route.path === path || route.path.startsWith(path + '/')

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
.navbar {
  padding-bottom: 0;
}

.nav-link {
  font-size: large;
}

.nav-tabs .nav-link.active {
  color: #084298;
  background-color: #f0f6ff;
  border-color: #cfe2ff;
  box-shadow: inset 0 -3px 0 #0d6efd;
  font-weight: 700;
}

.nav-tabs .nav-link.active :deep(svg) {
  stroke-width: 2.5;
}

.navbar-brand :deep(svg),
.nav-link :deep(svg) {
  transform: translateY(-2px);
}
</style>
