<template>
  <div class="card">
    <div class="card-header">{{ pageTitle }}</div>
    <div class="card-body">
      <div style="float: right">
        <app-pagination
          :current-page="currentPage"
          :total-items="totalItems"
          :page-size="pageSize"
          @page-selected="pagedSelected"
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
              <td>
                <div class="position-relative">
                  <div class="position-absolute top-0 end-0">
                    <div class="d-flex">
                      <CopyToClipboard
                        :text="auditLog.log"
                        className="custom-icon fa fa-copy"
                        title="Copy this text"
                      />
                    </div>
                  </div>
                  <div class="pe-5">{{ auditLog.log }}</div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <div style="float: right">
        <app-pagination
          :current-page="currentPage"
          :total-items="totalItems"
          :page-size="pageSize"
          @page-selected="pagedSelected"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from './axios'
import type { IAuditLogEntry } from './AuditLog'
import Pagination from '../../components/Pagination.vue'
import CopyToClipboard from '../../components/CopyToClipboard.vue'

// Register the component
const appPagination = Pagination

const pageTitle = 'Audit Logs'
const auditLogs = ref<IAuditLogEntry[]>([])
const totalItems = ref(0)
const currentPage = ref(1)
const pageSize = ref(5)
const errorMessage = ref('')

const loadAuditLogs = async (page: number) => {
  try {
    const response = await axios.get(`paged?page=${page}&pageSize=${pageSize.value}`)
    auditLogs.value = response.data.items
    totalItems.value = response.data.totalItems
  } catch (error) {
    errorMessage.value = 'Failed to load audit logs'
    console.error(error)
  }
}

const pagedSelected = (page: number) => {
  currentPage.value = page
  loadAuditLogs(page)
}

const formatedDateTime = (value: string): string => {
  if (!value) return value
  const date = new Date(value)
  return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`
}

onMounted(() => {
  loadAuditLogs(currentPage.value)
})
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>
