import { useEffect } from "react";

import { logout } from "../../containers/Auth/authService";

const Logout = () => {
  useEffect(() => {
    logout();
  }, []);

  return <div>Logging Out ...</div>;
};

export default Logout;
