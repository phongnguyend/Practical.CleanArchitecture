<template>
  <div class="card">
    <div class="card-header">{{ title }}</div>
    <div class="card-body">
      <div class="alert alert-danger" v-show="postError">
        {{ postErrorMessage }}
      </div>
      <form @submit.prevent="onSubmit">
        <div class="mb-3 row">
          <label for="name" class="col-sm-2 col-form-label">Name</label>
          <div class="col-sm-10">
            <input
              id="name"
              name="name"
              class="form-control"
              v-model="product.name"
              :class="{ 'is-invalid': isSubmitted && v$.product.name.$invalid }"
              @input="v$.product.name.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="v$.product.name.required.$invalid">Enter a name</span>
              <span v-if="v$.product.name.minLength.$invalid"
                >The name must be longer than 3 characters.</span
              >
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="code" class="col-sm-2 col-form-label">Code</label>
          <div class="col-sm-10">
            <input
              id="code"
              name="code"
              class="form-control"
              v-model="product.code"
              :class="{ 'is-invalid': isSubmitted && v$.product.code.$invalid }"
              @input="v$.product.code.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="v$.product.code.required.$invalid">Enter a code</span>
              <span v-if="v$.product.code.maxLength.$invalid"
                >The code must be less than 10 characters.</span
              >
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="description" class="col-sm-2 col-form-label">Description</label>
          <div class="col-sm-10">
            <input
              id="description"
              name="description"
              class="form-control"
              v-model="product.description"
              :class="{
                'is-invalid': isSubmitted && v$.product.description.$invalid,
              }"
              @input="v$.product.description.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="v$.product.description.required.$invalid">Enter a description</span>
              <span v-if="v$.product.description.maxLength.$invalid"
                >The code must be less than 100 characters.</span
              >
            </span>
          </div>
        </div>
        <div class="mb-3 row">
          <label for="description" class="col-sm-2 col-form-label"></label>
          <div class="col-sm-10">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </div>
    <div class="card-footer">
      <router-link class="btn btn-outline-secondary" to="/products" style="width: 80px">
        <i class="fa fa-chevron-left"></i> Back
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import useVuelidate from '@vuelidate/core'
import { required, minLength, maxLength } from '@vuelidate/validators'
import axios from './axios'
import type { IProduct } from './Product'

const route = useRoute()
const router = useRouter()

const product = ref<IProduct>({ name: '', code: '', description: '' } as IProduct)
const postError = ref(false)
const postErrorMessage = ref('')
const isSubmitted = ref(false)

const title = computed(() => {
  return route.params.id ? 'Edit Product' : 'Add Product'
})

const id = computed(() => {
  return route.params.id as string
})

const rules = {
  product: {
    name: {
      required,
      minLength: minLength(3),
    },
    code: {
      required,
      maxLength: maxLength(10),
    },
    description: { 
      required, 
      maxLength: maxLength(100) 
    },
  },
}

const v$ = useVuelidate(rules, { product })

const onSubmit = () => {
  isSubmitted.value = true

  if (v$.value.product.$invalid) {
    return
  }

  const promise = id.value 
    ? axios.put(id.value, product.value) 
    : axios.post('', product.value)

  promise.then((rs) => {
    const productId = id.value ? id.value : rs.data.id
    router.push('/products/' + productId)
  })
}

onMounted(() => {
  const productId = route.params.id as string
  if (productId) {
    axios.get(productId).then((rs) => {
      product.value = rs.data
    })
  }
})
</script>

<style scoped>
.field-error {
  border: 1px solid red;
}

.col-form-label {
  text-align: right;
}
</style>
