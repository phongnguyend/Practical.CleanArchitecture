import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { logout } from "./authService";
export default function Logout() {
  const navigate = useNavigate();
  const [error, setError] = useState("");
  useEffect(() => {
    void logout()
      .then(() => navigate("/home", { replace: true }))
      .catch(() =>
        setError(
          "Signed out locally. The browser session may still be active.",
        ),
      );
  }, [navigate]);
  return <p role="status">{error || "Signing out..."}</p>;
}
