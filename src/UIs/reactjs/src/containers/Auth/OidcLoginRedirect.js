import { UserManager, User, WebStorageStateStore } from "oidc-client";
import React, { Component } from "react";

var config = {
  userStore: new WebStorageStateStore({ store: window.localStorage })
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

        const returnUrl = localStorage.getItem("returnUrl");
        if (returnUrl) {
          localStorage.removeItem("returnUrl");
          window.location = returnUrl;
        } else {
          window.location = "/";
        }
      },
      error => {
        console.error(error);
      }
    );
  }

  render() {
    return (<div>Loading ...</div>);
  }
}

export default OidcLoginRedirect;