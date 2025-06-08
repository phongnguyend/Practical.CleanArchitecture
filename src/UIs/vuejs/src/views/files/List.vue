<template>
  <div class="card">
    <div class="card-header">
      Files
      <router-link class="btn btn-primary" style="float: right" to="/files/upload"
        >Upload File</router-link
      >
    </div>
    <div class="card-body">
      <div class="table-responsive" :style="{ width: '100%' }">
        <table class="table" v-if="files && files.length">
          <thead>
            <tr>
              <th>Name</th>
              <th>Description</th>
              <th>Size</th>
              <th>Uploaded Time</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="file in files" :key="file.id">
              <td>
                <router-link :to="'/files/edit/' + file.id"
                  >{{ file.name }} ({{ file.fileName }})</router-link
                >
              </td>
              <td>{{ file.description }}</td>
              <td>{{ file.size }}</td>
              <td>{{ formatedDateTime(file.uploadedTime) }}</td>
              <td>
                <button type="button" class="btn btn-primary btn-secondary" @click="download(file)">
                  Download
                </button>
                &nbsp;
                <router-link class="btn btn-primary" :to="'/files/edit/' + file.id"
                  >Edit</router-link
                >&nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-secondary"
                  @click="viewAuditLogs(file)"
                >
                  View Audit Logs
                </button>
                &nbsp;
                <button type="button" class="btn btn-primary btn-danger" @click="deleteFile(file)">
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger">Error: {{ errorMessage }}</div>
    <b-modal v-model="modalDelete" title="Delete File" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete:
        <strong>{{ selectedFile.name }}</strong>
      </p>
    </b-modal>
    <b-modal v-model="modalAuditLogs" no-footer no-header size="xl">
      <div class="table-responsive" :style="{ width: '100%' }">
        <table class="table">
          <thead>
            <tr>
              <th>Date Time</th>
              <th>User Name</th>
              <th>Action</th>
              <th>Name</th>
              <th>Description</th>
              <th>File Name</th>
              <th>File Location</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="auditLog in auditLogs" :key="auditLog.id">
              <td>{{ formatedDateTime(auditLog.createdDateTime) }}</td>
              <td>{{ auditLog.userName }}</td>
              <td>{{ auditLog.action }}</td>
              <td :style="{ color: auditLog.highLight.name ? 'red' : '' }">
                {{ auditLog.data.name }}
              </td>
              <td :style="{ color: auditLog.highLight.description ? 'red' : '' }">
                {{ auditLog.data.description }}
              </td>
              <td :style="{ color: auditLog.highLight.fileName ? 'red' : '' }">
                {{ auditLog.data.fileName }}
              </td>
              <td :style="{ color: auditLog.highLight.fileLocation ? 'red' : '' }">
                {{ auditLog.data.fileLocation }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from './axios'
import type { IFile } from './File'

interface IAuditLog {
  id: string
  createdDateTime: string
  userName: string
  action: string
  highLight: {
    name?: boolean
    description?: boolean
    fileName?: boolean
    fileLocation?: boolean
  }
  data: {
    name: string
    description: string
    fileName: string
    fileLocation: string
  }
}

const pageTitle = ref('Files')
const files = ref<IFile[]>([])
const selectedFile = ref<IFile>({} as IFile)
const auditLogs = ref<IAuditLog[]>([])
const errorMessage = ref('')
const modalAuditLogs = ref(false)
const modalDelete = ref(false)

const loadFiles = () => {
  axios.get('').then((rs) => {
    files.value = rs.data
  })
}

const download = (file: IFile) => {
  axios.get(file.id + '/download', { responseType: 'blob' }).then((rs) => {
    const url = window.URL.createObjectURL(rs.data)
    const element = document.createElement('a')
    element.href = url
    element.download = file.fileName
    document.body.appendChild(element)
    element.click()
  })
}

const deleteFile = (file: IFile) => {
  selectedFile.value = file
  modalDelete.value = true
}

const deleteConfirmed = () => {
  axios.delete(selectedFile.value.id).then((rs) => {
    loadFiles()
  })
}

const viewAuditLogs = (file: IFile) => {
  axios.get(file.id + '/auditLogs').then((rs) => {
    auditLogs.value = rs.data
    modalAuditLogs.value = true
  })
}

const formatedDateTime = (value: string | Date): string => {
  if (!value) return ''
  const date = new Date(value)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString()
}

onMounted(() => {
  loadFiles()
})
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>
