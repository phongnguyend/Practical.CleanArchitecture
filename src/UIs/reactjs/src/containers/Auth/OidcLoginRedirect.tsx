import { UserManager, User, WebStorageStateStore, UserManagerSettings } from "oidc-client-ts";
import { Component } from "react";

import env from "../../environments";

var config = {
  authority: env.OpenIdConnect.Authority,
  client_id: env.OpenIdConnect.ClientId,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  response_mode: "query",
} as UserManagerSettings;

var mgr = new UserManager(config);

class OidcLoginRedirect extends Component {
  componentDidMount() {
    mgr.signinRedirectCallback().then(
      (user: User) => {
        window.history.replaceState({}, window.document.title, window.location.origin);

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
