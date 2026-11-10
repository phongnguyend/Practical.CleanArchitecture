package com.classifiedads.mobile.shared.ui

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.text.selection.SelectionContainer
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import org.json.JSONArray
import org.json.JSONObject

@Composable
fun Sheet(
    title: String,
    close: () -> Unit,
    footer: @Composable RowScope.() -> Unit,
    content: @Composable ColumnScope.() -> Unit,
) {
    androidx.compose.ui.window.Dialog(onDismissRequest = close) {
        Surface(shape = MaterialTheme.shapes.large) {
            Column(Modifier.heightIn(max = 640.dp)) {
                Row(
                    Modifier.fillMaxWidth().padding(16.dp),
                    horizontalArrangement = Arrangement.spacedBy(12.dp),
                    verticalAlignment = Alignment.CenterVertically,
                ) {
                    Text(title, Modifier.weight(1f), style = MaterialTheme.typography.titleLarge)
                    OutlinedButton(shape = MaterialTheme.shapes.small, onClick = close) { ActionLabel("Close") }
                }
                HorizontalDivider()
                Column(
                    Modifier.weight(1f, fill = false)
                        .verticalScroll(rememberScrollState())
                        .padding(16.dp),
                    verticalArrangement = Arrangement.spacedBy(12.dp),
                    content = content,
                )
                HorizontalDivider()
                Row(
                    Modifier.fillMaxWidth().padding(16.dp),
                    horizontalArrangement = Arrangement.spacedBy(12.dp, Alignment.End),
                    verticalAlignment = Alignment.CenterVertically,
                    content = footer,
                )
            }
        }
    }
}

fun pretty(value: Any?): String =
    when (value) {
        is JSONObject -> value.toString(2)
        is JSONArray -> value.toString(2)
        else -> value?.toString() ?: "null"
    }

@Composable
fun JsonTree(value: Any?, name: String = "root") {
    var open by remember(value) { mutableStateOf(name == "root") }
    var limit by remember(value) { mutableIntStateOf(50) }
    val entries =
        when (value) {
            is JSONObject -> value.keys().asSequence().map { it to value.opt(it) }.toList()
            is JSONArray -> (0 until value.length()).map { it.toString() to value.opt(it) }
            else -> null
        }
    if (entries == null)
        SelectionContainer {
            Text("$name: ${pretty(value)}", color = MaterialTheme.colorScheme.primary)
        }
    else
        Column(Modifier.padding(start = 8.dp)) {
            OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { open = !open }) {
                Text("${if (open) "▼" else "▶"} $name · ${entries.size}")
            }
            if (open) {
                entries.take(limit).forEach { (key, item) -> JsonTree(item, key) }
                if (entries.size > limit)
                    OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { limit += 50 }) { ActionLabel("Show more") }
            }
        }
}

@Composable
fun JsonViewer(value: Any?) {
    var open by remember { mutableStateOf(false) }
    var copied by remember { mutableStateOf(false) }
    val clipboard = androidx.compose.ui.platform.LocalClipboardManager.current
    val parsed =
        if (value is String)
            runCatching { org.json.JSONTokener(value).nextValue() }.getOrDefault(value)
        else value
    Column {
        Row(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
            OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { open = !open }) { ActionLabel(if (open) "Hide JSON" else "Show JSON") }
            OutlinedButton(shape = MaterialTheme.shapes.small,
                onClick = {
                    clipboard.setText(
                        androidx.compose.ui.text.AnnotatedString(
                            if (value is String) value else pretty(value)
                        )
                    )
                    copied = true
                }
            ) {
                ActionLabel(if (copied) "Copied" else "Copy JSON")
            }
        }
        if (open) JsonTree(parsed)
    }
}
