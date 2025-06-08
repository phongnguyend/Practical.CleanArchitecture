<template>
  <div class="card" v-if="user">
    <div class="card-header">{{ 'User Detail: ' + user.userName }}</div>

    <div class="card-body">
      <div class="row">
        <div class="col-md-8">
          <div class="row">
            <div class="col-md-4">User Name:</div>
            <div class="col-md-8">{{ user.userName }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Email:</div>
            <div class="col-md-8">{{ user.email }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Email Confirmed:</div>
            <div class="col-md-8">{{ user.emailConfirmed }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Phone Number:</div>
            <div class="col-md-8">{{ user.phoneNumber }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Phone Number Confirmed:</div>
            <div class="col-md-8">{{ user.phoneNumberConfirmed }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Two Factor Enabled:</div>
            <div class="col-md-8">{{ user.twoFactorEnabled }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Lockout Enabled:</div>
            <div class="col-md-8">{{ user.lockoutEnabled }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Access Failed Count:</div>
            <div class="col-md-8">{{ user.accessFailedCount }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Lockout End:</div>
            <div class="col-md-8">{{ user.lockoutEnd }}</div>
          </div>
        </div>

        <div class="col-md-4">
          <img
            class="center-block img-responsive"
            :style="{ width: '200px', margin: '2px' }"
            :src="'/img/icons/android-chrome-192x192.png'"
            :title="user.userName"
          />
        </div>
      </div>
    </div>

    <div class="card-footer">
      <button class="btn btn-outline-secondary" @click="onBack" style="width: 80px">
        <i class="fa fa-chevron-left"></i> Back
      </button>
      &nbsp;
      <router-link class="btn btn-primary" :to="'/users/edit/' + user.id">Edit</router-link>&nbsp;
      <button type="button" class="btn btn-secondary" @click="setPasswordModal()">
        Set Password</button
      >&nbsp;
      <button type="button" class="btn btn-secondary" @click="sendPasswordResetEmailModal()">
        Send Password Reset Email</button
      >&nbsp;
      <button
        type="button"
        class="btn btn-secondary"
        @click="sendEmailAddressConfirmationEmailModal()"
      >
        Send Email Address Confirmation Email
      </button>
    </div>

    <b-modal v-model="modalSetPassword" title="Set Password" :no-footer="true">
      <div
        class="row alert alert-danger"
        v-show="passwordValidationErrors && passwordValidationErrors.length"
      >
        <ul>
          <li v-for="error in passwordValidationErrors" :key="error.code">
            {{ error.description }}
          </li>
        </ul>
      </div>
      <div class="row alert alert-danger" v-show="postErrorMessage">
        {{ postErrorMessage }}
      </div>
      <form @submit.prevent="confirmSetPassword">
        <div class="mb-3 row">
          <label class="col-sm-4 col-form-label">User Name</label>
          <div class="col-sm-8">{{ user.userName }}</div>
        </div>
        <div class="mb-3 row">
          <label for="password" class="col-sm-4 col-form-label">Password</label>
          <div class="col-sm-8">
            <input
              type="password"
              id="password"
              name="password"
              class="form-control"
              required
              v-model="setPasswordModel.password"
              :class="{
                'is-invalid': isSubmitted && v$.setPasswordModel.password.$invalid,
              }"
              @input="v$.setPasswordModel.password.$touch()"
            />
            <span class="invalid-feedback">Enter a password</span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="confirmPassword" class="col-sm-4 col-form-label">Confirm Password</label>
          <div class="col-sm-8">
            <input
              type="password"
              id="confirmPassword"
              name="confirmPassword"
              class="form-control"
              v-model="setPasswordModel.confirmPassword"
              :class="{
                'is-invalid':
                  isSubmitted && setPasswordModel.confirmPassword != setPasswordModel.password,
              }"
            />
            <span class="invalid-feedback">Confirm Password does not match</span>
          </div>
        </div>
        <div class="mb-3 row">
          <label class="col-sm-4 col-form-label"></label>
          <div class="col-sm-8">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </b-modal>

    <b-modal
      v-model="modalSendPasswordResetEmail"
      title="Send Password Reset Email"
      @ok="confirmSendPasswordResetEmail"
    >
      <p>
        Are you sure you want to send reset password email
        <strong>{{ user.userName }}</strong>
      </p>
    </b-modal>

    <b-modal
      v-model="modalSendEmailAddressConfirmationEmail"
      title="Send Email Address Confirmation Email"
      @ok="confirmSendEmailAddressConfirmationEmail"
    >
      <p>
        Are you sure you want to send email address confirmation email
        <strong>{{ user.userName }}</strong>
      </p>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import useVuelidate from '@vuelidate/core'
import { required } from '@vuelidate/validators'
import axios from './axios'
import type { IUser } from './User'

interface PasswordValidationError {
  code: string
  description: string
}

interface SetPasswordModel {
  password: string | null
  confirmPassword: string | null
}

const route = useRoute()
const router = useRouter()

const errorMessage = ref('')
const postErrorMessage = ref('')
const setPasswordModel = ref<SetPasswordModel>({ password: null, confirmPassword: null })
const isSubmitted = ref(false)
const passwordValidationErrors = ref<PasswordValidationError[]>([])
const user = ref<IUser>({} as IUser)
const modalSetPassword = ref(false)
const modalSendPasswordResetEmail = ref(false)
const modalSendEmailAddressConfirmationEmail = ref(false)

const rules = {
  setPasswordModel: {
    password: {
      required,
    },
  },
}

const v$ = useVuelidate(rules, { setPasswordModel })

const onBack = () => {
  router.push('/users')
}

const setPasswordModal = () => {
  modalSetPassword.value = true
}

const confirmSetPassword = () => {
  isSubmitted.value = true

  if (
    !setPasswordModel.value.password ||
    setPasswordModel.value.password != setPasswordModel.value.confirmPassword
  ) {
    return
  }

  axios
    .put(user.value.id + '/password', {
      id: user.value.id,
      password: setPasswordModel.value.password,
    })
    .then((rs) => {
      setPasswordModel.value = { password: null, confirmPassword: null }
      isSubmitted.value = false
      passwordValidationErrors.value = []
      postErrorMessage.value = ''
      modalSetPassword.value = false
    })
    .catch((error) => {
      if (error?.response?.status == 400) {
        passwordValidationErrors.value = error.response.data
      } else {
        postErrorMessage.value = error?.response?.status
      }
    })
}

const sendPasswordResetEmailModal = () => {
  modalSendPasswordResetEmail.value = true
}

const confirmSendPasswordResetEmail = () => {
  axios
    .post(user.value.id + '/passwordresetemail', {
      id: user.value.id,
    })
    .then((rs) => {
      modalSendPasswordResetEmail.value = false
    })
}

const sendEmailAddressConfirmationEmailModal = () => {
  modalSendEmailAddressConfirmationEmail.value = true
}

const confirmSendEmailAddressConfirmationEmail = () => {
  axios
    .post(user.value.id + '/emailaddressconfirmation', {
      id: user.value.id,
    })
    .then((rs) => {
      modalSendEmailAddressConfirmationEmail.value = false
    })
}

onMounted(() => {
  const id = route.params.id as string
  axios.get(id).then((rs) => {
    user.value = rs.data
  })
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
