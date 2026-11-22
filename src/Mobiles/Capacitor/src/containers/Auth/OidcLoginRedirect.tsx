import { useEffect, useState } from "react";
import { userManager } from "./authService";
import { safeReturnPath } from "../../core/auth/callback";
let pending: Promise<unknown> | undefined;
export default function OidcLoginRedirect() {
  const [error, setError] = useState("");
  useEffect(() => {
    pending ??= userManager.signinRedirectCallback();
    void pending
      .then(() => {
        const target = safeReturnPath(sessionStorage.getItem("returnUrl"));
        sessionStorage.removeItem("returnUrl");
        window.location.replace(target);
      })
      .catch(() => setError("Sign-in failed. Return Home and try again."));
  }, []);
  return <p role="status">{error || "Completing sign-in..."}</p>;
}
