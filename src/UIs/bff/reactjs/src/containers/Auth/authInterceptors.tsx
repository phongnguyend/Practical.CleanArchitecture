import { login, logout } from "./authService";

const addAuthInterceptors = (axios) => {
  axios.interceptors.request.use((config) => {
    try {
      const xsrfToken = document
        .cookie!.split("; ")!
        .find((row) => row.startsWith("PHONG-XSRF-TOKEN="))!
        .split("=")[1];
      config.headers["X-XSRF-TOKEN"] = xsrfToken;
    } catch {}
    return config;
  });
  axios.interceptors.response.use(
    (response) => {
      return response;
    },
    (error) => {
      if (401 === error.response.status) {
        var urlObj = new URL(window.location.href);
        var newURL = urlObj.href.replace(urlObj.origin, "");
        logout();
        login(newURL);
      } else {
        return Promise.reject(error);
      }
    }
  );
};

export default addAuthInterceptors;
