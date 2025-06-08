<template>
  <div class="card">
    <div class="card-header">{{ title }}</div>
    <div class="card-body">
      <div class="alert alert-danger" v-show="postError">
        {{ postErrorMessage }}
      </div>
      <form @submit.prevent="onSubmit">
        <div class="mb-3 row">
          <label for="userName" class="col-sm-3 col-form-label">User Name</label>
          <div class="col-sm-9">
            <input
              id="userName"
              name="userName"
              class="form-control"
              v-model="user.userName"
              :class="{
                'is-invalid': isSubmitted && v$.user.userName.$invalid,
              }"
              @input="v$.user.userName.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="v$.user.userName.required.$invalid">Enter an user name</span>
              <span v-if="v$.user.userName.minLength.$invalid"
                >The user name must be longer than 3 characters.</span
              >
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="email" class="col-sm-3 col-form-label">Email</label>
          <div class="col-sm-9">
            <input
              id="email"
              name="email"
              class="form-control"
              v-model="user.email"
              :class="{ 'is-invalid': isSubmitted && v$.user.email.$invalid }"
              @input="v$.user.email.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="v$.user.email.required.$invalid">Enter an email</span>
              <span v-if="v$.user.email.minLength.$invalid"
                >The email must be longer than 3 characters.</span
              >
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="emailConfirmed" class="col-sm-3 col-form-label">Email Confirmed</label>
          <div class="col-sm-9">
            <input
              type="checkbox"
              id="emailConfirmed"
              name="emailConfirmed"
              v-model="user.emailConfirmed"
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="phoneNumber" class="col-sm-3 col-form-label">Phone Number</label>
          <div class="col-sm-9">
            <input
              id="phoneNumber"
              name="phoneNumber"
              class="form-control"
              v-model="user.phoneNumber"
            />
            <span class="invalid-feedback">Enter a phone number</span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="phoneNumberConfirmed" class="col-sm-3 col-form-label"
            >Phone Number Confirmed</label
          >
          <div class="col-sm-9">
            <input
              type="checkbox"
              id="phoneNumberConfirmed"
              name="phoneNumberConfirmed"
              v-model="user.phoneNumberConfirmed"
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="twoFactorEnabled" class="col-sm-3 col-form-label">Two Factor Enabled</label>
          <div class="col-sm-9">
            <input
              type="checkbox"
              id="twoFactorEnabled"
              name="twoFactorEnabled"
              v-model="user.twoFactorEnabled"
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="lockoutEnabled" class="col-sm-3 col-form-label">Lockout Enabled</label>
          <div class="col-sm-9">
            <input
              type="checkbox"
              id="lockoutEnabled"
              name="lockoutEnabled"
              v-model="user.lockoutEnabled"
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="accessFailedCount" class="col-sm-3 col-form-label">Access Failed Count</label>
          <div class="col-sm-9">
            <input
              type="number"
              id="accessFailedCount"
              name="accessFailedCount"
              class="form-control"
              v-model="user.accessFailedCount"
            />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="lockoutEnd" class="col-sm-3 col-form-label">Lockout End</label>
          <div class="col-sm-9">
            <VueDatePicker v-model="user.lockoutEnd" />
          </div>
        </div>
        <div class="mb-3 row">
          <label for="description" class="col-sm-3 col-form-label"></label>
          <div class="col-sm-9">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </div>
    <div class="card-footer">
      <router-link class="btn btn-outline-secondary" to="/users" style="width: 80px">
        <i class="fa fa-chevron-left"></i> Back
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import useVuelidate from '@vuelidate/core'
import { required, minLength } from '@vuelidate/validators'
import axios from './axios'

interface IEditUser {
  id: string
  userName: string
  email: string
  emailConfirmed: boolean
  phoneNumber: string
  phoneNumberConfirmed: boolean
  twoFactorEnabled: boolean
  lockoutEnabled: boolean
  lockoutEnd?: Date
  accessFailedCount: number
}

const route = useRoute()
const router = useRouter()

const user = ref<IEditUser>({ userName: '', email: '' } as IEditUser)
const postError = ref(false)
const postErrorMessage = ref('')
const isSubmitted = ref(false)

const title = computed((): string => {
  return route.params.id ? 'Edit User' : 'Add User'
})

const id = computed((): string => {
  return route.params.id as string
})

const rules = {
  user: {
    userName: {
      required,
      minLength: minLength(3),
    },
    email: {
      required,
      minLength: minLength(3),
    },
  },
}

const v$ = useVuelidate(rules, { user })

const onSubmit = () => {
  isSubmitted.value = true
  if (v$.value.user.$invalid) {
    return
  }

  user.value.lockoutEnd = user.value.lockoutEnd ? user.value.lockoutEnd : undefined
  user.value.accessFailedCount = user.value.accessFailedCount ? user.value.accessFailedCount : 0

  const promise = id.value 
    ? axios.put(id.value, user.value) 
    : axios.post('', user.value)

  promise.then((rs) => {
    const userId = id.value ? id.value : rs.data.id
    router.push('/users/' + userId)
  })
}

onMounted(() => {
  const userId = route.params.id as string
  if (userId) {
    axios.get(userId).then((rs) => {
      user.value = rs.data
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
