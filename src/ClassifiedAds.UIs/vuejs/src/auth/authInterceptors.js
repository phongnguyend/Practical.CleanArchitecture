import env from "../../environments";
import authService from "./authService"

const interceptors = {
    requests: [
        {
            f1: config => {
                if (config.baseURL?.startsWith(env.ResourceServer.Endpoint) || config.url?.startsWith(env.ResourceServer.Endpoint)) {
                    config.headers["Authorization"] = "Bearer " + authService.getAccessToken();
                }
                return config;
            }
        }],
    responses: [
        {
            f1: response => {
                return response;
            },
            f2: error => {
                if (401 === error.response.status) {
                    authService.login(window.location.href);
                } else {
                    return Promise.reject(error);
                }
            }
        }
    ]
}


const addAuthInterceptors = (axios) => {
    {
        interceptors.requests.forEach(interceptor => {
            axios.interceptors.request.use(interceptor.f1, interceptor.f2);
        })

        interceptors.responses.forEach(interceptor => {
            axios.interceptors.response.use(interceptor.f1, interceptor.f2);
        })
    }
}

export default addAuthInterceptors

