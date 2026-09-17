<template>
  <details ref="menu" class="theme-switcher">
    <summary class="theme-switcher-trigger" :title="`Theme: ${theme}`" :aria-label="`Theme: ${theme}. Choose appearance`">
      <Monitor v-if="theme === 'system'" :size="20" aria-hidden="true" />
      <Sun v-else-if="theme === 'light'" :size="20" aria-hidden="true" />
      <Moon v-else :size="20" aria-hidden="true" />
    </summary>
    <div class="theme-switcher-menu" role="group" aria-label="Appearance">
      <button v-for="choice in choices" :key="choice.value" type="button" class="theme-switcher-option" :aria-pressed="theme === choice.value" @click="choose(choice.value)">
        <Monitor v-if="choice.value === 'system'" :size="17" aria-hidden="true" />
        <Sun v-else-if="choice.value === 'light'" :size="17" aria-hidden="true" />
        <Moon v-else :size="17" aria-hidden="true" />
        <span>{{ choice.label }}</span>
        <Check v-if="theme === choice.value" :size="16" class="theme-switcher-check" aria-hidden="true" />
      </button>
    </div>
  </details>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import { Check, Monitor, Moon, Sun } from 'lucide-vue-next'

type Theme = 'system' | 'light' | 'dark'
const storageKey = 'classifiedads-theme'
const choices: { value: Theme; label: string }[] = [
  { value: 'system', label: 'System' },
  { value: 'light', label: 'Light' },
  { value: 'dark', label: 'Dark' },
]
const theme = ref<Theme>('system')
const menu = ref<HTMLDetailsElement | null>(null)
const media = window.matchMedia('(prefers-color-scheme: dark)')

const readTheme = (): Theme => {
  try {
    const stored = localStorage.getItem(storageKey)
    return stored === 'light' || stored === 'dark' ? stored : 'system'
  } catch {
    return 'system'
  }
}

const applyTheme = (choice: Theme) => {
  document.documentElement.setAttribute('data-bs-theme', choice === 'dark' || (choice === 'system' && media.matches) ? 'dark' : 'light')
}

const choose = (choice: Theme) => {
  try { localStorage.setItem(storageKey, choice) } catch { /* Apply for this page. */ }
  theme.value = choice
  applyTheme(choice)
  menu.value?.removeAttribute('open')
}

const onSystemChange = () => { if (readTheme() === 'system') applyTheme('system') }
const onOutside = (event: PointerEvent) => {
  if (menu.value && !menu.value.contains(event.target as Node)) menu.value.removeAttribute('open')
}
const onStorage = (event: StorageEvent) => {
  if (event.key === storageKey) {
    theme.value = readTheme()
    applyTheme(theme.value)
  }
}

onMounted(() => {
  theme.value = readTheme()
  applyTheme(theme.value)
  media.addEventListener('change', onSystemChange)
  document.addEventListener('pointerdown', onOutside)
  window.addEventListener('storage', onStorage)
})
onUnmounted(() => {
  media.removeEventListener('change', onSystemChange)
  document.removeEventListener('pointerdown', onOutside)
  window.removeEventListener('storage', onStorage)
})
</script>

<style scoped>
.theme-switcher { position: relative; }
.theme-switcher-trigger { display: inline-flex; align-items: center; justify-content: center; width: 2.5rem; height: 2.5rem; border: 1px solid var(--bs-border-color); border-radius: .5rem; color: var(--bs-body-color); background: var(--bs-body-bg); cursor: pointer; list-style: none; }
.theme-switcher-trigger::-webkit-details-marker { display: none; }
.theme-switcher-trigger:hover, .theme-switcher[open] .theme-switcher-trigger { color: var(--bs-primary-text-emphasis); background: var(--bs-primary-bg-subtle); border-color: var(--bs-primary-border-subtle); }
.theme-switcher-trigger:focus-visible { outline: 2px solid var(--bs-primary); outline-offset: 2px; }
.theme-switcher-menu { position: absolute; z-index: 1050; top: calc(100% + .375rem); right: 0; min-width: 10rem; padding: .35rem; border: 1px solid var(--bs-border-color); border-radius: .5rem; background: var(--bs-body-bg); box-shadow: 0 .5rem 1rem rgba(0, 0, 0, .16); }
.theme-switcher-option { display: flex; align-items: center; gap: .65rem; width: 100%; padding: .5rem .65rem; border: 0; border-radius: .35rem; color: var(--bs-body-color); background: transparent; text-align: left; cursor: pointer; }
.theme-switcher-option:hover, .theme-switcher-option:focus-visible { background: var(--bs-tertiary-bg); }
.theme-switcher-option[aria-pressed="true"] { color: var(--bs-primary-text-emphasis); font-weight: 600; }
.theme-switcher-check { margin-left: auto; }
</style>
