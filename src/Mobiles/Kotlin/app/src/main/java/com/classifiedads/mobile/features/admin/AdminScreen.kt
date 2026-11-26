package com.classifiedads.mobile.features.admin

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.features.workspace.WorkspaceViewModel
import com.classifiedads.mobile.R
import androidx.compose.ui.res.painterResource

@Composable
fun AdminScreen(model: WorkspaceViewModel) {
    var page by remember { mutableStateOf("admin") }
    Column(verticalArrangement = Arrangement.spacedBy(16.dp)) {
        Row(Modifier.fillMaxWidth()) {
            PlaygroundTab("Playgrounds", page != "admin", Modifier.weight(1f)) { page = "playgrounds" }
        }
        if (page != "admin") {
            Row(Modifier.fillMaxWidth()) {
                for ((channel, label) in listOf("mobilepush" to "Mobile Push", "email" to "Email", "sms" to "SMS")) {
                    PlaygroundTab(label, page == channel, Modifier.weight(1f)) { page = channel }
                }
            }
        }
        if (page == "admin" || page == "playgrounds") {
            Text(if (page == "admin") "Admin" else "Playgrounds", style = MaterialTheme.typography.titleLarge)
            Text("Choose a playground to inspect its queue and compose a message.")
        } else {
            key(page) {
                NotificationPlaygrounds(model, page)
            }
        }
    }
}

@Composable
private fun PlaygroundTab(label: String, selected: Boolean, modifier: Modifier, onClick: () -> Unit) {
    Column(modifier) {
        val icon = when (label) {
            "Playgrounds" -> R.drawable.action_configuration
            "Email" -> R.drawable.action_mail
            else -> R.drawable.action_notifications
        }
        Tab(
            selected = selected,
            onClick = onClick,
            modifier = Modifier.heightIn(min = 48.dp),
            text = { Text(label) },
            icon = { Icon(painterResource(icon), contentDescription = null, modifier = Modifier.size(18.dp)) }
        )
        Box(Modifier.fillMaxWidth().height(3.dp).background(if (selected) MaterialTheme.colorScheme.primary else MaterialTheme.colorScheme.outlineVariant))
    }
}
