<template>
  <div class="card">
    <div class="card-header">Audit Logs</div>
    <div class="card-body">
      <div style="float: right">
        <app-pagination
          :currentPage="currentPage"
          :totalItems="totalItems"
          :pageSize="pageSize"
          @pageSelected="pagedSelected"
        />
      </div>
      <div class="table-responsive" :style="{ width: '100%' }">
        <table class="table" v-if="auditLogs">
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
              <td>{{ formatedDateTime(auditLog.createdDateTime) }}</td>
              <td>{{ auditLog.userName }}</td>
              <td>{{ auditLog.action }}</td>
              <td>{{ auditLog.log }}</td>
            </tr>
          </tbody>
        </table>
      </div>
      <div style="float: right">
        <app-pagination
          :currentPage="currentPage"
          :totalItems="totalItems"
          :pageSize="pageSize"
          @pageSelected="pagedSelected"
        />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import axios from './axios'
import { IAuditLogEntry } from './AuditLog'
import Pagination from '../../components/Pagination.vue'

export default defineComponent({
  components: {
    appPagination: Pagination,
  },
  data() {
    return {
      pageTitle: 'Audit Logs' as string,
      auditLogs: [] as IAuditLogEntry[],
      totalItems: 0 as number,
      currentPage: 1 as number,
      pageSize: 5 as number,
      errorMessage: '' as string,
    }
  },
  computed: {},
  methods: {
    loadAuditLogs(page: number) {
      axios.get('paged?page=' + page + '&pageSize=' + this.pageSize).then((rs: any) => {
        this.auditLogs = rs.data.items
        this.totalItems = rs.data.totalItems
      })
    },
    pagedSelected(page: number) {
      this.currentPage = page
      this.loadAuditLogs(page)
    },
    lowercase: function (value: string) {
      return value.toLowerCase()
    },
    formatedDateTime: function (value: string) {
      if (!value) return value
      var date = new Date(value)
      return date.toLocaleDateString() + ' ' + date.toLocaleTimeString()
    },
  },
  created() {
    this.loadAuditLogs(this.currentPage)
  },
})
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>
