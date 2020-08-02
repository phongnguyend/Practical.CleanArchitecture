<template>
  <div class="card">
    <div class="card-header">{{title}}</div>
    <div class="card-body">
      <div class="alert alert-danger" v-show="postError">{{ postErrorMessage }}</div>
      <form @submit.prevent="onSubmit">
        <div class="form-group row">
          <label for="name" class="col-sm-2 col-form-label">Name</label>
          <div class="col-sm-10">
            <input
              id="name"
              name="name"
              class="form-control"
              v-model="file.name"
              :class="{'is-invalid': isSubmitted && $v.file.name.$invalid}"
              @input="$v.file.name.$touch()"
            />
            {{$v.name}}
            <span class="invalid-feedback">
              <span v-if="!$v.file.name.required">Enter a name</span>
              <span v-if="!$v.file.name.minLength">The name must be longer than 3 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="description" class="col-sm-2 col-form-label">Description</label>
          <div class="col-sm-10">
            <input
              id="description"
              name="description"
              class="form-control"
              v-model="file.description"
              :class=" {'is-invalid': isSubmitted && $v.file.description.$invalid}"
              @input="$v.file.description.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="!$v.file.description.required">Enter a description</span>
              <span v-if="!$v.file.description.maxLength">The code must be less than 100 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
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
        <i class="fa fa-chevron-left"></i> Back
      </router-link>&nbsp;
      <button
        type="button"
        class="btn btn-primary btn-secondary"
        @click="viewAuditLogs(file)"
      >View Audit Logs</button>
    </div>
    <b-modal id="modal-audit-logs" hide-footer hide-header size="xl">
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
              <td>{{ auditLog.createdDateTime | formatedDateTime}}</td>
              <td>{{ auditLog.userName }}</td>
              <td>{{ auditLog.action }}</td>
              <td :style="{color: auditLog.highLight.name ? 'red' : ''}">{{ auditLog.data.name }}</td>
              <td
                :style="{color: auditLog.highLight.description ? 'red' : ''}"
              >{{ auditLog.data.description }}</td>
              <td
                :style="{color: auditLog.highLight.fileName ? 'red' : ''}"
              >{{ auditLog.data.fileName }}</td>
              <td
                :style="{color: auditLog.highLight.fileLocation ? 'red' : ''}"
              >{{ auditLog.data.fileLocation }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </b-modal>
  </div>
</template>
<script>
import { required, minLength, maxLength } from "vuelidate/lib/validators";

import axios from "./axios";

export default {
  data() {
    return {
      file: { name: "", description: "" },
      auditLogs: [],
      postError: false,
      postErrorMessage: "",
      isSubmitted: false,
    };
  },
  computed: {
    title() {
      return this.$route.params.id ? "Edit File" : "Upload File";
    },
    id() {
      return this.$route.params.id;
    },
  },
  validations: {
    file: {
      name: {
        required,
        minLength: minLength(3),
      },
      description: { required, maxLength: maxLength(100) },
    },
  },
  methods: {
    onSubmit() {
      this.isSubmitted = true;

      if (this.$v.file.$invalid) {
        return;
      }

      const promise = this.id
        ? axios.put(this.id, this.file)
        : axios.post("", this.file);

      promise.then((rs) => {
        const id = this.id ? this.id : rs.data.id;
        this.$router.push("/files");
      });
    },
    viewAuditLogs(file) {
      axios.get(file.id + "/auditLogs").then((rs) => {
        this.auditLogs = rs.data;
        this.$bvModal.show("modal-audit-logs");
      });
    },
  },
  created() {
    const id = this.$route.params.id;
    if (id) {
      axios.get(id).then((rs) => {
        this.file = rs.data;
      });
    }
  },
};
</script>
<style scoped>
.field-error {
  border: 1px solid red;
}

.col-form-label {
  text-align: right;
}
</style>