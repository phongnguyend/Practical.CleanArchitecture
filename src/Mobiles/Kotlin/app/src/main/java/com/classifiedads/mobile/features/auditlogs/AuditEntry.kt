package com.classifiedads.mobile.features.auditlogs

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.selection.SelectionContainer
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject
import org.json.JSONArray
import org.json.JSONTokener
import java.time.OffsetDateTime
import java.time.ZoneId
import java.time.format.DateTimeFormatter
import java.time.format.FormatStyle

@Composable
fun AuditEntry(entry: JSONObject) {
    var expanded by remember { mutableStateOf(false) }
    val raw = entry.opt("log")
    val data = if (raw is String) {
        runCatching { JSONTokener(raw).nextValue() }.getOrDefault(raw)
    } else {
        raw
    }
    val rawDate = entry.optString("createdDateTime")
    val timestamp = runCatching {
        OffsetDateTime.parse(rawDate).atZoneSameInstant(ZoneId.systemDefault())
            .format(DateTimeFormatter.ofLocalizedDateTime(FormatStyle.MEDIUM))
    }.getOrDefault(rawDate)
    Card(Modifier.fillMaxWidth()) {
        Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(12.dp)) {
            Surface(color = MaterialTheme.colorScheme.secondaryContainer, shape = MaterialTheme.shapes.small) {
                Text(entry.optString("action").ifBlank { "Event" }, modifier = Modifier.padding(horizontal = 10.dp, vertical = 4.dp), style = MaterialTheme.typography.titleSmall)
            }
            Text(timestamp, style = MaterialTheme.typography.bodySmall)
            Text("By ${entry.optString("userName").ifBlank { "Unknown user" }}")
            OutlinedButton(onClick = { expanded = !expanded }, modifier = Modifier.fillMaxWidth(), shape = MaterialTheme.shapes.small) {
                ActionLabel(if (expanded) "Hide event details" else "View event details")
            }
            if (expanded) {
                if (data is JSONObject) {
                    data.keys().forEach { key ->
                        HorizontalDivider()
                        Text(key.replace(Regex("([a-z])([A-Z])"), "$1 $2"), style = MaterialTheme.typography.labelMedium)
                        SelectionContainer {
                            Text(auditValue(data.opt(key)))
                        }
                    }
                } else {
                    SelectionContainer {
                        Text(auditValue(data))
                    }
                }
                HorizontalDivider()
                Text("Full data", style = MaterialTheme.typography.labelMedium)
                JsonViewer(data)
            }
        }
    }
}

private fun auditValue(value: Any?): String = when {
    value == null || value == JSONObject.NULL || value == "" -> "Not provided"
    value is JSONObject -> value.toString(2)
    value is JSONArray -> value.toString(2)
    else -> value.toString()
}
