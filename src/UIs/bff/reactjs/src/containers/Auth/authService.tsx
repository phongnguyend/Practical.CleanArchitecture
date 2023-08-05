import axios from "axios";

export const login = (returnUrl?: string) => {
  console.log("Return Url:", returnUrl);
  window.location.href = "/login?returnUrl=" + returnUrl;
};

export const logout = () => {
  localStorage.removeItem("userinfor");
};

export const getCurrentUser = () => {
  const userinfor = localStorage.getItem("userinfor");
  return JSON.parse(userinfor!);
};

export const setCurrentUser = (userinfor) => {
  localStorage.setItem("userinfor", JSON.stringify(userinfor));
};

export const isAuthenticated = () => {
  const userinfor = localStorage.getItem("userinfor");
  return !!userinfor;
};

export const loadUser = (): Promise<any> => {
  return axios
    .get("/userinfor")
    .then(function (response) {
      setCurrentUser(response.data);
      return Promise.resolve(getCurrentUser());
    })
    .catch(function (error) {
      return Promise.resolve(null);
    })
    .finally(function () {});
};
