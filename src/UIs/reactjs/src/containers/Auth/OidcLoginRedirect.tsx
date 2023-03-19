import { UserManager, User, WebStorageStateStore, UserManagerSettings } from "oidc-client-ts";
import { useEffect } from "react";
import { useDispatch } from "react-redux";

import env from "../../environments";

var config = {
  authority: env.OpenIdConnect.Authority,
  client_id: env.OpenIdConnect.ClientId,
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  response_mode: "query",
} as UserManagerSettings;

var userManager = new UserManager(config);

const OidcLoginRedirect = () => {
  const dispatch = useDispatch();

  useEffect(() => {
    userManager.signinRedirectCallback().then(
      (user: User) => {
        dispatch({
          type: "LOGIN",
          user: user,
        });

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
  }, []);

  return <div>Loading ...</div>;
};

export default OidcLoginRedirect;
