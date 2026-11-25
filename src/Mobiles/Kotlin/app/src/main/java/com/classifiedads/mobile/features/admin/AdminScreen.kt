package com.classifiedads.mobile.features.admin

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.features.workspace.WorkspaceViewModel
import com.classifiedads.mobile.shared.ui.ActionLabel

@Composable
fun AdminScreen(model: WorkspaceViewModel) {
    var page by remember { mutableStateOf("admin") }
    if (page != "admin" && page != "playgrounds") {
        key(page) {
            NotificationPlaygrounds(model, page) { page = "playgrounds" }
        }
        return
    }
    Column(verticalArrangement = Arrangement.spacedBy(16.dp)) {
        Text(if (page == "admin") "Admin" else "Playgrounds", style = MaterialTheme.typography.titleLarge)
        if (page == "admin") {
            OutlinedButton(modifier = Modifier.fillMaxWidth(), onClick = { page = "playgrounds" }) {
                ActionLabel("Playgrounds")
            }
        } else {
            OutlinedButton(modifier = Modifier.fillMaxWidth(), onClick = { page = "admin" }) {
                ActionLabel("Back")
            }
            for ((channel, label) in listOf("mobilepush" to "Mobile Push", "email" to "Email", "sms" to "SMS")) {
                OutlinedButton(modifier = Modifier.fillMaxWidth(), onClick = { page = channel }) {
                    ActionLabel(label)
                }
            }
        }
    }
}
