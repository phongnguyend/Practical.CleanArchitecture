<template>
  <div class="card">
    <div class="card-header">
      Settings
      <div style="float: right">
        <button type="button" class="btn btn-secondary" @click="exportAsExcel()">
          Export as Excel
        </button>
        &nbsp;
        <button class="btn btn-primary" @click="addEntry()">Add</button>
        &nbsp;
        <button class="btn btn-primary" @click="openImportExcelModal()">Import Excel</button>
      </div>
    </div>
    <div class="card-body">
      <div class="table-responsive" :style="{ width: '100%' }">
        <table class="table" v-if="configurationEntries && configurationEntries.length">
          <thead>
            <tr>
              <th>Key</th>
              <th>Value</th>
              <th>Description</th>
              <th>Updated Time</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in configurationEntries" :key="entry.id">
              <td>{{ entry.key }}</td>
              <td>{{ entry.isSensitive ? '******' : entry.value }}</td>
              <td>{{ entry.description }}</td>
              <td>{{ formatedDateTime(entry.updatedDateTime || '') }}</td>
              <td>
                <button class="btn btn-primary" @click="updateEntry(entry)">Edit</button>
                &nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-danger"
                  @click="deleteEntry(entry)"
                >
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger">Error: {{ errorMessage }}</div>
    <b-modal v-model="modalDelete" title="Delete Entry" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete:
        <strong>{{ selectedEntry.key }}</strong>
      </p>
    </b-modal>
    <b-modal v-model="modalAddUpdate" no-footer size="lg" v-bind:title="title">
      <form @submit.prevent="confirmAddUpdate">
        <div class="mb-3 row">
          <label for="key" class="col-sm-3 col-form-label">Key</label>
          <div class="col-sm-9">
            <input
              id="key"
              name="key"
              class="form-control"
              v-model="selectedEntry.key"
              @input="v$.selectedEntry.key.$touch()"
              :class="{
                'is-invalid': isSubmitted && v$.selectedEntry.key.$invalid,
              }"
            />
            <span class="invalid-feedback"> Enter a key </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="value" class="col-sm-3 col-form-label">Value</label>
          <div class="col-sm-9">
            <input
              id="value"
              name="value"
              class="form-control"
              v-model="selectedEntry.value"
              @input="v$.selectedEntry.value.$touch()"
              :class="{
                'is-invalid': isSubmitted && v$.selectedEntry.value.$invalid,
              }"
            />
            <span class="invalid-feedback"> Enter a value </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="description" class="col-sm-3 col-form-label">Description</label>
          <div class="col-sm-9">
            <input
              id="description"
              name="description"
              class="form-control"
              v-model="selectedEntry.description"
            />
            <span class="invalid-feedback"> Enter a description </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="sensitive" class="col-sm-3 col-form-label">Sensitive</label>
          <div class="col-sm-9">
            <input
              type="checkbox"
              id="sensitive"
              name="sensitive"
              v-model="selectedEntry.isSensitive"
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label class="col-sm-3 col-form-label"></label>
          <div class="col-sm-9">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </b-modal>
    <b-modal v-model="modalImportExcel" no-footer title="Import Excel">
      <form @submit.prevent="confirmImportExcelFile">
        <div class="mb-3 row">
          <div class="col-sm-12">
            <input
              id="importingFile"
              type="file"
              name="importingFile"
              class="form-control"
              :class="{
                'is-invalid': isImportExcelFormSubmitted && !importingFile,
              }"
              @change="handleFileInput(($event.target as HTMLInputElement)?.files || null)"
            />
            <span class="invalid-feedback"> Select a file </span>
          </div>
        </div>
        <div class="mb-3 row">
          <div class="col-sm-12" style="text-align: center">
            <button class="btn btn-primary">Import</button>
          </div>
        </div>
      </form>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import useVuelidate from '@vuelidate/core'
import { required } from '@vuelidate/validators'
import axios from './axios'
import type { IConfigurationEntry } from './ConfigurationEntry'

const configurationEntries = ref<IConfigurationEntry[]>([])
const selectedEntry = ref<IConfigurationEntry>({} as IConfigurationEntry)
const isSubmitted = ref(false)
const isImportExcelFormSubmitted = ref(false)
const importingFile = ref<File | null>(null)
const errorMessage = ref('')
const modalDelete = ref(false)
const modalAddUpdate = ref(false)
const modalImportExcel = ref(false)

const rules = {
  selectedEntry: {
    key: {
      required,
    },
    value: {
      required,
    },
  },
}

const v$ = useVuelidate(rules, { selectedEntry })

const title = computed(() => {
  return selectedEntry.value.id == '00000000-0000-0000-0000-000000000000' ? 'Add' : 'Update'
})

const loadConfigurationEntries = () => {
  axios.get('').then((rs) => {
    configurationEntries.value = rs.data
  })
}

const deleteEntry = (entry: IConfigurationEntry) => {
  selectedEntry.value = entry
  modalDelete.value = true
}

const deleteConfirmed = () => {
  axios.delete(selectedEntry.value.id).then((rs) => {
    loadConfigurationEntries()
  })
}

const addEntry = () => {
  selectedEntry.value = {
    id: '00000000-0000-0000-0000-000000000000',
    key: '',
    value: '',
    description: '',
    isSensitive: false,
    createdDateTime: new Date(),
  }
  isSubmitted.value = false
  modalAddUpdate.value = true
}

const updateEntry = (entry: IConfigurationEntry) => {
  selectedEntry.value = {
    id: entry.id,
    key: entry.key,
    value: entry.isSensitive ? '' : entry.value,
    description: entry.description,
    isSensitive: entry.isSensitive,
    createdDateTime: new Date(),
  }
  isSubmitted.value = false
  modalAddUpdate.value = true
}

const confirmAddUpdate = () => {
  isSubmitted.value = true
  if (v$.value.selectedEntry.$invalid) {
    return
  }

  const promise =
    selectedEntry.value.id != '00000000-0000-0000-0000-000000000000'
      ? axios.put(selectedEntry.value.id, selectedEntry.value)
      : axios.post('', selectedEntry.value)

  promise.then((rs) => {
    isSubmitted.value = false
    modalAddUpdate.value = false
    loadConfigurationEntries()
  })
}

const exportAsExcel = async () => {
  const rs = await axios.get('/ExportAsExcel', { responseType: 'blob' })
  const url = window.URL.createObjectURL(rs.data)
  const element = document.createElement('a')
  element.href = url
  element.download = 'Settings.xlsx'
  document.body.appendChild(element)
  element.click()
}

const openImportExcelModal = () => {
  isImportExcelFormSubmitted.value = false
  importingFile.value = null
  modalImportExcel.value = true
}

const handleFileInput = (files: FileList | null) => {
  if (files && files.length > 0) {
    importingFile.value = files.item(0)
  }
}

const confirmImportExcelFile = async () => {
  isImportExcelFormSubmitted.value = true
  if (!importingFile.value) {
    return
  }
  const formData = new FormData()
  formData.append('formFile', importingFile.value)
  const rs = await axios.post('ImportExcel', formData)
  isImportExcelFormSubmitted.value = false
  modalImportExcel.value = false
  loadConfigurationEntries()
}

const formatedDateTime = (value: string | Date): string => {
  if (!value) return ''
  const date = new Date(value)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString()
}

onMounted(() => {
  loadConfigurationEntries()
})
</script>

<style scoped>
thead {
  color: #337ab7;
}

.col-form-label {
  text-align: right;
}
</style>
