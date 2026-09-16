<template>
  <div>
    <label class="file-picker" :class="{ 'file-picker-invalid': invalid }">
      <input :id="id" :name="name" type="file" :accept="accept" :title="file?.name" :aria-label="label" :aria-describedby="`${id}-hint`" :aria-invalid="invalid" @change="emit('change', ($event.target as HTMLInputElement).files)" />
      <span class="file-picker-icon"><FileUp :size="22" aria-hidden="true" /></span>
      <span class="file-picker-copy">
        <strong class="file-picker-title">{{ file ? file.name : 'Choose a file' }}</strong>
        <span :id="`${id}-hint`" class="file-picker-hint">{{ hint }}</span>
      </span>
      <span class="file-picker-button">Browse</span>
    </label>
    <div v-if="invalid" class="text-danger small mt-1">Select a file</div>
  </div>
</template>

<script setup lang="ts">
import { FileUp } from 'lucide-vue-next'

defineProps<{
  id: string
  name: string
  label: string
  hint: string
  file: File | null | undefined
  invalid?: boolean
  accept?: string
}>()

const emit = defineEmits<{ change: [files: FileList | null] }>()
</script>

<style scoped>
.file-picker { position: relative; display: flex; align-items: center; gap: .875rem; min-height: 5.25rem; padding: .875rem 1rem; border: 2px dashed #a8bedb; border-radius: .75rem; background: #f7faff; color: #1e3352; cursor: pointer; transition: border-color .15s, background-color .15s; }
.file-picker:hover, .file-picker:focus-within { border-color: #0d6efd; background: #eef5ff; }
.file-picker:focus-within { outline: 2px solid #80bdff; outline-offset: 2px; }
.file-picker-invalid { border-color: #dc3545; }
.file-picker input[type="file"] { position: absolute; z-index: 1; inset: 0; width: 100%; height: 100%; opacity: 0; cursor: pointer; }
.file-picker-icon { display: inline-flex; flex: 0 0 auto; padding: .65rem; border-radius: .65rem; background: #e8f1ff; color: #0d6efd; }
.file-picker-copy { display: flex; flex: 1 1 auto; flex-direction: column; min-width: 0; line-height: 1.35; }
.file-picker-title { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.file-picker-hint { color: #61728a; font-size: .875rem; }
.file-picker-button { flex: 0 0 auto; border: 1px solid #b6c9e5; border-radius: .4rem; background: #fff; color: #0d6efd; font-weight: 600; padding: .4rem .7rem; }
@media (max-width: 450px) { .file-picker { flex-wrap: wrap; } .file-picker-button { margin-left: auto; } }
</style>
