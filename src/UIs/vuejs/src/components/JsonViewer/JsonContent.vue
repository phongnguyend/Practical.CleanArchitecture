<template><div ref="host" /></template>
<script setup lang="ts">
import { ref, watch, onBeforeUnmount } from 'vue'
import { mountJsonViewer } from './jsonRuntime'
const props = defineProps<{ jsonData: unknown }>()
const host = ref<HTMLElement | null>(null)
let cleanup: (() => void) | undefined
watch(
  () => [host.value, props.jsonData],
  () => {
    cleanup?.()
    if (host.value) cleanup = mountJsonViewer(host.value, props.jsonData)
  },
  { immediate: true, flush: 'post' },
)
onBeforeUnmount(() => cleanup?.())
</script>
<style src="./json.css"></style>
