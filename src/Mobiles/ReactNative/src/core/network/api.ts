import { apiError } from "../../shared/models/record";
import { accessToken } from "../auth/session";
import { config } from "../config/environment";

export async function response(path: string, method = "GET", body?: any) {
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 60000);
  try {
    const result = await fetch(config.api + path.replace(/^\//, ""), {
      method,
      signal: controller.signal,
      headers: {
        Authorization: `Bearer ${await accessToken()}`,
        ...(body && !(body instanceof FormData)
          ? { "Content-Type": "application/json" }
          : {}),
      },
      body:
        body instanceof FormData
          ? body
          : body
            ? JSON.stringify(body)
            : undefined,
    });
    if (!result.ok) {
      const text = await result.text();
      throw new Error(apiError(result.status, text));
    }
    return result;
  } finally {
    clearTimeout(timeout);
  }
}
export async function api(path: string, method = "GET", body?: any) {
  const text = await (await response(path, method, body)).text();
  return text ? JSON.parse(text) : null;
}
