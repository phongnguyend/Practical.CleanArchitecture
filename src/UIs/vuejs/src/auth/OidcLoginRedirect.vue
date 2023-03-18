<template>
  <div>Loading ...</div>
</template>

<script>
import { UserManager, User, WebStorageStateStore } from "oidc-client-ts";

import env from "../../environments";

var config = {
  authority: env.OpenIdConnect.Authority,
  client_id: env.OpenIdConnect.ClientId,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  response_mode: "query"
};
var mgr = new UserManager(config);
export default {
  created() {
    mgr.signinRedirectCallback().then(
      () => {
        window.history.replaceState(
          {},
          window.document.title,
          window.location.origin
        );

        const returnUrl = localStorage.getItem("returnUrl");
        if (returnUrl) {
          localStorage.removeItem("returnUrl");
          window.location = returnUrl;
        } else {
          window.location = "/";
        }
      },
      error => {
        //console.error(error);
      }
    );
  }
};
</script>
