import axios from "axios"

import env from "../../../environments";

const instance = axios.create({
    baseURL: env.ResourceServer.Endpoint + "products/",
});

axios.defaultInterceptors.requests.forEach(interceptor => {
    instance.interceptors.request.use(interceptor.f1, interceptor.f2);
})

axios.defaultInterceptors.responses.forEach(interceptor => {
    instance.interceptors.response.use(interceptor.f1, interceptor.f2);
})
export default instance