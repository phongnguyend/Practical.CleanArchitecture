package com.classifiedads.mobile.shared.ui

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp

@Composable
fun Pagination(page: Int, total: Int, pageSize: Int, busy: Boolean = false, onPage: (Int) -> Unit) {
    Column(Modifier.fillMaxWidth().padding(vertical = 12.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
        Text("Page $page | $total records", Modifier.fillMaxWidth(), textAlign = TextAlign.Center)
        Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp)) {
            OutlinedButton(onClick = { onPage(page - 1) }, enabled = !busy && page > 1,
                shape = MaterialTheme.shapes.small, modifier = Modifier.weight(1f).heightIn(min = 48.dp)) { ActionLabel("Previous") }
            OutlinedButton(onClick = { onPage(page + 1) }, enabled = !busy && page * pageSize < total,
                shape = MaterialTheme.shapes.small, modifier = Modifier.weight(1f).heightIn(min = 48.dp)) { ActionLabel("Next") }
        }
    }
}
