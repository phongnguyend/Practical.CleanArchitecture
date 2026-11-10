package com.classifiedads.mobile.features.roles

import com.classifiedads.mobile.shared.model.Entity
import org.json.JSONObject

data class Role(
    override val id: String = "",
    val name: String = "",
    val normalizedName: String = "",
) : Entity {
    fun toJson() =
        JSONObject().put("id", id).put("name", name).put("normalizedName", normalizedName)

    fun toPayload() = JSONObject().put("name", name).apply { if (id.isNotEmpty()) put("id", id) }

    fun validate(): String? {
        if (name.isBlank()) return "Name is required."
        return null
    }

    companion object {
        fun fromJson(json: JSONObject) =
            Role(
                id = if (json.isNull("id")) "" else json.optString("id", ""),
                name = if (json.isNull("name")) "" else json.optString("name", ""),
                normalizedName =
                    if (json.isNull("normalizedName")) "" else json.optString("normalizedName", ""),
            )
    }
}
