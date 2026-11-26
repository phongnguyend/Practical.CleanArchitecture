<template>
  <section class="card"><div class="card-body">
    <nav aria-label="Admin sections" class="nav nav-tabs mb-3">
      <router-link to="/admin/playgrounds" class="nav-link" :class="{ active: view !== 'admin' }" :aria-current="view !== 'admin' ? 'page' : undefined"><ActionIcon action="settings" />Playgrounds</router-link>
    </nav>
    <nav v-if="view !== 'admin'" aria-label="Playground channels" class="nav nav-tabs mb-3 flex-wrap">
      <router-link v-for="tab in channels" :key="tab.value" :to="'/admin/playgrounds/' + tab.path" class="nav-link" :class="{ active: view === tab.value }" :aria-current="view === tab.value ? 'page' : undefined"><ActionIcon action="notification" />{{ tab.label }}</router-link>
    </nav>
    <template v-if="view === 'admin' || view === 'playgrounds'">
      <h1>{{ view === 'admin' ? 'Admin' : 'Playgrounds' }}</h1>
      <p>Choose a playground to inspect its queue and compose a message.</p>
    </template>
    <NotificationPlaygrounds v-else :key="view" :channel="view" />
  </div></section>
</template>
<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import NotificationPlaygrounds from './NotificationPlaygrounds.vue'
import ActionIcon from '../../components/ActionIcon.vue'
const channels = [{ value: 'mobilepush', path: 'mobile-push', label: 'Mobile Push' }, { value: 'email', path: 'email', label: 'Email' }, { value: 'sms', path: 'sms', label: 'SMS' }]
const route = useRoute()
const view = computed(() => route.path.endsWith('mobile-push') ? 'mobilepush' : route.path.endsWith('/email') ? 'email' : route.path.endsWith('/sms') ? 'sms' : route.path.endsWith('playgrounds') ? 'playgrounds' : 'admin')
</script>
