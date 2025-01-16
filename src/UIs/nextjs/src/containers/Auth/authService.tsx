"use client";

import { UserManager, WebStorageStateStore } from "oidc-client-ts";

import env from "../../environments";

const getUserManager = () => {
  const config = {
    authority: env.OpenIdConnect.Authority,
    client_id: env.OpenIdConnect.ClientId,
    redirect_uri: `${env.CurrentUrl}oidc-login-redirect`,
    scope: "openid profile ClassifiedAds.WebAPI",
    response_type: "code",
    post_logout_redirect_uri: `${env.CurrentUrl}?postLogout=true`,
    userStore: new WebStorageStateStore({ store: localStorage }),
  };

  return new UserManager(config);
};

export const login = (returnUrl?: string) => {
  console.log("Return Url:", returnUrl);
  if (returnUrl) {
    localStorage.setItem("returnUrl", returnUrl);
  }
  return getUserManager().signinRedirect();
};

export const logout = () => {
  localStorage.removeItem("access_token");
  return getUserManager().signoutRedirect();
};

export const getAccessToken = () => {
  return localStorage.getItem("access_token");
};

export const signoutRedirectCallback = () => {
  return getUserManager().signoutRedirectCallback();
};
