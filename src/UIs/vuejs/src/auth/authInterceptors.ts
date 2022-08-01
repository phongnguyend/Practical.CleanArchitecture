import { AxiosInstance } from "axios";

import env from "../../environments";
import authService from "./authService";

const addAuthInterceptors = (axios: AxiosInstance) => {
  {
    axios.interceptors.request.use(config => {
      if (
        config.baseURL?.startsWith(env.ResourceServer.Endpoint) ||
        config.url?.startsWith(env.ResourceServer.Endpoint)
      ) {
        config.headers!["Authorization"] =
          "Bearer " + authService.getAccessToken();
      }
      return config;
    });
    axios.interceptors.response.use(
      response => {
        return response;
      },
      error => {
        if (401 === error.response.status) {
          authService.login(window.location.href);
        } else {
          return Promise.reject(error);
        }
      }
    );
  }
};

export default addAuthInterceptors;
