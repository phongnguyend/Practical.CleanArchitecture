<template>
  <div class="card" v-if="user">
    <div class="card-header">{{ "User Detail: " + user.userName }}</div>

    <div class="card-body">
      <div class="row">
        <div class="col-md-8">
          <div class="row">
            <div class="col-md-4">User Name:</div>
            <div class="col-md-8">{{ user.userName }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Email:</div>
            <div class="col-md-8">{{ user.email }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Email Confirmed:</div>
            <div class="col-md-8">{{ user.emailConfirmed }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Phone Number:</div>
            <div class="col-md-8">{{ user.phoneNumber }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Phone Number Confirmed:</div>
            <div class="col-md-8">{{ user.phoneNumberConfirmed }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Two Factor Enabled:</div>
            <div class="col-md-8">{{ user.twoFactorEnabled }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Lockout Enabled:</div>
            <div class="col-md-8">{{ user.lockoutEnabled }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Access Failed Count:</div>
            <div class="col-md-8">{{ user.accessFailedCount }}</div>
          </div>
          <div class="row">
            <div class="col-md-4">Lockout End:</div>
            <div class="col-md-8">{{ user.lockoutEnd }}</div>
          </div>
        </div>

        <div class="col-md-4">
          <img class="center-block img-responsive" :style="{ width: '200px', margin: '2px' }"
            :src="'/img/icons/android-chrome-192x192.png'" :title="user.userName" />
        </div>
      </div>
    </div>

    <div class="card-footer">
      <button class="btn btn-outline-secondary" @click="onBack" style="width: 80px">
        <i class="fa fa-chevron-left"></i> Back
      </button>
      &nbsp;
      <router-link class="btn btn-primary" :to="'/users/edit/' + user.id">Edit</router-link>&nbsp;
      <button type="button" class="btn btn-secondary" @click="setPasswordModal()">
        Set Password</button>&nbsp;
      <button type="button" class="btn btn-secondary" @click="sendPasswordResetEmailModal()">
        Send Password Reset Email</button>&nbsp;
      <button type="button" class="btn btn-secondary" @click="sendEmailAddressConfirmationEmailModal()">
        Send Email Address Confirmation Email
      </button>
    </div>

    <b-modal ref="modal-set-password" title="Set Password" :hide-footer="true">
      <div class="row alert alert-danger" v-show="passwordValidationErrors && passwordValidationErrors.length">
        <ul>
          <li v-for="error in passwordValidationErrors" :key="error.code">
            {{ error.description }}
          </li>
        </ul>
      </div>
      <div class="row alert alert-danger" v-show="postErrorMessage">
        {{ postErrorMessage }}
      </div>
      <form @submit.prevent="confirmSetPassword">
        <div class="form-group row">
          <label class="col-sm-4 col-form-label">User Name</label>
          <div class="col-sm-8">{{ user.userName }}</div>
        </div>
        <div class="form-group row">
          <label for="password" class="col-sm-4 col-form-label">Password</label>
          <div class="col-sm-8">
            <input type="password" id="password" name="password" class="form-control" required
              v-model="setPasswordModel.password" :class="{
                'is-invalid':
                  isSubmitted && v$.setPasswordModel.password.$invalid
              }" @input="v$.setPasswordModel.password.$touch()" />
            <span class="invalid-feedback">Enter a password</span>
          </div>
        </div>
        <div class="form-group row">
          <label for="confirmPassword" class="col-sm-4 col-form-label">Confirm Password</label>
          <div class="col-sm-8">
            <input type="password" id="confirmPassword" name="confirmPassword" class="form-control"
              v-model="setPasswordModel.confirmPassword" :class="{
                'is-invalid':
                  isSubmitted &&
                  setPasswordModel.confirmPassword != setPasswordModel.password
              }" />
            <span class="invalid-feedback">Confirm Password does not match</span>
          </div>
        </div>
        <div class="form-group row">
          <label class="col-sm-4 col-form-label"></label>
          <div class="col-sm-8">
            <button class="btn btn-primary">Save</button>
          </div>
        </div>
      </form>
    </b-modal>

    <b-modal ref="modal-send-password-reset-email" title="Send Password Reset Email"
      @ok="confirmSendPasswordResetEmail">
      <p>
        Are you sure you want to send reset password email
        <strong>{{ user.userName }}</strong>
      </p>
    </b-modal>

    <b-modal ref="modal-send-email-address-confirmation-email" title="Send Email Address Confirmation Email"
      @ok="confirmSendEmailAddressConfirmationEmail">
      <p>
        Are you sure you want to send email address confirmation email
        <strong>{{ user.userName }}</strong>
      </p>
    </b-modal>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import useVuelidate from "@vuelidate/core";
import { required } from "@vuelidate/validators";
import { BModal } from "bootstrap-vue";
import axios from "./axios";

import { IUser } from "./User";

export default defineComponent({
  setup() {
    return { v$: useVuelidate() }
  },
  data() {
    return {
      errorMessage: "",
      postErrorMessage: "",
      setPasswordModel: { password: null, confirmPassword: null },
      isSubmitted: false,
      passwordValidationErrors: [],
      user: {} as IUser
    };
  },
  validations: {
    setPasswordModel: {
      password: {
        required
      }
    }
  },
  methods: {
    onBack() {
      this.$router.push("/users");
    },
    setPasswordModal() {
      (this.$refs["modal-set-password"] as BModal).show();
    },
    confirmSetPassword() {
      this.isSubmitted = true;

      if (
        !this.setPasswordModel.password ||
        this.setPasswordModel.password != this.setPasswordModel.confirmPassword
      ) {
        return;
      }

      axios
        .put(this.user.id + "/password", {
          id: this.user.id,
          password: this.setPasswordModel.password
        })
        .then(rs => {
          this.setPasswordModel = { password: null, confirmPassword: null };
          this.isSubmitted = false;
          this.passwordValidationErrors = [];
          this.postErrorMessage = "";
          (this.$refs["modal-set-password"] as BModal).hide();
        })
        .catch(error => {
          if (error?.response?.status == 400) {
            this.passwordValidationErrors = error.response.data;
          } else {
            this.postErrorMessage = error?.response?.status;
          }
        });
    },
    sendPasswordResetEmailModal() {
      (this.$refs["modal-send-password-reset-email"] as BModal).show();
    },
    confirmSendPasswordResetEmail() {
      axios
        .post(this.user.id + "/passwordresetemail", {
          id: this.user.id
        })
        .then(rs => {
          (this.$refs["modal-send-password-reset-email"] as BModal).hide();
        });
    },
    sendEmailAddressConfirmationEmailModal() {
      (this.$refs["modal-send-email-address-confirmation-email"] as BModal).show();
    },
    confirmSendEmailAddressConfirmationEmail() {
      axios
        .post(this.user.id + "/emailaddressconfirmation", {
          id: this.user.id
        })
        .then(rs => {
          (this.$refs["modal-send-email-address-confirmation-email"] as BModal).hide();
        });
    }
  },
  components: {},
  created() {
    const id = this.$route.params.id as string;
    axios.get(id).then(rs => {
      this.user = rs.data;
    });
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
