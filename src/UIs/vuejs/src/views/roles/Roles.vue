<template>
  <div class="card">
    <div class="card-header">
      <ActionIcon action="roles" />{{ mode === 'list' ? 'Roles' : 'Role Details' }}
      <button v-if="mode === 'list'" class="btn btn-primary float-end" type="button" @click="openAdd"><ActionIcon action="add" />Add Role</button>
    </div>
    <div class="card-body">
      <div v-if="error && !modalAdd && !modalEdit" class="alert alert-danger" role="alert">{{ error }}</div>
      <div v-if="mode === 'list'" class="table-responsive">
        <table class="table">
          <thead><tr><th>Name</th><th>Actions</th></tr></thead>
          <tbody><tr v-for="item in roles" :key="item.id">
            <td><router-link :to="'/roles/' + item.id"><ActionIcon action="view" />{{ item.name }}</router-link></td>
            <td><button class="btn btn-primary" type="button" @click="openEdit(item)"><ActionIcon action="edit" />Edit</button>
              <button class="btn btn-danger ms-1" type="button" @click="remove(item)"><ActionIcon action="delete" />Delete</button></td>
          </tr></tbody>
        </table>
      </div>
      <div v-if="mode === 'view'">
        <dl><dt>Name</dt><dd>{{ role.name }}</dd><dt>Normalized Name</dt><dd>{{ role.normalizedName }}</dd></dl>
        <button class="btn btn-primary" type="button" :disabled="!role.id" @click="openEdit(role)"><ActionIcon action="edit" />Edit</button>
      </div>
    </div>
    <div v-if="mode !== 'list'" class="card-footer"><router-link class="btn btn-outline-secondary" to="/roles"><ActionIcon action="back" />Back</router-link></div>
    <b-modal v-model="modalAdd" title="Add Role" no-footer>
      <form @submit.prevent="add">
        <div v-if="error" class="alert alert-danger" role="alert">{{ error }}</div>
        <div class="mb-3"><label class="form-label" for="newRoleName">Name</label>
          <input id="newRoleName" v-model="newName" class="form-control" required /></div>
        <div class="d-flex justify-content-end gap-2">
          <button class="btn btn-secondary" type="button" @click="modalAdd = false"><ActionIcon action="cancel" />Cancel</button>
          <button class="btn btn-primary" type="submit" :disabled="saving"><ActionIcon action="save" />{{ saving ? 'Saving...' : 'Save' }}</button>
        </div>
      </form>
    </b-modal>
    <b-modal v-model="modalEdit" title="Edit Role" no-footer>
      <form @submit.prevent="saveEdit">
        <div v-if="error" class="alert alert-danger" role="alert">{{ error }}</div>
        <div class="mb-3"><label class="form-label" for="editRoleName">Name</label>
          <input id="editRoleName" v-model="editName" class="form-control" required /></div>
        <div class="d-flex justify-content-end gap-2">
          <button class="btn btn-secondary" type="button" @click="modalEdit = false"><ActionIcon action="cancel" />Cancel</button>
          <button class="btn btn-primary" type="submit" :disabled="saving"><ActionIcon action="save" />{{ saving ? 'Saving...' : 'Save' }}</button>
        </div>
      </form>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import axios from './axios'

interface Role { id: string; name: string; normalizedName?: string }
const route = useRoute()
const id = computed(() => route.params.id as string | undefined)
const mode = computed(() => route.path === '/roles' ? 'list' : 'view')
const roles = ref<Role[]>([])
const role = ref<Role>({ id: '', name: '' })
const error = ref('')
const saving = ref(false)
const modalAdd = ref(false)
const newName = ref('')
const modalEdit = ref(false)
const editingRole = ref<Role | null>(null)
const editName = ref('')

const openEdit = (item: Role) => {
  error.value = ''
  editingRole.value = item
  editName.value = item.name
  modalEdit.value = true
}

const openAdd = () => {
  error.value = ''
  newName.value = ''
  modalAdd.value = true
}

const add = async () => {
  if (!newName.value.trim()) { error.value = 'Enter a role name.'; return }
  saving.value = true
  error.value = ''
  try {
    await axios.post<Role>('', { name: newName.value.trim() })
    modalAdd.value = false
    newName.value = ''
    await load()
  } catch { error.value = 'Unable to add the role. Please try again.' }
  finally { saving.value = false }
}

const load = async () => {
  error.value = ''
  role.value = { id: '', name: '' }
  try {
    if (mode.value === 'list') roles.value = (await axios.get<Role[]>('')).data
    else if (id.value) role.value = (await axios.get<Role>(id.value)).data
  } catch { error.value = 'Unable to load roles. Please try again.' }
}

const saveEdit = async () => {
  if (!editingRole.value || !editName.value.trim()) { error.value = 'Enter a role name.'; return }
  saving.value = true
  error.value = ''
  try {
    await axios.put<Role>(editingRole.value.id, { ...editingRole.value, name: editName.value.trim() })
    modalEdit.value = false
    editingRole.value = null
    await load()
  } catch { error.value = 'Unable to save the role. Please try again.' }
  finally { saving.value = false }
}

const remove = async (item: Role) => {
  if (!window.confirm(`Delete role ${item.name}?`)) return
  try { await axios.delete(item.id); await load() }
  catch { error.value = 'Unable to delete the role. Please try again.' }
}

watch(() => route.fullPath, () => { void load() }, { immediate: true })
</script>
