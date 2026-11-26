<template>
  <nav aria-label="Main navigation"
    class="navbar navbar-expand bg-body"
  >
    <div class="nav-header">
      <a class="navbar-brand" href="/">{{ pageTitle }}</a>
      <div class="nav-utilities d-flex align-items-center gap-2 ms-auto flex-shrink-0">
        <a v-if="!isAuthenticated" class="nav-link" @click="login" href="javascript:void(0)"><ActionIcon action="login" />Login</a>
        <a v-if="isAuthenticated" class="nav-link" @click="logout" href="javascript:void(0)"><ActionIcon action="logout" />Logout</a>
        <ThemeSwitcher />
      </div>
    </div>
    <ul class="nav nav-tabs flex-wrap nav-pages">
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
      <li><router-link class="nav-link" :class="{ active: isActive('/admin') }" to="/admin"><ActionIcon action="settings" />Admin</router-link></li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useStore } from 'vuex'
import { useRoute } from 'vue-router'
import ThemeSwitcher from '../components/ThemeSwitcher.vue'

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

</script>

<style scoped>
.navbar {
  display: block;
  padding-inline: 0;
  padding-bottom: 0;
}

.navbar-brand { min-width: 0; margin-right: .75rem; font-size: 1.1rem; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.nav-pages { min-width: 0; width: 100%; flex-wrap: wrap; }
.nav-header { display: flex; align-items: center; justify-content: space-between; gap: .75rem; padding-bottom: .5rem; }
.nav-pages .nav-link { padding-inline: .65rem; font-size: 1rem; white-space: nowrap; }
.nav-utilities .nav-link { font-size: 1rem; white-space: nowrap; }

.nav-link {
  font-size: large;
}

.nav-tabs .nav-link.active {
  color: var(--bs-primary-text-emphasis);
  background-color: var(--bs-primary-bg-subtle);
  border-color: var(--bs-primary-border-subtle);
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
