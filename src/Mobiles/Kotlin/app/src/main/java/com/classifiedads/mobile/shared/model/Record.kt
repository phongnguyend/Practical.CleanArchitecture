package com.classifiedads.mobile.shared.model

import org.json.JSONArray
import org.json.JSONObject

fun JSONArray.objects() = (0 until length()).map { getJSONObject(it) }

fun auditSnapshot(row: JSONObject): JSONObject =
    row.optJSONObject("data")
        ?: runCatching { JSONObject(row.optString("log")) }.getOrDefault(JSONObject())

fun apiError(status: Int, text: String): String {
    if (status == 401) return "Session expired. Please sign out and sign in again."
    if (status == 403) return "You do not have permission for this action."
    val body = runCatching { JSONObject(text) }.getOrNull()
    if (body != null) {
        if (!body.isNull("errorMessage")) return body.getString("errorMessage")
        body.optJSONObject("errors")?.let { errors ->
            return errors.keys().asSequence().joinToString("\n") { key ->
                val values = errors.optJSONArray(key)
                if (values == null) errors.optString(key)
                else (0 until values.length()).joinToString("\n") { values.optString(it) }
            }
        }
        if (!body.isNull("detail")) return body.getString("detail")
        if (!body.isNull("title")) return body.getString("title")
    }
    return "Request failed ($status): ${text.take(1500)}"
}
