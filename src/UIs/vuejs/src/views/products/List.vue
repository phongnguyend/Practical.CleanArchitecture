<template>
  <div class="card">
    <div class="card-header">
      {{ pageTitle }}
      <router-link class="btn btn-primary" style="float: right;" to="/products/add">Add Product</router-link>
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
                <button
                  class="btn btn-primary"
                  @click="toggleImage"
                >{{ showImage ? "Hide" : "Show" }} Image</button>
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
                  :style="{width: imageWidth + 'px', margin: imageMargin+'px'}"
                />
              </td>
              <td>
                <router-link :to="'/products/'+ product.id">{{ product.name }}</router-link>
              </td>
              <td>{{ product.code | uppercase }}</td>
              <td>{{ product.description }}</td>
              <td>{{ product.price || 5 }}</td>
              <td>
                <app-star
                  :rating="product.starRating || 4"
                  @ratingClicked="onRatingClicked($event)"
                ></app-star>
              </td>
              <td>
                <router-link class="btn btn-primary" :to="'/products/edit/'+ product.id">Edit</router-link>&nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-secondary"
                  @click="viewAuditLogs(product)"
                >View Audit Logs</button>&nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-danger"
                  @click="deleteProduct(product)"
                >Delete</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
    <div v-if="errorMessage" class="alert alert-danger">Error: {{ errorMessage }}</div>
    <b-modal id="modal-delete" title="Delete Product" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete:
        <strong>{{selectedProduct.name}}</strong>
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
              <th>Code</th>
              <th>Name</th>
              <th>Description</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="auditLog in auditLogs" :key="auditLog.id">
              <td>{{ auditLog.createdDateTime | formatedDateTime}}</td>
              <td>{{ auditLog.userName }}</td>
              <td>{{ auditLog.action }}</td>
              <td :style="{color: auditLog.highLight.code ? 'red' : ''}">{{ auditLog.data.code }}</td>
              <td :style="{color: auditLog.highLight.name ? 'red' : ''}">{{ auditLog.data.name }}</td>
              <td
                :style="{color: auditLog.highLight.description ? 'red' : ''}"
              >{{ auditLog.data.description }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </b-modal>
  </div>
</template>

<script>
import axios from "./axios";

import logo from "../../assets/logo.png";
import Star from "../../components/Star.vue";

export default {
  data() {
    return {
      pageTitle: "Product List",
      showImage: false,
      imageWidth: 50,
      imageMargin: 2,
      logo: logo,
      products: [],
      auditLogs: [],
      selectedProduct: {},
      listFilter: "",
      errorMessage: "",
    };
  },
  computed: {
    filteredProducts() {
      if (this.listFilter) {
        return this.products.filter(
          (product) =>
            product.name
              .toLocaleLowerCase()
              .indexOf(this.listFilter.toLocaleLowerCase()) !== -1
        );
      }
      return this.products;
    },
  },
  methods: {
    toggleImage() {
      this.showImage = !this.showImage;
    },
    onRatingClicked(event) {
      this.pageTitle = "Product List: " + event;
    },
    loadProducts() {
      axios.get("").then((rs) => {
        this.products = rs.data;
      });
    },
    deleteProduct(product) {
      this.selectedProduct = product;
      this.$bvModal.show("modal-delete");
    },
    deleteConfirmed() {
      axios.delete(this.selectedProduct.id).then((rs) => {
        this.loadProducts();
      });
    },
    viewAuditLogs(product) {
      axios.get(product.id + "/auditLogs").then((rs) => {
        this.auditLogs = rs.data;
        this.$bvModal.show("modal-audit-logs");
      });
    },
  },
  components: {
    appStar: Star,
  },
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
    this.loadProducts();
  },
};
</script>

<style scoped>
thead {
  color: #337ab7;
}
</style>