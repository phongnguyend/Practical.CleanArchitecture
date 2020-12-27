import { Component, OnInit } from "@angular/core";
import { Location } from "@angular/common";
import { UserManager, User, WebStorageStateStore } from "oidc-client";

@Component({
  template: "<div>Loading ...</div>",
})
export class OidcLoginRedirect implements OnInit {
  constructor(private location: Location) {}

  ngOnInit(): void {
    var config = {
      userStore: new WebStorageStateStore({ store: window.localStorage }),
      response_mode: "query",
    };
    var mgr = new UserManager(config);
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
}
