<template>
  <div class="card">
    <div class="card-header">
      {{ pageTitle }}
      <div style="float: right">
        <button type="button" class="btn btn-secondary" @click="exportAsPdf">
          Export as Pdf
        </button>
        &nbsp;
        <button type="button" class="btn btn-secondary" @click="exportAsCsv">
          Export as Csv
        </button>
        &nbsp;
        <router-link class="btn btn-primary" to="/products/add">Add Product</router-link>
        &nbsp;
        <button class="btn btn-primary" @click="openImportCsvModal()">
          Import Csv
        </button>
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
      <div class="table-responsive">
        <table class="table" v-if="products && products.length">
          <thead>
            <tr>
              <th>
                <button class="btn btn-primary" @click="toggleImage">
                  {{ showImage ? "Hide" : "Show" }} Image
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
                <img v-if="showImage" :src="
                  product.imageUrl || '/img/icons/android-chrome-192x192.png'
                " :title="product.name" :style="{
  width: imageWidth + 'px',
  margin: imageMargin + 'px'
}" />
              </td>
              <td>
                <router-link :to="'/products/' + product.id">{{
                    product.name
                }}</router-link>
              </td>
              <td>{{ uppercase(product.code) }}</td>
              <td>{{ product.description }}</td>
              <td>{{ product.price || 5 }}</td>
              <td>
                <app-star :rating="product.starRating || 4" @ratingClicked="onRatingClicked($event)"></app-star>
              </td>
              <td>
                <router-link class="btn btn-primary" :to="'/products/edit/' + product.id">Edit</router-link>&nbsp;
                <button type="button" class="btn btn-primary btn-secondary" @click="viewAuditLogs(product)">
                  View Audit Logs</button>&nbsp;
                <button type="button" class="btn btn-primary btn-danger" @click="deleteProduct(product)">
                  Delete
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger">
      Error: {{ errorMessage }}
    </div>
    <b-modal ref="modal-delete" title="Delete Product" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete:
        <strong>{{ selectedProduct.name }}</strong>
      </p>
    </b-modal>
    <b-modal ref="modal-audit-logs" hide-footer hide-header size="xl">
      <div class="table-responsive">
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
    <b-modal ref="modal-import-csv" hide-footer title="Import Csv">
      <form @submit.prevent="confirmImportCsvFile">
        <div class="form-group row">
          <div class="col-sm-12">
            <input id="importingFile" type="file" name="importingFile" class="form-control" :class="{
              'is-invalid': isImportCsvFormSubmitted && !importingFile
            }" @change="handleFileInput($event.target.files)" />
            <span class="invalid-feedback"> Select a file </span>
          </div>
        </div>
        <div class="form-group row">
          <div class="col-sm-12" style="text-align: center">
            <button class="btn btn-primary">Import</button>
          </div>
        </div>
      </form>
    </b-modal>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { BModal } from "bootstrap-vue";
import axios from "./axios";

import logo from "../../assets/logo.png";
import Star from "../../components/Star.vue";
import { IProduct } from "./Product";

export default defineComponent({
  data() {
    return {
      pageTitle: "Product List",
      showImage: false,
      imageWidth: 50,
      imageMargin: 2,
      logo: logo,
      products: [] as IProduct[],
      auditLogs: [],
      selectedProduct: {} as IProduct,
      listFilter: "",
      errorMessage: "",
      isImportCsvFormSubmitted: false,
      importingFile: null as File | null
    };
  },
  computed: {
    filteredProducts(): IProduct[] {
      if (this.listFilter) {
        return this.products.filter(
          product =>
            product.name
              .toLocaleLowerCase()
              .indexOf(this.listFilter.toLocaleLowerCase()) !== -1
        );
      }
      return this.products;
    }
  },
  methods: {
    toggleImage() {
      this.showImage = !this.showImage;
    },
    onRatingClicked(event: Event) {
      this.pageTitle = "Product List: " + event;
    },
    loadProducts() {
      axios.get("").then(rs => {
        this.products = rs.data;
      });
    },
    deleteProduct(product: IProduct) {
      this.selectedProduct = product;
      (this.$refs["modal-delete"] as BModal).show();
    },
    deleteConfirmed() {
      axios.delete(this.selectedProduct.id).then(rs => {
        this.loadProducts();
      });
    },
    viewAuditLogs(product: IProduct) {
      axios.get(product.id + "/auditLogs").then(rs => {
        this.auditLogs = rs.data;
        (this.$refs["modal-audit-logs"] as BModal).show();
      });
    },
    exportAsPdf() {
      axios.get("/ExportAsPdf", { responseType: "blob" }).then(rs => {
        const url = window.URL.createObjectURL(rs.data);
        const element = document.createElement("a");
        element.href = url;
        element.download = "Products.pdf";
        document.body.appendChild(element);
        element.click();
      });
    },
    exportAsCsv() {
      axios.get("/ExportAsCsv", { responseType: "blob" }).then(rs => {
        const url = window.URL.createObjectURL(rs.data);
        const element = document.createElement("a");
        element.href = url;
        element.download = "Products.csv";
        document.body.appendChild(element);
        element.click();
      });
    },
    openImportCsvModal() {
      this.isImportCsvFormSubmitted = false;
      this.importingFile = null;
      (this.$refs["modal-import-csv"] as BModal).show();
    },
    handleFileInput(files: FileList) {
      this.importingFile = files.item(0);
    },
    async confirmImportCsvFile() {
      this.isImportCsvFormSubmitted = true;
      if (!this.importingFile) {
        return;
      }
      const formData = new FormData();
      formData.append("formFile", this.importingFile);
      const rs = await axios.post("ImportCsv", formData);
      this.isImportCsvFormSubmitted = false;
      (this.$refs["modal-import-csv"] as BModal).hide();
      this.loadProducts();
    },
    formatedDateTime(value: string) {
      if (!value) return value;
      var date = new Date(value);
      return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    },
    uppercase(value: string) {
      return value?.toUpperCase();
    }
  },
  components: {
    appStar: Star
  },
  created() {
    this.loadProducts();
  }
});
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>
