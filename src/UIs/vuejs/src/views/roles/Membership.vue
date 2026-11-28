<template>
  <section class="membership-section" :aria-label="roleId ? 'Role users' : 'Assigned roles'">
    <div class="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
      <h2 class="h5 mb-0">{{ roleId ? 'Role users' : 'Assigned roles' }}</h2>
      <button class="btn btn-outline-secondary" :disabled="busy" @click="load"><ActionIcon action="refresh" />{{ roleId ? 'Refresh users' : 'Refresh roles' }}</button>
    </div>
    <p v-if="roleId" class="membership-description">Manage who belongs to this role. Search by username or email, then select a user to assign.</p>
    <p v-if="busy" role="status">Loading...</p>
    <p v-if="error" class="alert alert-danger" role="alert">{{ error }}</p>
    <div v-if="roleId" class="membership-assignment">
      <label for="role-user-search">Search users</label>
      <input id="role-user-search" placeholder="Username or email" v-model="search" class="form-control" @input="selected = ''" />
      <label for="role-user-select">User to assign</label>
      <select id="role-user-select" v-model="selected" class="form-select" :disabled="busy">
        <option value="">Select a user</option>
        <option v-for="user in candidates" :key="user.id" :value="user.id">{{ user.userName }} ({{ user.email }})</option>
      </select>
      <button class="btn btn-primary" :disabled="busy || !selected" @click="assign"><ActionIcon action="add" />Assign user</button>
    </div>
    <p v-if="!busy && !error && items.length === 0">{{ roleId ? 'No users assigned.' : 'No roles assigned.' }}</p>
    <ul class="list-group">
      <li v-for="item in items" :key="item.id" class="list-group-item d-flex flex-wrap align-items-center justify-content-between gap-3">
        <span style="overflow-wrap: anywhere">{{ item.userName || item.name }}<small v-if="item.email" class="d-block">{{ item.email }}</small></span>
        <button v-if="roleId" class="btn btn-outline-danger" :aria-label="`Remove ${item.userName} from role`" :disabled="busy" @click="change(item, false)"><ActionIcon action="delete" />Remove</button>
      </li>
    </ul>
  </section>
</template>
<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import rolesApi from './axios'
import usersApi from '../users/axios'
import { confirmModal } from '../../components/Modal/modalRuntime'
const props = defineProps<{ roleId?: string; userId?: string }>()
type Item = { id: string; name?: string; userName?: string; email?: string }
const items = ref<Item[]>([])
const users = ref<Item[]>([])
const selected = ref('')
const search = ref('')
const busy = ref(false)
const error = ref('')
const candidates = computed(() => users.value.filter(user => !items.value.some(item => item.id === user.id)
  && `${user.userName} ${user.email || ''}`.toLowerCase().includes(search.value.toLowerCase())))
async function load() {
  busy.value = true
  error.value = ''
  try {
    if (props.roleId) {
      const [assigned, available] = await Promise.all([rolesApi.get<Item[]>(`${props.roleId}/users`), usersApi.get<Item[]>('')])
      items.value = assigned.data
      users.value = available.data
    } else {
      items.value = (await usersApi.get<Item[]>(`${props.userId}/roles`)).data
    }
  } catch {
    error.value = 'Unable to load role membership. Please try again.'
  } finally {
    busy.value = false
  }
}
async function assign() {
  const user = candidates.value.find(item => item.id === selected.value)
  if (user) {
    await change(user, true)
  }
}
async function change(user: Item, assign: boolean) {
  if (busy.value) {
    return
  }
  if (!assign && !await confirmModal(`Remove ${user.userName} from this role?`, { title: 'Remove assigned role', confirmText: 'Remove' })) {
    return
  }
  busy.value = true
  error.value = ''
  try {
    const path = `${props.roleId}/users/${user.id}`
    if (assign) {
      await rolesApi.put(path)
    } else {
      await rolesApi.delete(path)
    }
    selected.value = ''
    await load()
  } catch {
    error.value = 'Unable to update role membership. Refresh and try again.'
  } finally {
    busy.value = false
  }
}
watch(() => [props.roleId, props.userId], load, { immediate: true })
</script>

<style scoped>

.membership-section { margin-top: 1.5rem; padding: 1.25rem 0 0; border-top: 1px solid var(--ui-border, var(--bs-border-color, #dce3ed)); }
.membership-section h2 { font-weight: 650; }
.membership-section .membership-description { color: var(--ui-muted, var(--bs-secondary-color, #64748b)); margin: 0 0 1rem; font-size: .9rem; }
.membership-assignment { display: grid; grid-template-columns: minmax(0, 1fr) minmax(0, 1fr) auto; grid-template-areas: "search-label select-label ." "search select assign"; gap: .5rem 1rem; align-items: end; margin-bottom: 1.25rem; padding: 0; }
.membership-assignment > label:first-child { grid-area: search-label; }
.membership-assignment > input { grid-area: search; }
.membership-assignment > label { grid-area: select-label; margin: 0; font-size: .875rem; font-weight: 600; }
.membership-assignment > select { grid-area: select; }
.membership-assignment > button { grid-area: assign; white-space: nowrap; }
.membership-assignment input, .membership-assignment select, .membership-assignment button { min-height: 42px; min-width: 0; }
.membership-section .list-group { padding-left: 0; list-style: none; margin-bottom: 0; }
.membership-section .list-group-item { padding: 1rem 0; gap: 1rem; border: 0; border-bottom: 1px solid var(--ui-border, var(--bs-border-color, #dce3ed)); border-radius: 0; background: transparent; }
.membership-section .list-group-item:last-child { border-bottom: 0; padding-bottom: 0; }
.membership-section small { color: var(--ui-muted, var(--bs-secondary-color, #64748b)); margin-top: .25rem; }
.membership-section .btn { display: inline-flex; align-items: center; justify-content: center; gap: .4rem; }
.membership-section .assigned-role { padding: .65rem 1rem; border: 1px solid var(--bs-border-color, #dce3ed); border-radius: 8px; }
.membership-section .assigned-roles { display: flex; flex-wrap: wrap; gap: .65rem; padding: 0; list-style: none; margin: 0; }
@media (max-width: 700px) {
  .membership-section { padding-top: 1rem; }
  .membership-assignment { grid-template-columns: minmax(0, 1fr); grid-template-areas: "search-label" "search" "select-label" "select" "assign"; gap: .65rem; }
  .membership-assignment > button { margin-top: .35rem; width: 100%; }
}
</style>
