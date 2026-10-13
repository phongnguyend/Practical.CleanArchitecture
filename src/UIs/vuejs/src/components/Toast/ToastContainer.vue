<template><span hidden aria-hidden="true" /></template>
<script setup lang="ts">
import { onMounted, onBeforeUnmount } from 'vue'
import { mountToasts } from './toast'
let cleanup: (() => void) | undefined
onMounted(() => {
  cleanup = mountToasts()
})
onBeforeUnmount(() => cleanup?.())
</script>
<style src="./toast.css"></style>
