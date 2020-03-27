import { UserManager, User, WebStorageStateStore } from "oidc-client";

export default class AuthService {
  constructor() {
    var config = {
      authority: "https://localhost:44367",
      client_id: "ClassifiedAds.React",
      redirect_uri: `http://localhost:3000/oidc-login-redirect.html`,
      scope: "openid profile ClassifiedAds.WebAPI",
      response_type: "id_token token",
      post_logout_redirect_uri: `http://localhost:3000/?postLogout=true`,
      userStore: new WebStorageStateStore({ store: window.localStorage })
    };
    this._userManager = new UserManager(config);
  }

  loadUser = () => {
    var promise = this._userManager.getUser();
    promise.then(user => {
      if (user && !user.expired) {
        this._user = user;
      }
    });
    return promise;
  };

  login = returnUrl => {
    console.log("Return Url:", returnUrl);
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
      id: this._user.profile.sub,
      userName: "phongnguyend",
      firstName: "Phong",
      lastName: "Nguyen"
    };
  };

  isAuthenticated = () => {
    return this.isLoggedIn();
  };
}
