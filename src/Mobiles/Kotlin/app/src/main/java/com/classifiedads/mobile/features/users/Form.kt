package com.classifiedads.mobile.features.users

import android.net.Uri
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*

@Composable
fun UserForm(
    value: User,
    change: (User) -> Unit,
    file: Uri?,
    pick: () -> Unit,
    busy: Boolean,
    fileName: (Uri) -> String,
) {
    TextInput(
        "Username",
        value.userName.toString(),
        { change(value.copy(userName = it)) },
        disabled = busy,
    )
    TextInput("Email", value.email.toString(), { change(value.copy(email = it)) }, disabled = busy)
    ToggleInput(
        "Email confirmed",
        value.emailConfirmed,
        { change(value.copy(emailConfirmed = it)) },
        disabled = busy,
    )
    TextInput(
        "Phone number",
        value.phoneNumber.toString(),
        { change(value.copy(phoneNumber = it)) },
        disabled = busy,
    )
    ToggleInput(
        "Phone confirmed",
        value.phoneNumberConfirmed,
        { change(value.copy(phoneNumberConfirmed = it)) },
        disabled = busy,
    )
    ToggleInput(
        "Two-factor enabled",
        value.twoFactorEnabled,
        { change(value.copy(twoFactorEnabled = it)) },
        disabled = busy,
    )
    ToggleInput(
        "Lockout enabled",
        value.lockoutEnabled,
        { change(value.copy(lockoutEnabled = it)) },
        disabled = busy,
    )
    DateField("Lockout end", value.lockoutEnd, !busy, { change(value.copy(lockoutEnd = it)) })
    TextInput(
        "Failed attempts",
        value.accessFailedCount.toString(),
        { change(value.copy(accessFailedCount = it.toIntOrNull() ?: -1)) },
        disabled = busy,
        numeric = true,
    )
}
