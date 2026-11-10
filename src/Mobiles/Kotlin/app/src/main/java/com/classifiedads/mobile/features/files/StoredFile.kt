package com.classifiedads.mobile.features.files

import com.classifiedads.mobile.shared.model.Entity
import org.json.JSONObject

data class StoredFile(
    override val id: String = "",
    val name: String = "",
    val description: String = "",
    val encrypted: Boolean = false,
    val fileName: String = "",
    val size: Long = 0L,
    val uploadedTime: String = "",
    val fileLocation: String = "",
) : Entity {
    fun toJson() =
        JSONObject()
            .put("id", id)
            .put("name", name)
            .put("description", description)
            .put("encrypted", encrypted)
            .put("fileName", fileName)
            .put("size", size)
            .put("uploadedTime", uploadedTime)
            .put("fileLocation", fileLocation)

    fun toPayload() =
        JSONObject()
            .put("name", name)
            .put("description", description)
            .put("encrypted", encrypted)
            .apply {
                if (id.isNotEmpty()) {
                    put("id", id)
                    put("fileName", fileName)
                }
            }

    fun validate(): String? {
        if (name.isBlank()) return "Name is required."
        if (name.trim().length < 3) return "Name needs at least 3 characters."
        if (description.isBlank()) return "Description is required."
        if (description.length > 100) return "Description must be at most 100 characters."
        return null
    }

    companion object {
        fun fromJson(json: JSONObject) =
            StoredFile(
                id = if (json.isNull("id")) "" else json.optString("id", ""),
                name = if (json.isNull("name")) "" else json.optString("name", ""),
                description =
                    if (json.isNull("description")) "" else json.optString("description", ""),
                encrypted = json.optBoolean("encrypted"),
                fileName = if (json.isNull("fileName")) "" else json.optString("fileName", ""),
                size = json.optLong("size"),
                uploadedTime =
                    if (json.isNull("uploadedTime")) "" else json.optString("uploadedTime", ""),
                fileLocation =
                    if (json.isNull("fileLocation")) "" else json.optString("fileLocation", ""),
            )
    }
}
