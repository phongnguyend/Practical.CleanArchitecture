import { Component, OnInit } from "@angular/core";
import { UserManager, WebStorageStateStore, UserManagerSettings, User } from "oidc-client-ts";
import { ActivatedRoute } from "@angular/router";

import { environment } from "src/environments/environment";

@Component({
  template: "<div>Loading ...</div>",
})
export class OidcLoginRedirect implements OnInit {
  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    var config = {
      authority: environment.OpenIdConnect.Authority,
      client_id: environment.OpenIdConnect.ClientId,
      userStore: new WebStorageStateStore({ store: window.localStorage }),
      response_mode: "query",
    } as UserManagerSettings;

    var mgr = new UserManager(config);
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
}
