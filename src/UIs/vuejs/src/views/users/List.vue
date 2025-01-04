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

<script lang="ts">
import { defineComponent, ref } from 'vue'
import axios from './axios'

import { IUser } from './User'

export default defineComponent({
  data() {
    return {
      users: [] as IUser[],
      selectedUser: {} as IUser,
      errorMessage: '',
      modalDelete: ref(false),
    }
  },
  computed: {},
  methods: {
    loadUsers() {
      axios.get('').then((rs) => {
        this.users = rs.data
      })
    },
    deleteUser(user: IUser) {
      this.selectedUser = user
      this.modalDelete = true
    },
    deleteConfirmed() {
      axios.delete(this.selectedUser.id).then((rs) => {
        this.loadUsers()
      })
    },
  },
  components: {},
  created() {
    this.loadUsers()
  },
})
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>
