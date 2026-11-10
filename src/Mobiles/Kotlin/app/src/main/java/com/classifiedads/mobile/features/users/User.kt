package com.classifiedads.mobile.features.users

import com.classifiedads.mobile.shared.model.Entity
import org.json.JSONObject

data class User(
    override val id: String = "",
    val userName: String = "",
    val email: String = "",
    val emailConfirmed: Boolean = false,
    val phoneNumber: String = "",
    val phoneNumberConfirmed: Boolean = false,
    val twoFactorEnabled: Boolean = false,
    val lockoutEnabled: Boolean = false,
    val lockoutEnd: String = "",
    val accessFailedCount: Int = 0,
) : Entity {
    fun toJson() =
        JSONObject()
            .put("id", id)
            .put("userName", userName)
            .put("email", email)
            .put("emailConfirmed", emailConfirmed)
            .put("phoneNumber", phoneNumber)
            .put("phoneNumberConfirmed", phoneNumberConfirmed)
            .put("twoFactorEnabled", twoFactorEnabled)
            .put("lockoutEnabled", lockoutEnabled)
            .put("lockoutEnd", lockoutEnd)
            .put("accessFailedCount", accessFailedCount)

    fun toPayload() =
        JSONObject()
            .put("userName", userName)
            .put("email", email.trim())
            .put("emailConfirmed", emailConfirmed)
            .put("phoneNumber", phoneNumber)
            .put("phoneNumberConfirmed", phoneNumberConfirmed)
            .put("twoFactorEnabled", twoFactorEnabled)
            .put("lockoutEnabled", lockoutEnabled)
            .put(
                "lockoutEnd",
                if (lockoutEnd.isEmpty()) JSONObject.NULL
                else java.time.OffsetDateTime.parse(lockoutEnd).toInstant().toString(),
            )
            .put("accessFailedCount", accessFailedCount)
            .apply { if (id.isNotEmpty()) put("id", id) }

    fun validate(): String? {
        if (userName.isBlank()) return "Username is required."
        if (userName.trim().length < 3) return "Username needs at least 3 characters."
        if (email.isBlank()) return "Email is required."
        if (email.trim().length < 3) return "Email needs at least 3 characters."
        if (email.isNotBlank() && !Regex("""^[^\s@]+@[^\s@]+\.[^\s@]+$""").matches(email.trim()))
            return "Enter a valid email."
        if (
            lockoutEnd.isNotEmpty() &&
                runCatching { java.time.OffsetDateTime.parse(lockoutEnd) }.isFailure
        )
            return "Enter a valid date and time."
        if (accessFailedCount < 0) return "Failed attempts must be a nonnegative integer."
        return null
    }

    companion object {
        fun fromJson(json: JSONObject) =
            User(
                id = if (json.isNull("id")) "" else json.optString("id", ""),
                userName = if (json.isNull("userName")) "" else json.optString("userName", ""),
                email = if (json.isNull("email")) "" else json.optString("email", ""),
                emailConfirmed = json.optBoolean("emailConfirmed"),
                phoneNumber =
                    if (json.isNull("phoneNumber")) "" else json.optString("phoneNumber", ""),
                phoneNumberConfirmed = json.optBoolean("phoneNumberConfirmed"),
                twoFactorEnabled = json.optBoolean("twoFactorEnabled"),
                lockoutEnabled = json.optBoolean("lockoutEnabled"),
                lockoutEnd =
                    if (json.isNull("lockoutEnd")) "" else json.optString("lockoutEnd", ""),
                accessFailedCount = json.optInt("accessFailedCount"),
            )
    }
}
