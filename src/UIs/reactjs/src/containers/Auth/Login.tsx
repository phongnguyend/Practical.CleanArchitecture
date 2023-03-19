import { useEffect } from "react";
import { login } from "../../containers/Auth/authService";

const Login = () => {
  useEffect(() => {
    login();
  }, []);

  return <div>Logging In ...</div>;
};

export default Login;
