import { AxiosInstance } from "axios";
import env from "../../environments";

const addAuthInterceptors = (axios: AxiosInstance) => {
  axios.interceptors.request.use((config) => {
    if (
      config.baseURL?.startsWith(env.ResourceServer.Endpoint) ||
      config.url?.startsWith(env.ResourceServer.Endpoint)
    ) {
      // TODO: handle CSRF token if needed
    }
    return config;
  });
  axios.interceptors.response.use(
    (response) => {
      return response;
    },
    (error) => {
      if (401 === error.response.status) {
        const returnUrl = window.location.href;
        window.location.href = `/login?returnUrl=${encodeURIComponent(
          returnUrl
        )}`;
      } else {
        return Promise.reject(error);
      }
    }
  );
};

export default addAuthInterceptors;
