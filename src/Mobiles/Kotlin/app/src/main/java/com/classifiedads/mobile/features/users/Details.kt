package com.classifiedads.mobile.features.users

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun UserSummary(value: User, changed: JSONObject = JSONObject()) {
    RecordValue("Email", value.email, changed.optBoolean("email"))
}

@Composable
fun UserDetails(value: User, changed: JSONObject = JSONObject()) {
    RecordValue("Username", value.userName, changed.optBoolean("userName"))
    RecordValue("Email", value.email, changed.optBoolean("email"))
    RecordValue(
        "Email confirmed",
        if (value.emailConfirmed) "Yes" else "No",
        changed.optBoolean("emailConfirmed"),
    )
    RecordValue("Phone number", value.phoneNumber, changed.optBoolean("phoneNumber"))
    RecordValue(
        "Phone confirmed",
        if (value.phoneNumberConfirmed) "Yes" else "No",
        changed.optBoolean("phoneNumberConfirmed"),
    )
    RecordValue(
        "Two-factor enabled",
        if (value.twoFactorEnabled) "Yes" else "No",
        changed.optBoolean("twoFactorEnabled"),
    )
    RecordValue(
        "Lockout enabled",
        if (value.lockoutEnabled) "Yes" else "No",
        changed.optBoolean("lockoutEnabled"),
    )
    RecordValue(
        "Lockout end (ISO 8601)",
        localDate(value.lockoutEnd),
        changed.optBoolean("lockoutEnd"),
    )
    RecordValue("Failed attempts", value.accessFailedCount, changed.optBoolean("accessFailedCount"))
}

@Composable
fun UserHistory(value: User, changed: JSONObject = JSONObject()) {
    RecordValue("Username", value.userName, changed.optBoolean("userName"))
    RecordValue("Email", value.email, changed.optBoolean("email"))
    RecordValue(
        "Email confirmed",
        if (value.emailConfirmed) "Yes" else "No",
        changed.optBoolean("emailConfirmed"),
    )
    RecordValue("Phone number", value.phoneNumber, changed.optBoolean("phoneNumber"))
    RecordValue(
        "Phone confirmed",
        if (value.phoneNumberConfirmed) "Yes" else "No",
        changed.optBoolean("phoneNumberConfirmed"),
    )
    RecordValue(
        "Two-factor enabled",
        if (value.twoFactorEnabled) "Yes" else "No",
        changed.optBoolean("twoFactorEnabled"),
    )
    RecordValue(
        "Lockout enabled",
        if (value.lockoutEnabled) "Yes" else "No",
        changed.optBoolean("lockoutEnabled"),
    )
    RecordValue(
        "Lockout end (ISO 8601)",
        localDate(value.lockoutEnd),
        changed.optBoolean("lockoutEnd"),
    )
    RecordValue("Failed attempts", value.accessFailedCount, changed.optBoolean("accessFailedCount"))
}
