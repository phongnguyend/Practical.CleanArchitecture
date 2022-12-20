<template>
  <div class="card">
    <div class="card-header">{{ title }}</div>
    <div class="card-body">
      <div class="alert alert-danger" v-show="postError">
        {{ postErrorMessage }}
      </div>
      <form @submit.prevent="onSubmit">
        <div class="form-group row">
          <label for="name" class="col-sm-2 col-form-label">Name</label>
          <div class="col-sm-10">
            <input id="name" name="name" class="form-control" v-model="file.name"
              :class="{ 'is-invalid': isSubmitted && v$.file.name.$invalid }" @input="v$.file.name.$touch()" />
            <span class="invalid-feedback">
              <span v-if="v$.file.name.required.$invalid">Enter a name</span>
              <span v-if="v$.file.name.minLength.$invalid">The name must be longer than 3 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="description" class="col-sm-2 col-form-label">Description</label>
          <div class="col-sm-10">
            <input id="description" name="description" class="form-control" v-model="file.description" :class="{
              'is-invalid': isSubmitted && v$.file.description.$invalid
            }" @input="v$.file.description.$touch()" />
            <span class="invalid-feedback">
              <span v-if="v$.file.description.required.$invalid">Enter a description</span>
              <span v-if="v$.file.description.maxLength.$invalid">The code must be less than 100 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="formFile" class="col-sm-2 col-form-label">File</label>
          <div class="col-sm-10">
            <input id="formFile" name="formFile" class="form-control" :value="file.fileName" disabled />
          </div>
        </div>
        <div class="form-group row">
          <label for="encrypted" class="col-sm-2 col-form-label">Encrypted</label>
          <div class="col-sm-10">
            <input type="checkbox" id="encrypted" name="encrypted" v-model="file.encrypted" disabled />
          </div>
        </div>
        <div class="form-group row">
          <label for="description" class="col-sm-2 col-form-label"></label>
          <div class="col-sm-10">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </div>
    <div class="card-footer">
      <router-link class="btn btn-outline-secondary" to="/files" style="width:80px">
        <i class="fa fa-chevron-left"></i> Back </router-link>&nbsp;
      <button type="button" class="btn btn-primary btn-secondary" @click="viewAuditLogs(file)">
        View Audit Logs
      </button>
    </div>
    <b-modal ref="modal-audit-logs" hide-footer hide-header size="xl">
      <div class="table-responsive">
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

<script lang="ts">
import { defineComponent } from "vue";
import { useVuelidate } from '@vuelidate/core'
import { required, minLength, maxLength } from "@vuelidate/validators";
import { BModal } from "bootstrap-vue";

import axios from "./axios";
import { IFile } from "./File";

export default defineComponent({
  setup() {
    return { v$: useVuelidate() }
  },
  data() {
    return {
      file: { name: "", description: "" } as IFile,
      auditLogs: [],
      postError: false,
      postErrorMessage: "",
      isSubmitted: false
    };
  },
  computed: {
    title() {
      return this.$route.params.id ? "Edit File" : "Upload File";
    },
    id() {
      return this.$route.params.id as string;
    }
  },
  validations: {
    file: {
      name: {
        required,
        minLength: minLength(3)
      },
      description: { required, maxLength: maxLength(100) }
    }
  },
  methods: {
    onSubmit() {
      this.isSubmitted = true;

      if (this.v$.file.$invalid) {
        return;
      }

      const promise = this.id
        ? axios.put(this.id, this.file)
        : axios.post("", this.file);

      promise.then(rs => {
        const id = this.id ? this.id : rs.data.id;
        this.$router.push("/files");
      });
    },
    viewAuditLogs(file: IFile) {
      axios.get(file.id + "/auditLogs").then(rs => {
        this.auditLogs = rs.data;
        (this.$refs["modal-audit-logs"] as BModal).show();
      });
    },
    formatedDateTime(value: string) {
      if (!value) return value;
      var date = new Date(value);
      return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    }
  },
  created() {
    const id = this.$route.params.id as string;
    if (id) {
      axios.get(id).then(rs => {
        this.file = rs.data;
      });
    }
  }
});
</script>

<style scoped>
.field-error {
  border: 1px solid red;
}

.col-form-label {
  text-align: right;
}
</style>
