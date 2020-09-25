<template>
  <div class="card">
    <div class="card-header">{{title}}</div>
    <div class="card-body">
      <div class="alert alert-danger" v-show="postError">{{ postErrorMessage }}</div>
      <form @submit.prevent="onSubmit">
        <div class="form-group row">
          <label for="name" class="col-sm-2 col-form-label">Name</label>
          <div class="col-sm-10">
            <input
              id="name"
              name="name"
              class="form-control"
              v-model="file.name"
              :class="{'is-invalid': isSubmitted && $v.file.name.$invalid}"
              @input="$v.file.name.$touch()"
            />
            {{$v.name}}
            <span class="invalid-feedback">
              <span v-if="!$v.file.name.required">Enter a name</span>
              <span v-if="!$v.file.name.minLength">The name must be longer than 3 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="description" class="col-sm-2 col-form-label">Description</label>
          <div class="col-sm-10">
            <input
              id="description"
              name="description"
              class="form-control"
              v-model="file.description"
              :class=" {'is-invalid': isSubmitted && $v.file.description.$invalid}"
              @input="$v.file.description.$touch()"
            />
            <span class="invalid-feedback">
              <span v-if="!$v.file.description.required">Enter a description</span>
              <span v-if="!$v.file.description.maxLength">The code must be less than 100 characters.</span>
            </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="formFile" class="col-sm-2 col-form-label">File</label>
          <div class="col-sm-10">
            <input
              type="file"
              id="formFile"
              name="formFile"
              class="form-control"
              :class=" {'is-invalid': isSubmitted && !hasFile}"
              @change="handleFileInput($event.target.files)"
            />
            <span class="invalid-feedback">
              <span>Select a file</span>
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
      <router-link class="btn btn-outline-secondary" to="/files" style="width:80px">
        <i class="fa fa-chevron-left"></i> Back
      </router-link>
    </div>
  </div>
</template>
<script>
import { required, minLength, maxLength } from "vuelidate/lib/validators";

import axios from "./axios";

export default {
  data() {
    return {
      file: { name: "", description: "" },
      postError: false,
      postErrorMessage: "",
      isSubmitted: false,
      hasFile: false,
    };
  },
  computed: {
    title() {
      return "Upload File";
    },
    id() {
      return this.$route.params.id;
    },
  },
  validations: {
    file: {
      name: {
        required,
        minLength: minLength(3),
      },
      description: { required, maxLength: maxLength(100) },
    },
  },
  methods: {
    handleFileInput(files) {
      this.file.formFile = files.item(0);
      this.hasFile = !!this.file.formFile;
    },
    onSubmit() {
      this.isSubmitted = true;
      if (this.$v.file.$invalid || !this.hasFile) {
        return;
      }
      const formData = new FormData();
      formData.append("formFile", this.file.formFile);
      formData.append("name", this.file.name);
      formData.append("description", this.file.description);
      const promise = axios.post("", formData);

      promise.then((rs) => {
        const id = rs.data.id;
        this.$router.push("/files/edit/" + id);
      });
    },
  },
  created() {},
};
</script>
<style scoped>
.field-error {
  border: 1px solid red;
}

.col-form-label {
  text-align: right;
}
</style>