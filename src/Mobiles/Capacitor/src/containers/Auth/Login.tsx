import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Capacitor } from "@capacitor/core";
import { login } from "./authService";
export default function Login() {
  const [error, setError] = useState("");
  const navigate = useNavigate();
  useEffect(() => {
    void login()
      .then(() => {
        if (Capacitor.isNativePlatform()) {
          navigate("/home", { replace: true });
        }
      })
      .catch(() =>
        setError("Sign-in did not complete. Return Home and try again."),
      );
  }, [navigate]);
  return <p role="status">{error || "Opening sign-in..."}</p>;
}
