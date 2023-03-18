import { UserManager, User, WebStorageStateStore } from "oidc-client-ts";

import env from "../../environments";

class AuthService {
  _userManager: UserManager;
  _user?: User;

  constructor() {
    const config = {
      authority: env.OpenIdConnect.Authority,
      client_id: env.OpenIdConnect.ClientId,
      redirect_uri: `${env.CurrentUrl}oidc-login-redirect`,
      scope: "openid profile ClassifiedAds.WebAPI",
      response_type: "code",
      post_logout_redirect_uri: `${env.CurrentUrl}?postLogout=true`,
      userStore: new WebStorageStateStore({ store: window.localStorage }),
    };
    this._userManager = new UserManager(config);
  }

  loadUser = () => {
    const promise = this._userManager.getUser();
    promise.then((user) => {
      if (user && !user.expired) {
        this._user = user;
      }
    });
    return promise;
  };

  login = (returnUrl: string) => {
    //console.log("Return Url:", returnUrl);
    localStorage.setItem("returnUrl", returnUrl);
    return this._userManager.signinRedirect();
  };

  logout = () => {
    return this._userManager.signoutRedirect();
  };

  isLoggedIn = () => {
    return this._user && this._user.access_token && !this._user.expired;
  };

  getAccessToken = () => {
    return this._user ? this._user.access_token : "";
  };

  signoutRedirectCallback = () => {
    return this._userManager.signoutRedirectCallback();
  };

  getCurrentUser = () => {
    return {
      id: this._user?.profile.sub,
      userName: "phongnguyend",
      firstName: "Phong",
      lastName: "Nguyen",
    };
  };

  isAuthenticated = () => {
    return this.isLoggedIn();
  };
}

const instance = new AuthService();

export default instance;
