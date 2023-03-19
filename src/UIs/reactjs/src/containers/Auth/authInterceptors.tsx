import env from "../../environments";
import { getAccessToken, login } from "./authService";

const addAuthInterceptors = (axios) => {
  axios.interceptors.request.use((config) => {
    if (
      config.baseURL?.startsWith(env.ResourceServer.Endpoint) ||
      config.url?.startsWith(env.ResourceServer.Endpoint)
    ) {
      config.headers["Authorization"] = "Bearer " + getAccessToken();
    }
    return config;
  });
  axios.interceptors.response.use(
    (response) => {
      return response;
    },
    (error) => {
      if (401 === error.response.status) {
        login(window.location.href);
      } else {
        return Promise.reject(error);
      }
    }
  );
};

export default addAuthInterceptors;
