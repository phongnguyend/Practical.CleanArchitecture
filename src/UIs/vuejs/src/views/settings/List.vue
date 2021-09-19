<template>
  <div class="card">
    <div class="card-header">
      Settings
      <div style="float: right">
        <button
          type="button"
          class="btn btn-secondary"
          @click="exportAsExcel()"
        >
          Export as Excel
        </button>
        &nbsp;
        <button class="btn btn-primary" @click="addEntry()">
          Add
        </button>
        &nbsp;
        <button class="btn btn-primary" @click="openImportExcelModal()">
          Import Excel
        </button>
      </div>
    </div>
    <div class="card-body">
      <div class="table-responsive">
        <table
          class="table"
          v-if="configurationEntries && configurationEntries.length"
        >
          <thead>
            <tr>
              <th>Key</th>
              <th>Value</th>
              <th>Description</th>
              <th>Updated Time</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="entry in configurationEntries" :key="entry.id">
              <td>{{ entry.key }}</td>
              <td>{{ entry.isSensitive ? "******" : entry.value }}</td>
              <td>{{ entry.description }}</td>
              <td>{{ entry.updatedDateTime | formatedDateTime }}</td>
              <td>
                <button class="btn btn-primary" @click="updateEntry(entry)">
                  Edit
                </button>
                &nbsp;
                <button
                  type="button"
                  class="btn btn-primary btn-danger"
                  @click="deleteEntry(entry)"
                >
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
    <b-modal id="modal-delete" title="Delete Entry" @ok="deleteConfirmed">
      <p class="my-4">
        Are you sure you want to delete:
        <strong>{{ selectedEntry.key }}</strong>
      </p>
    </b-modal>
    <b-modal id="modal-add-update" hide-footer size="lg" v-bind:title="title">
      <form @submit.prevent="confirmAddUpdate">
        <div class="form-group row">
          <label for="key" class="col-sm-3 col-form-label">Key</label>
          <div class="col-sm-9">
            <input
              id="key"
              name="key"
              class="form-control"
              v-model="selectedEntry.key"
              @input="$v.selectedEntry.key.$touch()"
              :class="{
                'is-invalid': isSubmitted && $v.selectedEntry.key.$invalid
              }"
            />
            <span class="invalid-feedback"> Enter a key </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="value" class="col-sm-3 col-form-label">Value</label>
          <div class="col-sm-9">
            <input
              id="value"
              name="value"
              class="form-control"
              v-model="selectedEntry.value"
              @input="$v.selectedEntry.value.$touch()"
              :class="{
                'is-invalid': isSubmitted && $v.selectedEntry.value.$invalid
              }"
            />
            <span class="invalid-feedback"> Enter a value </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="description" class="col-sm-3 col-form-label"
            >Description</label
          >
          <div class="col-sm-9">
            <input
              id="description"
              name="description"
              class="form-control"
              v-model="selectedEntry.description"
            />
            <span class="invalid-feedback"> Enter a description </span>
          </div>
        </div>
        <div class="form-group row">
          <label for="sensitive" class="col-sm-3 col-form-label"
            >Sensitive</label
          >
          <div class="col-sm-9">
            <input
              type="checkbox"
              id="sensitive"
              name="sensitive"
              v-model="selectedEntry.isSensitive"
            />
          </div>
        </div>
        <div class="form-group row">
          <label class="col-sm-3 col-form-label"></label>
          <div class="col-sm-9">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </b-modal>
    <b-modal id="modal-import-excel" hide-footer title="Import Excel">
      <form @submit.prevent="confirmImportExcelFile">
        <div class="form-group row">
          <div class="col-sm-12">
            <input
              id="importingFile"
              type="file"
              name="importingFile"
              class="form-control"
              :class="{
                'is-invalid': isImportExcelFormSubmitted && !importingFile
              }"
              @change="handleFileInput($event.target.files)"
            />
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
import Vue from "vue";
import { required } from "vuelidate/lib/validators";
import axios from "./axios";

import { IConfigurationEntry } from "./ConfigurationEntry";

export default Vue.extend({
  data() {
    return {
      configurationEntries: [] as IConfigurationEntry[],
      selectedEntry: {} as IConfigurationEntry,
      isSubmitted: false,
      isImportExcelFormSubmitted: false,
      importingFile: null as File | null,
      errorMessage: ""
    };
  },
  validations: {
    selectedEntry: {
      key: {
        required
      },
      value: {
        required
      }
    }
  },
  computed: {
    title() {
      return this.$data.selectedEntry.id ==
        "00000000-0000-0000-0000-000000000000"
        ? "Add"
        : "Update";
    }
  },
  methods: {
    loadConfigurationEntries() {
      axios.get("").then(rs => {
        this.configurationEntries = rs.data;
      });
    },
    deleteEntry(entry: IConfigurationEntry) {
      this.selectedEntry = entry;
      this.$bvModal.show("modal-delete");
    },
    deleteConfirmed() {
      axios.delete(this.selectedEntry.id).then(rs => {
        this.loadConfigurationEntries();
      });
    },
    addEntry() {
      this.selectedEntry = {
        id: "00000000-0000-0000-0000-000000000000",
        key: "",
        value: "",
        description: "",
        isSensitive: false,
        createdDateTime: new Date()
      };
      this.isSubmitted = false;
      this.$bvModal.show("modal-add-update");
    },
    updateEntry(entry: IConfigurationEntry) {
      this.selectedEntry = {
        id: entry.id,
        key: entry.key,
        value: entry.isSensitive ? "" : entry.value,
        description: entry.description,
        isSensitive: entry.isSensitive,
        createdDateTime: new Date()
      };
      this.isSubmitted = false;
      this.$bvModal.show("modal-add-update");
    },
    confirmAddUpdate() {
      this.isSubmitted = true;
      if (this.$v.selectedEntry.$invalid) {
        return;
      }

      const promise =
        this.selectedEntry.id != "00000000-0000-0000-0000-000000000000"
          ? axios.put(this.selectedEntry.id, this.selectedEntry)
          : axios.post("", this.selectedEntry);

      promise.then(rs => {
        this.isSubmitted = false;
        this.$bvModal.hide("modal-add-update");
        this.loadConfigurationEntries();
      });
    },
    async exportAsExcel() {
      const rs = await axios.get("/ExportAsExcel", { responseType: "blob" });
      const url = window.URL.createObjectURL(rs.data);
      const element = document.createElement("a");
      element.href = url;
      element.download = "Settings.xlsx";
      document.body.appendChild(element);
      element.click();
    },
    openImportExcelModal() {
      this.isImportExcelFormSubmitted = false;
      this.importingFile = null;
      this.$bvModal.show("modal-import-excel");
    },
    handleFileInput(files: FileList) {
      this.importingFile = files.item(0);
    },
    async confirmImportExcelFile() {
      this.isImportExcelFormSubmitted = true;
      if (!this.importingFile) {
        return;
      }
      const formData = new FormData();
      formData.append("formFile", this.importingFile);
      const rs = await axios.post("ImportExcel", formData);
      this.isImportExcelFormSubmitted = false;
      this.$bvModal.hide("modal-import-excel");
      this.loadConfigurationEntries();
    }
  },
  components: {},
  filters: {
    formatedDateTime: function(value: string) {
      if (!value) return value;
      var date = new Date(value);
      return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    }
  },
  created() {
    this.loadConfigurationEntries();
  }
});
</script>

<style scoped>
thead {
  color: #337ab7;
}
.col-form-label {
  text-align: right;
}
</style>
