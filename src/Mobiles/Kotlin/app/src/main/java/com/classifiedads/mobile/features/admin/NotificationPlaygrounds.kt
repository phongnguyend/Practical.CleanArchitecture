package com.classifiedads.mobile.features.admin

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.KeyboardOptions
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.input.KeyboardType
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.features.workspace.WorkspaceViewModel
import com.classifiedads.mobile.shared.ui.ActionLabel
import org.json.JSONObject

@Composable
fun NotificationPlaygrounds(model: WorkspaceViewModel, channel: String, back: () -> Unit) {
    val push = channel == "mobilepush"
    var section by remember { mutableStateOf("notifications") }
    var choosing by remember { mutableStateOf(false) }
    val email = channel == "email"
    val path = if (push) "notifications/playground" else "notifications/playgrounds/$channel"
    var from by remember { mutableStateOf("") }
    var recipient by remember { mutableStateOf("") }
    var subject by remember { mutableStateOf("") }
    var body by remember { mutableStateOf("") }
    var feedback by remember { mutableStateOf("") }
    var info by remember { mutableStateOf<JSONObject?>(null) }
    suspend fun refresh() {
        info = model.api.json(path) as JSONObject
    }
    LaunchedEffect(channel) {
        model.run { refresh() }
    }
    Column(verticalArrangement = Arrangement.spacedBy(16.dp)) {
        Text(if (push) "Mobile Push" else if (email) "Email" else "SMS", style = MaterialTheme.typography.titleLarge)
        OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy, onClick = back) {
            ActionLabel("Back")
        }
        Text(if (push) "Queue a notification for your own registered devices." else "Queue a message for delivery by the existing ${if (email) "Email" else "SMS"} worker.")
        info?.let { data ->
            Text("Provider: ${data.getString("provider")}. " + if (data.getString("provider") == "Fake") "Delivery is simulated; no message is sent." else "Queued messages may be delivered to the specified recipient.")
            if (push) {
                Text("Registered devices: ${data.getInt("deviceCount")}")
            }
        }
        if (email) {
            OutlinedTextField(value = from, onValueChange = { from = it.take(254) }, label = { Text("From") }, enabled = !model.busy, keyboardOptions = KeyboardOptions(keyboardType = KeyboardType.Email), modifier = Modifier.fillMaxWidth())
        }
        if (!push) {
            OutlinedTextField(value = recipient, onValueChange = { recipient = it.take(if (email) 254 else 16) }, label = { Text(if (email) "To" else "Phone number") }, enabled = !model.busy, keyboardOptions = KeyboardOptions(keyboardType = if (email) KeyboardType.Email else KeyboardType.Phone), modifier = Modifier.fillMaxWidth())
        }
        if (!email && !push) {
            Text("International format, for example +15551234567.")
        }
        if (email || push) {
            OutlinedTextField(value = subject, onValueChange = { subject = it.take(if (push) 100 else 200) }, label = { Text(if (push) "Title" else "Subject") }, enabled = !model.busy, modifier = Modifier.fillMaxWidth())
        }
        OutlinedTextField(value = body, onValueChange = { body = it.take(if (push) 500 else if (email) 10000 else 1600) }, label = { Text("Message") }, minLines = 3, enabled = !model.busy, modifier = Modifier.fillMaxWidth())
        Text("${body.length}/${if (push) 500 else if (email) 10000 else 1600} characters")
        if (push) {
            Box {
                OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy, onClick = { choosing = true }) {
                    ActionLabel("Destination: $section")
                }
                DropdownMenu(expanded = choosing, onDismissRequest = { choosing = false }) {
                    for (value in listOf("notifications", "home", "products", "files", "users", "roles", "configurationentries", "auditlogentries")) {
                        DropdownMenuItem(text = { Text(value) }, onClick = {
                            section = value
                            choosing = false
                        })
                    }
                }
            }
        }
        Button(modifier = Modifier.fillMaxWidth(), enabled = !model.busy && info != null, onClick = {
            feedback = ""
            val pattern = Regex("""^[^\s@;]+@[^\s@;]+\.[^\s@;]+$""")
            if ((!push && recipient.isBlank()) || body.isBlank() || ((email || push) && subject.isBlank()) || (email && from.isBlank())) {
                feedback = if (push) "Title and message are required." else "Complete all required fields."
            } else if (!push && (if (email) !pattern.matches(from.trim()) || !pattern.matches(recipient.trim()) else !Regex("""^\+[1-9][0-9]{7,14}$""").matches(recipient.trim()))) {
                feedback = if (email) "Enter valid From and To email addresses." else "Enter a phone number in international format, for example +15551234567."
            } else {
                model.run {
                    model.api.json(path, "POST", if (push) JSONObject().put("title", subject.trim()).put("body", body.trim()).put("section", section) else JSONObject().put("from", from.trim()).put("recipient", recipient.trim()).put("subject", subject.trim()).put("body", body.trim()))
                    feedback = "Message queued successfully."
                    refresh()
                }
            }
        }) {
            ActionLabel("Queue message")
        }
        OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy, onClick = { model.run { refresh() } }) {
            ActionLabel("Refresh queue")
        }
        if (feedback.isNotEmpty()) {
            Text(feedback)
        }
        Text("Recent messages", style = MaterialTheme.typography.titleMedium)
        val items = info?.getJSONArray("items")
        if (items?.length() == 0) {
            Text("No queued messages yet.")
        }
        for (index in 0 until (items?.length() ?: 0)) {
            val item = items!!.getJSONObject(index)
            Card(modifier = Modifier.fillMaxWidth()) {
                Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text(item.getString("title"), style = MaterialTheme.typography.titleMedium)
                    if (!push) {
                        Text("To: ${item.getString("recipient")}")
                    }
                    Text(item.getString("body"))
                    Text(if (push) "Pending: ${item.getInt("pending")} / Completed: ${item.getInt("completed")} / Failed: ${item.getInt("failed")}" else "${item.getString("status")} / Attempts: ${item.getInt("attempts")}")
                }
            }
        }
    }
}
