package com.classifiedads.mobile.features.auditlogs

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.objects
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun AuditLogsScreen(api: Api) {
    var page by remember { mutableIntStateOf(1) }
    var total by remember { mutableIntStateOf(0) }
    var rows by remember { mutableStateOf(listOf<JSONObject>()) }
    var busy by remember { mutableStateOf(true) }
    var error by remember { mutableStateOf("") }
    var search by remember { mutableStateOf("") }
    var refresh by remember { mutableIntStateOf(0) }
    LaunchedEffect(page, refresh) {
        busy = true
        try {
            val result = api.json("auditlogentries/paged?page=$page&pageSize=5") as JSONObject
            rows = result.getJSONArray("items").objects()
            total = result.optInt("totalItems")
            error = ""
        } catch (e: Exception) {
            error = e.message ?: e.toString()
        } finally {
            busy = false
        }
    }
    Column(verticalArrangement = Arrangement.spacedBy(8.dp)) {
        Text("Audit logs", style = MaterialTheme.typography.headlineSmall)
        TextInput("Search current records", search, { search = it })
        OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { refresh++ }) { Text("Refresh") }
        if (busy) LinearProgressIndicator()
        if (error.isNotEmpty()) Text(error)
        rows
            .filter { it.toString().contains(search, true) }
            .forEach { row ->
                Card {
                    Column(Modifier.padding(12.dp)) {
                        Text(
                            "${row.optString("createdDateTime")} · ${row.optString("userName")} · ${row.optString("action")}"
                        )
                        JsonViewer(row.opt("log"))
                    }
                }
            }
        Row {
            if (page > 1) OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { page-- }) { Text("Previous") }
            Text("Page $page · $total records")
            if (page * 5 < total) OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { page++ }) { Text("Next") }
        }
    }
}
