<template>
  <div class="card">
    <div class="card-header">{{ title }}</div>
    <div class="card-body">
      <div class="alert alert-danger" v-show="postError">
        {{ postErrorMessage }}
      </div>
      <form @submit.prevent="onSubmit">
        <div class="mb-3 row">
          <label for="name" class="col-sm-2 col-form-label">Name</label>
          <div class="col-sm-10">
            <input
              id="name"
              name="name"
              class="form-control"
              v-model="file.name"
              :class="{ 'is-invalid': isSubmitted && v$.file.name.$invalid }"
              @input="v$.file.name.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="v$.file.name.required.$invalid">Enter a name</span>
              <span v-if="v$.file.name.minLength.$invalid"
                >The name must be longer than 3 characters.</span
              >
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="description" class="col-sm-2 col-form-label">Description</label>
          <div class="col-sm-10">
            <input
              id="description"
              name="description"
              class="form-control"
              v-model="file.description"
              :class="{
                'is-invalid': isSubmitted && v$.file.description.$invalid,
              }"
              @input="v$.file.description.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="v$.file.description.required.$invalid">Enter a description</span>
              <span v-if="v$.file.description.maxLength.$invalid"
                >The code must be less than 100 characters.</span
              >
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="formFile" class="col-sm-2 col-form-label">File</label>
          <div class="col-sm-10">
            <input
              id="formFile"
              name="formFile"
              class="form-control"
              :value="file.fileName"
              disabled
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="encrypted" class="col-sm-2 col-form-label">Encrypted</label>
          <div class="col-sm-10">
            <input
              type="checkbox"
              id="encrypted"
              name="encrypted"
              v-model="file.encrypted"
              disabled
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="description" class="col-sm-2 col-form-label"></label>
          <div class="col-sm-10">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </div>
    <div class="card-footer">
      <router-link class="btn btn-outline-secondary" to="/files" style="width: 80px">
        <i class="fa fa-chevron-left"></i> Back </router-link
      >&nbsp;
      <button type="button" class="btn btn-primary btn-secondary" @click="viewAuditLogs(file)">
        View Audit Logs
      </button>
    </div>
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
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import useVuelidate from '@vuelidate/core'
import { required, minLength, maxLength } from '@vuelidate/validators'
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

const route = useRoute()
const router = useRouter()

const file = ref<IFile>({ name: '', description: '' } as IFile)
const auditLogs = ref<IAuditLog[]>([])
const postError = ref(false)
const postErrorMessage = ref('')
const isSubmitted = ref(false)
const modalAuditLogs = ref(false)

const title = computed(() => {
  return route.params.id ? 'Edit File' : 'Upload File'
})

const id = computed(() => {
  return route.params.id as string
})

const rules = {
  file: {
    name: {
      required,
      minLength: minLength(3),
    },
    description: { 
      required, 
      maxLength: maxLength(100) 
    },
  },
}

const v$ = useVuelidate(rules, { file })

const onSubmit = () => {
  isSubmitted.value = true

  if (v$.value.file.$invalid) {
    return
  }

  const promise = id.value 
    ? axios.put(id.value, file.value) 
    : axios.post('', file.value)

  promise.then((rs) => {
    const fileId = id.value ? id.value : rs.data.id
    router.push('/files')
  })
}

const viewAuditLogs = (fileItem: IFile) => {
  axios.get(fileItem.id + '/auditLogs').then((rs) => {
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
  const fileId = route.params.id as string
  if (fileId) {
    axios.get(fileId).then((rs) => {
      file.value = rs.data
    })
  }
})
</script>

<style scoped>
.field-error {
  border: 1px solid red;
}

.col-form-label {
  text-align: right;
}
</style>
