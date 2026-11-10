package com.classifiedads.mobile.features.users

import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.ui.*
import kotlinx.coroutines.launch
import org.json.JSONObject

fun validatePassword(password: String, confirmation: String): String? =
    if (password.isBlank()) "New password is required."
    else if (password != confirmation) "Passwords do not match." else null

@Composable
fun UserActions(value: User, api: Api) {
    var mode by remember { mutableStateOf("") }
    var password by remember { mutableStateOf("") }
    var confirmation by remember { mutableStateOf("") }
    var error by remember { mutableStateOf("") }
    var busy by remember { mutableStateOf(false) }
    var discard by remember { mutableStateOf(false) }
    var notice by remember { mutableStateOf("") }
    val scope = rememberCoroutineScope()
    fun open(next: String) {
        mode = next
        password = ""
        confirmation = ""
        error = ""
        discard = false
    }
    fun close() {
        if (!busy) {
            if (mode == "password" && (password.isNotEmpty() || confirmation.isNotEmpty()))
                discard = true
            else mode = ""
        }
    }
    OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { open("password") }) { Text("Set password") }
    OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { open("passwordresetemail") }) { Text("Reset email") }
    OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { open("emailaddressconfirmation") }) { Text("Confirm email") }
    if (notice.isNotEmpty()) Text(notice)
    if (mode.isNotEmpty())
        Sheet(
            if (discard) "Discard unsaved changes?"
            else if (mode == "password") "Set password" else "Confirm action",
            ::close,
            footer = {
                if (discard) {
                    OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { discard = false }) { Text("Keep editing") }
                    Button(shape = MaterialTheme.shapes.small, onClick = { mode = "" }) { Text("Discard") }
                } else {
                    OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = ::close) { Text("Cancel") }
                    Button(shape = MaterialTheme.shapes.small,
                        enabled = !busy,
                        onClick = {
                            val validation =
                                if (mode == "password") validatePassword(password, confirmation)
                                else null
                            if (validation != null) error = validation
                            else {
                                busy = true
                                scope.launch {
                                    try {
                                        val body = JSONObject().put("id", value.id)
                                        if (mode == "password") body.put("password", password)
                                        api.json(
                                            "users/${android.net.Uri.encode(value.id)}/$mode",
                                            if (mode == "password") "PUT" else "POST",
                                            body,
                                        )
                                        notice =
                                            if (mode == "password") "Password updated"
                                            else "Email requested"
                                        mode = ""
                                    } catch (e: Exception) {
                                        error = e.message ?: e.toString()
                                    } finally {
                                        busy = false
                                    }
                                }
                            }
                        },
                    ) {
                        Text(if (mode == "password") "Save" else "Confirm")
                    }
                }
            },
        ) {
            if (discard) Text("Discard unsaved changes?")
            else if (mode == "password") {
                TextInput(
                    "New password",
                    password,
                    { password = it },
                    disabled = busy,
                    secret = true,
                )
                TextInput(
                    "Confirm password",
                    confirmation,
                    { confirmation = it },
                    disabled = busy,
                    secret = true,
                )
            } else
                Text(
                    "Send ${if(mode=="passwordresetemail")"password reset" else "confirmation"} email to ${value.email}?"
                )
            if (error.isNotEmpty()) Text(error, color = MaterialTheme.colorScheme.error)
        }
}
