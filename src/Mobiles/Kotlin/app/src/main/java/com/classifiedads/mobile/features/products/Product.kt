package com.classifiedads.mobile.features.products

import com.classifiedads.mobile.shared.model.Entity
import org.json.JSONObject

data class Product(
    override val id: String = "",
    val name: String = "",
    val code: String = "",
    val description: String = "",
    val price: Double = 0.0,
    val starRating: Double = 0.0,
    val imageUrl: String = "",
) : Entity {
    fun toJson() =
        JSONObject()
            .put("id", id)
            .put("name", name)
            .put("code", code)
            .put("description", description)
            .put("price", price)
            .put("starRating", starRating)
            .put("imageUrl", imageUrl)

    fun toPayload() =
        JSONObject().put("name", name).put("code", code).put("description", description).apply {
            if (id.isNotEmpty()) put("id", id)
        }

    fun validate(): String? {
        if (name.isBlank()) return "Name is required."
        if (name.trim().length < 3) return "Name needs at least 3 characters."
        if (code.isBlank()) return "Code is required."
        if (code.length > 10) return "Code must be at most 10 characters."
        if (description.isBlank()) return "Description is required."
        if (description.length > 100) return "Description must be at most 100 characters."
        return null
    }

    companion object {
        fun fromJson(json: JSONObject) =
            Product(
                id = if (json.isNull("id")) "" else json.optString("id", ""),
                name = if (json.isNull("name")) "" else json.optString("name", ""),
                code = if (json.isNull("code")) "" else json.optString("code", ""),
                description =
                    if (json.isNull("description")) "" else json.optString("description", ""),
                price = json.optDouble("price", 0.0),
                starRating = json.optDouble("starRating", 0.0),
                imageUrl = if (json.isNull("imageUrl")) "" else json.optString("imageUrl", ""),
            )
    }
}
