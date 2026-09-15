<template>
  <component :is="icon" :size="16" class="me-1" aria-hidden="true" />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  Check, ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight, ClipboardList,
  Download, ExternalLink, Eye, EyeOff, FileDown, FileUp, Files, History, House,
  KeyRound, LogIn, LogOut, Mail, Package, Pencil, Plus, Save, Settings, Trash2,
  Upload, Users, X,
} from 'lucide-vue-next'

const icons = {
  add: Plus, audit: ClipboardList, back: ChevronLeft, cancel: X, close: X,
  confirm: Check, delete: Trash2, download: Download, edit: Pencil,
  export: FileDown, external: ExternalLink, files: Files, first: ChevronsLeft,
  history: History, home: House, import: FileUp, key: KeyRound,
  last: ChevronsRight, login: LogIn, logout: LogOut, mail: Mail,
  next: ChevronRight, previous: ChevronLeft, products: Package, save: Save,
  settings: Settings, show: Eye, hide: EyeOff, upload: Upload, users: Users,
  view: Eye,
}

const props = defineProps<{ action: keyof typeof icons }>()
const icon = computed(() => icons[props.action])
</script>
