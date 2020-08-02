<template>
  <div class="card">
    <div class="card-header">Audit Logs</div>
    <div class="card-body">
      <div class="table-responsive">
        <table class="table" v-if="auditLogs && auditLogs.length">
          <thead>
            <tr>
              <th>Date Time</th>
              <th>User Name</th>
              <th>Action</th>
              <th>Data</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="auditLog in auditLogs" :key="auditLog.id">
              <td>{{ auditLog.createdDateTime | formatedDateTime}}</td>
              <td>{{ auditLog.userName }}</td>
              <td>{{ auditLog.action }}</td>
              <td>{{ auditLog.log }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "./axios";

export default {
  data() {
    return {
      pageTitle: "Audit Logs",
      auditLogs: [],
      errorMessage: "",
    };
  },
  computed: {},
  methods: {
    loadAuditLogs() {
      axios.get("").then((rs) => {
        this.auditLogs = rs.data;
      });
    },
  },
  components: {},
  filters: {
    lowercase: function (value) {
      return value.toLowerCase();
    },
    formatedDateTime: function (value) {
      if (!value) return value;
      var date = new Date(value);
      return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    },
  },
  created() {
    this.loadAuditLogs();
  },
};
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>