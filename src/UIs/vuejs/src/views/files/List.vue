<template>
  <div class="card">
    <div class="card-header">
      Files
      <router-link class="btn btn-primary" style="float: right;" to="/files/upload">Upload File</router-link>
    </div>
    <div class="card-body">
      <div class="table-responsive">
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
                <router-link :to="'/files/edit/'+ file.id">{{ file.name }} ({{ file.fileName }})</router-link>
              </td>
              <td>{{ file.description }}</td>
              <td>{{ file.size }}</td>
              <td>{{ file.uploadedTime | formatedDateTime}}</td>
              <td>
                <button
                  type="button"
                  class="btn btn-primary btn-secondary"
                  @click="download(file)"
                >Download</button>
                &nbsp;
                <router-link class="btn btn-primary" :to="'/files/edit/'+ file.id">Edit</router-link>&nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-secondary"
                  @click="viewAuditLogs(file)"
                >View Audit Logs</button>
                &nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-danger"
                  @click="deleteFile(file)"
                >Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger">Error: {{ errorMessage }}</div>
    <b-modal id="modal-delete" title="Delete File" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete:
        <strong>{{selectedFile.name}}</strong>
      </p>
    </b-modal>
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
import axios from "./axios";

export default {
  data() {
    return {
      pageTitle: "Files",
      files: [],
      selectedFile: {},
      auditLogs: [],
      errorMessage: "",
    };
  },
  computed: {},
  methods: {
    loadFiles() {
      axios.get("").then((rs) => {
        this.files = rs.data;
      });
    },
    download(file) {
      axios.get(file.id + "/download", { responseType: "blob" }).then((rs) => {
        const url = window.URL.createObjectURL(rs.data);
        const element = document.createElement("a");
        element.href = url;
        element.download = file.fileName;
        document.body.appendChild(element);
        element.click();
      });
    },
    deleteFile(file) {
      this.selectedFile = file;
      this.$bvModal.show("modal-delete");
    },
    deleteConfirmed() {
      axios.delete(this.selectedFile.id).then((rs) => {
        this.loadFiles();
      });
    },
    viewAuditLogs(file) {
      axios.get(file.id + "/auditLogs").then((rs) => {
        this.auditLogs = rs.data;
        this.$bvModal.show("modal-audit-logs");
      });
    },
  },
  components: {},
  filters: {
    formatedDateTime: function (value) {
      if (!value) return value;
      var date = new Date(value);
      return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    },
  },
  created() {
    this.loadFiles();
  },
};
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>