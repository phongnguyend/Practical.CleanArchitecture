<template>
  <Teleport to="body">
    <div v-if="modelValue" ref="layer" class="app-modal-layer">
      <section
        ref="panel"
        :class="['app-modal-panel', size && `app-modal-panel--${size}`]"
        role="dialog"
        aria-modal="true"
        tabindex="-1"
      >
        <header class="app-modal-header">
          <h2 class="app-modal-title" data-modal-title>
            <slot name="header">{{ title }}</slot>
          </h2>
          <button type="button" class="app-modal-close" aria-label="Close dialog" @click="cancel">
            &#215;
          </button>
        </header>
        <div class="app-modal-body">
          <slot name="body"><slot /></slot>
        </div>
        <footer class="app-modal-footer">
          <slot name="footer" :ok="ok" :cancel="cancel">
            <button type="button" class="btn btn-secondary" @click="cancel">Close</button>
          </slot>
        </footer>
      </section>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { onBeforeUnmount, ref, watch } from 'vue'
import { mountModal } from './modalRuntime'

const props = defineProps<{ modelValue: boolean; title?: string; size?: 'sm' | 'lg' | 'xl' }>()
const emit = defineEmits<{ 'update:modelValue': [value: boolean]; ok: []; cancel: [] }>()
const layer = ref<HTMLElement | null>(null)
const panel = ref<HTMLElement | null>(null)
const cancel = () => {
  emit('cancel')
  emit('update:modelValue', false)
}
const ok = () => {
  emit('ok')
  emit('update:modelValue', false)
}
let cleanup: (() => void) | undefined
watch(
  () => [props.modelValue, layer.value, panel.value],
  () => {
    cleanup?.()
    cleanup = undefined
    if (props.modelValue && layer.value && panel.value)
      cleanup = mountModal(layer.value, panel.value, cancel)
  },
  { flush: 'post', immediate: true },
)
onBeforeUnmount(() => cleanup?.())
</script>

<style src="./modal.css"></style>
