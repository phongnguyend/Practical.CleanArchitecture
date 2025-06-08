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
              type="file"
              id="formFile"
              name="formFile"
              class="form-control"
              :class="{ 'is-invalid': isSubmitted && !hasFile }"
              @change="handleFileInput(($event.target as HTMLInputElement)?.files || null)"
            />
            <span class="invalid-feedback">
              <span>Select a file</span>
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="encrypted" class="col-sm-2 col-form-label">Encrypted</label>
          <div class="col-sm-10">
            <input type="checkbox" id="encrypted" name="encrypted" v-model="file.encrypted" />
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
        <i class="fa fa-chevron-left"></i> Back
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import useVuelidate from '@vuelidate/core'
import { required, minLength, maxLength } from '@vuelidate/validators'
import axios from './axios'
import type { IFile } from './File'

const route = useRoute()
const router = useRouter()

const file = ref<IFile>({ name: '', description: '', encrypted: false } as IFile)
const postError = ref(false)
const postErrorMessage = ref('')
const isSubmitted = ref(false)
const hasFile = ref(false)

const title = computed(() => {
  return 'Upload File'
})

const id = computed(() => {
  return route.params.id
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

const handleFileInput = (files: FileList | null) => {
  if (files && files.length > 0) {
    const selectedFile = files.item(0)
    if (selectedFile) {
      file.value.formFile = selectedFile
      hasFile.value = true
    }
  }
}

const onSubmit = () => {
  isSubmitted.value = true
  if (v$.value.file.$invalid || !hasFile.value) {
    return
  }
  const formData = new FormData()
  formData.append('formFile', file.value.formFile as File)
  formData.append('name', file.value.name)
  formData.append('description', file.value.description)
  formData.append('encrypted', file.value.encrypted.toString())
  const promise = axios.post('', formData)

  promise.then((rs) => {
    const fileId = rs.data.id
    router.push('/files/edit/' + fileId)
  })
}
</script>

<style scoped>
.field-error {
  border: 1px solid red;
}

.col-form-label {
  text-align: right;
}
</style>
