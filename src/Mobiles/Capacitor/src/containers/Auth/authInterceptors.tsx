import { toast } from "../../components/Toast/toast";
import type { AxiosInstance } from "axios";
import { getAccessToken } from "./authService";
export default function addAuthInterceptors(client: AxiosInstance) {
  client.interceptors.request.use((config) => {
    const token = getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });
  client.interceptors.response.use(
    (response) => response,
    (error) => {
      toast.error(
        error.response?.status === 401
          ? "Session expired. Please sign in again."
          : "Request failed. Please try again.",
      );
      return Promise.reject(error);
    },
  );
}
