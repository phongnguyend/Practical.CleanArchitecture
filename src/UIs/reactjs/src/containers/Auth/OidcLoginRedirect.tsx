import { UserManager, User, WebStorageStateStore } from "oidc-client";
import React, { Component } from "react";

var config = {
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  response_mode: "query",
};
var mgr = new UserManager(config);

class OidcLoginRedirect extends Component {
  componentDidMount() {
    mgr.signinRedirectCallback().then(
      () => {
        window.history.replaceState(
          {},
          window.document.title,
          window.location.origin
        );

        let returnUrl = localStorage.getItem("returnUrl");
        if (returnUrl) {
          localStorage.removeItem("returnUrl");
        } else {
          returnUrl = "/";
        }
        window.location.href = returnUrl;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  render() {
    return <div>Loading ...</div>;
  }
}

export default OidcLoginRedirect;
