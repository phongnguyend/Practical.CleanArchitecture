<template>
  <section class="card"><div class="card-body">
    <nav aria-label="Breadcrumb" class="mb-3 d-flex flex-wrap gap-2"><router-link to="/admin">Admin</router-link><template v-if="view !== 'admin'"><span>/</span><router-link to="/admin/playgrounds">Playgrounds</router-link></template><template v-if="view !== 'admin' && view !== 'playgrounds'"><span>/</span><span aria-current="page">{{ view === 'mobilepush' ? 'Mobile Push' : view === 'email' ? 'Email' : 'SMS' }}</span></template></nav>
    <template v-if="view === 'admin' || view === 'playgrounds'">
      <h1>{{ view === 'admin' ? 'Admin' : 'Playgrounds' }}</h1>
      <div class="d-flex flex-wrap gap-2"><router-link v-if="view === 'admin'" class="btn btn-outline-primary" to="/admin/playgrounds"><ActionIcon action="settings" />Playgrounds</router-link><template v-else>
        <router-link class="btn btn-outline-primary" to="/admin/playgrounds/mobile-push"><ActionIcon action="notification" />Mobile Push</router-link>
        <router-link class="btn btn-outline-primary" to="/admin/playgrounds/email"><ActionIcon action="notification" />Email</router-link>
        <router-link class="btn btn-outline-primary" to="/admin/playgrounds/sms"><ActionIcon action="notification" />SMS</router-link>
      </template></div>
    </template>
    <NotificationPlaygrounds v-else :key="view" :channel="view" />
  </div></section>
</template>
<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import NotificationPlaygrounds from './NotificationPlaygrounds.vue'
import ActionIcon from '../../components/ActionIcon.vue'
const route = useRoute()
const view = computed(() => route.path.endsWith('mobile-push') ? 'mobilepush' : route.path.endsWith('/email') ? 'email' : route.path.endsWith('/sms') ? 'sms' : route.path.endsWith('playgrounds') ? 'playgrounds' : 'admin')
</script>
