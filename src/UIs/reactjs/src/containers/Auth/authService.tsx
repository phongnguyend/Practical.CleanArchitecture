import { User, UserManager, WebStorageStateStore } from "oidc-client-ts";

import env from "../../environments";

const config = {
  authority: env.OpenIdConnect.Authority,
  client_id: env.OpenIdConnect.ClientId,
  redirect_uri: `${env.CurrentUrl}oidc-login-redirect`,
  scope: "openid profile ClassifiedAds.WebAPI",
  response_type: "code",
  post_logout_redirect_uri: `${env.CurrentUrl}?postLogout=true`,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
};

const userManager = new UserManager(config);

var _user: User;

export const loadUser = () => {
  var promise = userManager.getUser();
  promise.then((user) => {
    if (user && !user.expired) {
      _user = user;
    }
  });
  return promise;
};

export const login = (returnUrl?: string) => {
  console.log("Return Url:", returnUrl);
  if (returnUrl) {
    localStorage.setItem("returnUrl", returnUrl);
  }
  return userManager.signinRedirect();
};

export const logout = () => {
  return userManager.signoutRedirect();
};

export const isLoggedIn = () => {
  return _user && _user.access_token && !_user.expired;
};

export const getAccessToken = () => {
  return _user ? _user.access_token : "";
};

export const signoutRedirectCallback = () => {
  return userManager.signoutRedirectCallback();
};

export const getCurrentUser = () => {
  return {
    id: _user?.profile.sub,
    userName: "phongnguyend",
    firstName: "Phong",
    lastName: "Nguyen",
  };
};

export const isAuthenticated = () => {
  return isLoggedIn();
};
