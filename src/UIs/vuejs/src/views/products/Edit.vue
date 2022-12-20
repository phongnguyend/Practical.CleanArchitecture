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
            <input id="name" name="name" class="form-control" v-model="product.name"
              :class="{ 'is-invalid': isSubmitted && v$.product.name.$invalid }" @input="v$.product.name.$touch()" />
            <span class="invalid-feedback">
              <span v-if="v$.product.name.required.$invalid">Enter a name</span>
              <span v-if="v$.product.name.minLength.$invalid">The name must be longer than 3 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="code" class="col-sm-2 col-form-label">Code</label>
          <div class="col-sm-10">
            <input id="code" name="code" class="form-control" v-model="product.code"
              :class="{ 'is-invalid': isSubmitted && v$.product.code.$invalid }" @input="v$.product.code.$touch()" />
            <span class="invalid-feedback">
              <span v-if="v$.product.code.required.$invalid">Enter a code</span>
              <span v-if="v$.product.code.maxLength.$invalid">The code must be less than 10 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="description" class="col-sm-2 col-form-label">Description</label>
          <div class="col-sm-10">
            <input id="description" name="description" class="form-control" v-model="product.description" :class="{
              'is-invalid': isSubmitted && v$.product.description.$invalid
            }" @input="v$.product.description.$touch()" />
            <span class="invalid-feedback">
              <span v-if="v$.product.description.required.$invalid">Enter a description</span>
              <span v-if="v$.product.description.maxLength.$invalid">The code must be less than 100 characters.</span>
            </span>
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
      <router-link class="btn btn-outline-secondary" to="/products" style="width:80px">
        <i class="fa fa-chevron-left"></i> Back
      </router-link>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import useVuelidate from "@vuelidate/core";
import { required, minLength, maxLength } from "@vuelidate/validators";
import axios from "./axios";

import { IProduct } from "./Product";

export default defineComponent({
  setup() {
    return { v$: useVuelidate() }
  },
  data() {
    return {
      product: { name: "", code: "", description: "" } as IProduct,
      postError: false,
      postErrorMessage: "",
      isSubmitted: false
    };
  },
  computed: {
    title() {
      return this.$route.params.id ? "Edit Product" : "Add Product";
    },
    id() {
      return this.$route.params.id as string;
    }
  },
  validations: {
    product: {
      name: {
        required,
        minLength: minLength(3)
      },
      code: {
        required,
        maxLength: maxLength(10)
      },
      description: { required, maxLength: maxLength(100) }
    }
  },
  methods: {
    onSubmit() {
      this.isSubmitted = true;

      if (this.v$.product.$invalid) {
        return;
      }

      const promise = this.id
        ? axios.put(this.id, this.product)
        : axios.post("", this.product);

      promise.then(rs => {
        const id = this.id ? this.id : rs.data.id;
        this.$router.push("/products/" + id);
      });
    }
  },
  created() {
    const id = this.$route.params.id as string;
    if (id) {
      axios.get(id).then(rs => {
        this.product = rs.data;
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
