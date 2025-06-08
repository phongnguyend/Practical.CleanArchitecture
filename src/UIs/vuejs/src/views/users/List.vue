<template>
  <div class="card">
    <div class="card-header">
      Users
      <router-link class="btn btn-primary" style="float: right" to="/users/add"
        >Add User</router-link
      >
    </div>
    <div class="card-body">
      <div class="table-responsive" :style="{ width: '100%' }">
        <table class="table" v-if="users && users.length">
          <thead>
            <tr>
              <th>User Name</th>
              <th>Email</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in users" :key="user.id">
              <td>
                <router-link :to="'/users/' + user.id">{{ user.userName }}</router-link>
              </td>
              <td>{{ user.email }}</td>
              <td>
                <router-link class="btn btn-primary" :to="'/users/edit/' + user.id"
                  >Edit</router-link
                >&nbsp;
                <button type="button" class="btn btn-primary btn-danger" @click="deleteUser(user)">
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger">Error: {{ errorMessage }}</div>
    <b-modal v-model="modalDelete" title="Delete User" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete
        <strong>{{ selectedUser.userName }}</strong>
      </p>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from './axios'
import type { IUser } from './User'

const users = ref<IUser[]>([])
const selectedUser = ref<IUser>({} as IUser)
const errorMessage = ref('')
const modalDelete = ref(false)

const loadUsers = () => {
  axios.get('').then((rs) => {
    users.value = rs.data
  })
}

const deleteUser = (user: IUser) => {
  selectedUser.value = user
  modalDelete.value = true
}

const deleteConfirmed = () => {
  axios.delete(selectedUser.value.id).then((rs) => {
    loadUsers()
  })
}

onMounted(() => {
  loadUsers()
})
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>
