package com.classifiedads.mobile.features.configurationentries

import com.classifiedads.mobile.shared.model.Entity
import org.json.JSONObject

data class ConfigurationEntry(
    override val id: String = "",
    val key: String = "",
    val value: String = "",
    val description: String = "",
    val isSensitive: Boolean = false,
    val updatedDateTime: String = "",
) : Entity {
    fun toJson() =
        JSONObject()
            .put("id", id)
            .put("key", key)
            .put("value", value)
            .put("description", description)
            .put("isSensitive", isSensitive)
            .put("updatedDateTime", updatedDateTime)

    fun toPayload() =
        JSONObject()
            .put("key", key)
            .put("value", value)
            .put("description", description)
            .put("isSensitive", isSensitive)
            .apply { if (id.isNotEmpty()) put("id", id) }

    fun validate(): String? {
        if (key.isBlank()) return "Key is required."
        if (value.isBlank()) return "Value is required."
        return null
    }

    companion object {
        fun fromJson(json: JSONObject) =
            ConfigurationEntry(
                id = if (json.isNull("id")) "" else json.optString("id", ""),
                key = if (json.isNull("key")) "" else json.optString("key", ""),
                value = if (json.isNull("value")) "" else json.optString("value", ""),
                description =
                    if (json.isNull("description")) "" else json.optString("description", ""),
                isSensitive = json.optBoolean("isSensitive"),
                updatedDateTime =
                    if (json.isNull("updatedDateTime")) ""
                    else json.optString("updatedDateTime", ""),
            )
    }
}
