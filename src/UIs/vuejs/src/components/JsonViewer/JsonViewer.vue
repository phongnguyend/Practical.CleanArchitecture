<template>
  <button
    type="button"
    class="app-json-trigger"
    title="View JSON"
    aria-label="View JSON"
    @click="show = true"
  >
    { }
  </button>
  <AppModal v-model="show" title="JSON Data" size="lg">
    <JsonContent :json-data="jsonData" />
  </AppModal>
</template>
<script setup lang="ts">
import { ref } from 'vue'
import AppModal from '../Modal/Modal.vue'
import JsonContent from './JsonContent.vue'
defineProps<{ jsonData: unknown }>()
const show = ref(false)
</script>
