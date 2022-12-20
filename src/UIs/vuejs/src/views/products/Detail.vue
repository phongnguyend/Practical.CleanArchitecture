<template>
  <div class="card" v-if="product">
    <div class="card-header">{{ pageTitle + ": " + product.name }}</div>

    <div class="card-body">
      <div class="row">
        <div class="col-md-8">
          <div class="row">
            <div class="col-md-4">Name:</div>
            <div class="col-md-8">{{ product.name }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Code:</div>
            <div class="col-md-8">{{ uppercase(product.code) }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Description:</div>
            <div class="col-md-8">{{ product.description }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Price:</div>
            <div class="col-md-8">{{ product.price || 5 }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">5 Star Rating:</div>
            <div class="col-md-8">
              <app-star :rating="product.starRating || 4"></app-star>
            </div>
          </div>
        </div>

        <div class="col-md-4">
          <img class="center-block img-responsive" :style="{ width: '200px', margin: '2px' }"
            :src="product.imageUrl || '/img/icons/android-chrome-192x192.png'" :title="product.name" />
        </div>
      </div>
    </div>

    <div class="card-footer">
      <button class="btn btn-outline-secondary" @click="onBack" style="width:80px">
        <i class="fa fa-chevron-left"></i> Back
      </button>
      &nbsp;
      <router-link class="btn btn-primary" :to="'/products/edit/' + product.id">Edit</router-link>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import axios from "./axios";

import Star from "../../components/Star.vue";
import { IProduct } from "./Product";

export default defineComponent({
  data() {
    return {
      pageTitle: "Product Detail",
      errorMessage: "",
      product: {} as IProduct
    };
  },
  methods: {
    onBack() {
      this.$router.push("/products");
    },
    uppercase(value: string) {
      return value?.toUpperCase();
    }
  },
  components: {
    appStar: Star
  },
  created() {
    const id = this.$route.params.id as string;
    axios.get(id).then(rs => {
      this.product = rs.data;
    });
  }
});
</script>
