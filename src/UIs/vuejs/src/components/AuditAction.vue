<script setup lang="ts">
import { computed } from 'vue'
const props = defineProps<{ action?: string | null }>()
const label = computed(() => props.action?.trim() || 'Event')
const kind = computed(() => label.value.toUpperCase().split(/[_\s-]/)[0])

function formatAuditAction(action?: string | null): string {
  return (action?.trim() || "Event").replace(/_/g, " ");
}
</script>

<template>
  <span class="badge" :data-action="kind">{{ formatAuditAction(label) }}</span>
</template>

<style scoped>
/* Keep these semantic colors consistent with the mobile audit badges. */
.badge {
  display: inline-block;
  max-width: 100%;
  padding: 4px 10px;
  border-radius: 6px;
  color: #334155;
  background: #e2e8f0;
  font-size: .85rem;
  font-weight: 700;
  line-height: 1.5;
  white-space: normal;
  overflow-wrap: anywhere;
}
.badge[data-action="DELETE"],
.badge[data-action="DELETED"] {
  color: #b91c1c;
  background: #fee2e2;
}
.badge[data-action="UPDATE"],
.badge[data-action="UPDATED"] {
  color: #9a3412;
  background: #ffedd5;
}
.badge[data-action="ADD"],
.badge[data-action="ADDED"],
.badge[data-action="CREATE"],
.badge[data-action="CREATED"] {
  color: #166534;
  background: #dcfce7;
}
</style>
