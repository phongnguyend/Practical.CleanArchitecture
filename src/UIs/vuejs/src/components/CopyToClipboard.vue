<template>
  <span v-if="copyStatus" class="copy-icon">{{ copyStatus }}</span>
  <Clipboard
    v-else
    :class="className"
    :title="title"
    @click="handleCopy"
    role="button"
    tabindex="0"
    @keydown.enter="handleCopy"
    :size="16"
  />
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

const handleCopy = () => {
  navigator.clipboard
    .writeText(props.text)
    .then(() => {
      copyStatus.value = '✅ copied'
    })
    .catch(() => {
      copyStatus.value = '❌ cannot copy'
    })

  timeoutId = setTimeout(() => {
    copyStatus.value = ''
  }, 1000)
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
