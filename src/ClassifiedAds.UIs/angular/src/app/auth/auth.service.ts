import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { UserManager, User, WebStorageStateStore } from "oidc-client";

import { IUser } from "./user.model";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  private _userManager: UserManager;
  private _user: User;

  constructor(private httpClient: HttpClient) {
    var config = {
      authority: environment.OpenIdConnect.Authority,
      client_id: environment.OpenIdConnect.ClientId,
      redirect_uri: `${environment.CurrentUrl}assets/oidc-login-redirect.html`,
      scope: "openid profile ClassifiedAds.WebAPI",
      response_type: "id_token token",
      post_logout_redirect_uri: `${environment.CurrentUrl}?postLogout=true`,
      userStore: new WebStorageStateStore({ store: window.localStorage })
    };
    this._userManager = new UserManager(config);
  }

  loadUser() {
    var promise = this._userManager.getUser();
    promise.then(user => {
      if (user && !user.expired) {
        this._user = user;
      }
    });
    return promise;
  }

  login(returnUrl: string): Promise<any> {
    console.log("Return Url:", returnUrl);
    return this._userManager.signinRedirect();
  }

  logout(): Promise<any> {
    return this._userManager.signoutRedirect();
  }

  isLoggedIn(): boolean {
    return this._user && this._user.access_token && !this._user.expired;
  }

  getAccessToken(): string {
    return this._user ? this._user.access_token : "";
  }

  signoutRedirectCallback(): Promise<any> {
    return this._userManager.signoutRedirectCallback();
  }

  getCurrentUser(): IUser {
    return {
      id: this._user.profile.sub,
      userName: "phongnguyend",
      firstName: "Phong",
      lastName: "Nguyen"
    };
  }

  isAuthenticated() {
    return this.isLoggedIn();
  }

  updateCurrentUser(firstName: string, lastName: string) {}
}
