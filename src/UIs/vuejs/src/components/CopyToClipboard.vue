<template>
  <span class="app-copy-control">
    <button type="button" :class="['app-copy-trigger', className]" :title="title" :aria-label="title" @click="handleCopy">
      <Clipboard :size="18" aria-hidden="true" />
    </button>
    <span class="app-copy-feedback" role="status">{{ copyStatus }}</span>
  </span>
</template>

<script setup lang="ts">
import { ref, onUnmounted } from 'vue'
import { Clipboard } from 'lucide-vue-next'

interface Props {
  text: string
  className?: string
  title?: string
}

const props = withDefaults(defineProps<Props>(), {
  className: 'copy-icon',
  title: 'Copy Data',
})

const copyStatus = ref('')
let timeoutId: ReturnType<typeof setTimeout> | null = null

const handleCopy = async () => {
  if (timeoutId) {
    clearTimeout(timeoutId)
  }
  try {
    await navigator.clipboard.writeText(props.text)
    copyStatus.value = 'Copied to clipboard'
  } catch {
    copyStatus.value = 'Unable to copy'
  }
  timeoutId = setTimeout(() => {
    copyStatus.value = ''
  }, 2000)
}

// Cleanup timeout on component unmount
onUnmounted(() => {
  if (timeoutId) {
    clearTimeout(timeoutId)
  }
})
</script>

<style scoped>
.copy-icon {
  cursor: pointer;
}
</style>
