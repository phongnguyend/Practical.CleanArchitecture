package com.classifiedads.mobile.features.notifications

import android.Manifest
import android.os.Build
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.BuildConfig
import com.classifiedads.mobile.features.workspace.WorkspaceViewModel
import com.classifiedads.mobile.shared.ui.ActionLabel
import org.json.JSONObject

@Composable
fun PushInbox(model: WorkspaceViewModel) {
    var items by remember { mutableStateOf(listOf<JSONObject>()) }
    var page by remember { mutableIntStateOf(1) }
    var total by remember { mutableIntStateOf(0) }
    var testing by remember { mutableStateOf(false) }
    suspend fun refresh() {
        val data = model.api.json("notifications/inbox?page=$page") as JSONObject
        val array = data.getJSONArray("items")
        items = (0 until array.length()).map { array.getJSONObject(it) }
        total = data.getInt("total")
    }
    LaunchedEffect(page) {
        model.run {
            testing = (model.api.json("notifications/status") as JSONObject).optBoolean("allowTestMessages")
            refresh()
        }
    }
    val permission = rememberLauncherForActivityResult(ActivityResultContracts.RequestPermission()) { granted ->
        if (granted) {
            model.run { model.push.enable() }
        } else {
            model.error = "Allow notifications in device settings to receive updates."
        }
    }
    Column(verticalArrangement = Arrangement.spacedBy(12.dp)) {
        Text("Notification inbox", style = MaterialTheme.typography.titleLarge)
        if (!BuildConfig.PUSH_NOTIFICATIONS_ENABLED) {
            Text("Push notifications are not configured for this build.")
        }
        OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy && BuildConfig.PUSH_NOTIFICATIONS_ENABLED, onClick = {
            if (Build.VERSION.SDK_INT >= 33) {
                permission.launch(Manifest.permission.POST_NOTIFICATIONS)
            } else {
                model.run { model.push.enable() }
            }
        }) {
            ActionLabel("Enable notifications")
        }
        OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy && BuildConfig.PUSH_NOTIFICATIONS_ENABLED, onClick = { model.run { model.push.disable() } }) {
            ActionLabel("Disable notifications")
        }
        if (testing) {
            OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy, onClick = {
                model.run {
                    model.api.json("notifications/test", "POST", JSONObject())
                    refresh()
                }
            }) {
                ActionLabel("Send test notification")
            }
        }
        OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy, onClick = { model.run { refresh() } }) {
            ActionLabel("Refresh inbox")
        }
        if (items.isEmpty()) {
            Text("No saved notifications yet.")
        }
        for (item in items) {
            Card(modifier = Modifier.fillMaxWidth()) {
                Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(10.dp)) {
                    Text(item.getString("title"), style = MaterialTheme.typography.titleMedium)
                    Text(item.getString("body"))
                    Text(item.getString("createdAt") + if (item.isNull("readAt")) " / Unread" else " / Read")
                    OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy, onClick = {
                        model.run {
                            model.api.json("notifications/inbox/${item.getString("id")}/read", "PUT", JSONObject())
                            model.section = item.getString("section")
                        }
                    }) {
                        ActionLabel("Open notification")
                    }
                }
            }
        }
        Text("Page $page / $total notifications")
        Row(horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            OutlinedButton(modifier = Modifier.weight(1f), enabled = !model.busy && page > 1, onClick = { page-- }) {
                ActionLabel("Previous")
            }
            OutlinedButton(modifier = Modifier.weight(1f), enabled = !model.busy && page * 20 < total, onClick = { page++ }) {
                ActionLabel("Next")
            }
        }
    }
}
