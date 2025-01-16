"use client";

import {
  UserManager,
  User,
  WebStorageStateStore,
  UserManagerSettings,
} from "oidc-client-ts";
import { useEffect } from "react";

import env from "../../environments";

const OidcLoginRedirect = () => {
  useEffect(() => {
    const config = {
      authority: env.OpenIdConnect.Authority,
      client_id: env.OpenIdConnect.ClientId,
      userStore: new WebStorageStateStore({ store: localStorage }),
      response_mode: "query",
    } as UserManagerSettings;

    const userManager = new UserManager(config);

    userManager.signinRedirectCallback().then(
      (user: User) => {
        localStorage.setItem("access_token", user.access_token);

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
  }, []);

  return <div>Loading ...</div>;
};

export default OidcLoginRedirect;
