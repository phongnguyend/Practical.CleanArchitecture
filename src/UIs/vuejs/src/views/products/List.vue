<template>
  <div class="card">
    <div class="card-header">
      {{ pageTitle }}
      <div style="float: right">
        <button type="button" class="btn btn-secondary" @click="exportAsPdf">Export as Pdf</button>
        &nbsp;
        <button type="button" class="btn btn-secondary" @click="exportAsCsv">Export as Csv</button>
        &nbsp;
        <router-link class="btn btn-primary" to="/products/add">Add Product</router-link>
        &nbsp;
        <button class="btn btn-primary" @click="openImportCsvModal()">Import Csv</button>
      </div>
    </div>
    <div class="card-body">
      <div class="row">
        <div class="col-md-2">Filter by:</div>
        <div class="col-md-4">
          <input type="text" v-model="listFilter" />
        </div>
      </div>
      <div class="row" v-if="listFilter">
        <div class="col-md-6">
          <h4>Filtered by: {{ listFilter }}</h4>
        </div>
      </div>
      <div class="table-responsive" :style="{ width: '100%' }">
        <table class="table" v-if="products && products.length">
          <thead>
            <tr>
              <th>
                <button class="btn btn-primary" @click="toggleImage">
                  {{ showImage ? 'Hide' : 'Show' }} Image
                </button>
              </th>
              <th>Product</th>
              <th>Code</th>
              <th>Description</th>
              <th>Price</th>
              <th>5 Star Rating</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="product in filteredProducts" :key="product.id">
              <td>
                <img
                  v-if="showImage"
                  :src="product.imageUrl || '/img/icons/android-chrome-192x192.png'"
                  :title="product.name"
                  :style="{
                    width: imageWidth + 'px',
                    margin: imageMargin + 'px',
                  }"
                />
              </td>
              <td>
                <router-link :to="'/products/' + product.id">{{ product.name }}</router-link>
              </td>
              <td>{{ uppercase(product.code) }}</td>
              <td>{{ product.description }}</td>
              <td>{{ product.price || 5 }}</td>
              <td>
                <app-star
                  :rating="product.starRating || 4"
                  @ratingClicked="onRatingClicked($event)"
                ></app-star>
              </td>
              <td>
                <router-link class="btn btn-primary" :to="'/products/edit/' + product.id"
                  >Edit</router-link
                >&nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-secondary"
                  @click="viewAuditLogs(product)"
                >
                  View Audit Logs</button
                >&nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-danger"
                  @click="deleteProduct(product)"
                >
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger">Error: {{ errorMessage }}</div>
    <b-modal v-model="modalDelete" title="Delete Product" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete:
        <strong>{{ selectedProduct.name }}</strong>
      </p>
    </b-modal>
    <b-modal v-model="modalAuditLogs" no-footer no-header size="xl">
      <div class="table-responsive" :style="{ width: '100%' }">
        <table class="table">
          <thead>
            <tr>
              <th>Date Time</th>
              <th>User Name</th>
              <th>Action</th>
              <th>Code</th>
              <th>Name</th>
              <th>Description</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="auditLog in auditLogs" :key="auditLog.id">
              <td>{{ formatedDateTime(auditLog.createdDateTime) }}</td>
              <td>{{ auditLog.userName }}</td>
              <td>{{ auditLog.action }}</td>
              <td :style="{ color: auditLog.highLight.code ? 'red' : '' }">
                {{ auditLog.data.code }}
              </td>
              <td :style="{ color: auditLog.highLight.name ? 'red' : '' }">
                {{ auditLog.data.name }}
              </td>
              <td :style="{ color: auditLog.highLight.description ? 'red' : '' }">
                {{ auditLog.data.description }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </b-modal>
    <b-modal v-model="modalImportCsv" no-footer title="Import Csv">
      <form @submit.prevent="confirmImportCsvFile">
        <div class="mb-3 row">
          <div class="col-sm-12">
            <input
              id="importingFile"
              type="file"
              name="importingFile"
              class="form-control"
              :class="{
                'is-invalid': isImportCsvFormSubmitted && !importingFile,
              }"
              @change="handleFileInput($event.target.files)"
            />
            <span class="invalid-feedback"> Select a file </span>
          </div>
        </div>
        <div class="mb-3 row">
          <div class="col-sm-12" style="text-align: center">
            <button class="btn btn-primary">Import</button>
          </div>
        </div>
      </form>
    </b-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import axios from './axios'
import logo from '../../assets/logo.png'
import AppStar from '../../components/Star.vue'
import type { IProduct } from './Product'

interface IAuditLog {
  id: string
  createdDateTime: string
  userName: string
  action: string
  highLight: {
    code?: boolean
    name?: boolean
    description?: boolean
  }
  data: {
    code: string
    name: string
    description: string
  }
}

const pageTitle = ref('Product List')
const showImage = ref(false)
const imageWidth = ref(50)
const imageMargin = ref(2)
const products = ref<IProduct[]>([])
const auditLogs = ref<IAuditLog[]>([])
const selectedProduct = ref<IProduct>({} as IProduct)
const listFilter = ref('')
const errorMessage = ref('')
const isImportCsvFormSubmitted = ref(false)
const importingFile = ref<File | null>(null)
const modalAuditLogs = ref(false)
const modalDelete = ref(false)
const modalImportCsv = ref(false)

const filteredProducts = computed((): IProduct[] => {
  if (listFilter.value) {
    return products.value.filter(
      (product) =>
        product.name.toLocaleLowerCase().indexOf(listFilter.value.toLocaleLowerCase()) !== -1
    )
  }
  return products.value
})

const toggleImage = () => {
  showImage.value = !showImage.value
}

const onRatingClicked = (event: string) => {
  pageTitle.value = 'Product List: ' + event
}

const loadProducts = () => {
  axios.get('').then((rs) => {
    products.value = rs.data
  })
}

const deleteProduct = (product: IProduct) => {
  selectedProduct.value = product
  modalDelete.value = true
}

const deleteConfirmed = () => {
  axios.delete(selectedProduct.value.id).then((rs) => {
    loadProducts()
  })
}

const viewAuditLogs = (product: IProduct) => {
  axios.get(product.id + '/auditLogs').then((rs) => {
    auditLogs.value = rs.data
    modalAuditLogs.value = true
  })
}

const exportAsPdf = () => {
  axios.get('/ExportAsPdf', { responseType: 'blob' }).then((rs) => {
    const url = window.URL.createObjectURL(rs.data)
    const element = document.createElement('a')
    element.href = url
    element.download = 'Products.pdf'
    document.body.appendChild(element)
    element.click()
  })
}

const exportAsCsv = () => {
  axios.get('/ExportAsCsv', { responseType: 'blob' }).then((rs) => {
    const url = window.URL.createObjectURL(rs.data)
    const element = document.createElement('a')
    element.href = url
    element.download = 'Products.csv'
    document.body.appendChild(element)
    element.click()
  })
}

const openImportCsvModal = () => {
  isImportCsvFormSubmitted.value = false
  importingFile.value = null
  modalImportCsv.value = true
}

const handleFileInput = (files: FileList | null) => {
  importingFile.value = files?.item(0) || null
}

const confirmImportCsvFile = async () => {
  isImportCsvFormSubmitted.value = true
  if (!importingFile.value) {
    return
  }
  const formData = new FormData()
  formData.append('formFile', importingFile.value)
  const rs = await axios.post('ImportCsv', formData)
  isImportCsvFormSubmitted.value = false
  modalImportCsv.value = false
  loadProducts()
}

const formatedDateTime = (value: string) => {
  if (!value) return value
  const date = new Date(value)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString()
}

const uppercase = (value: string) => {
  return value?.toUpperCase()
}

onMounted(() => {
  loadProducts()
})
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>
