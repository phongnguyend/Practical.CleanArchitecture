package com.classifiedads.mobile.features.files

import android.net.Uri
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*

@Composable
fun StoredFileForm(
    value: StoredFile,
    change: (StoredFile) -> Unit,
    file: Uri?,
    pick: () -> Unit,
    busy: Boolean,
    fileName: (Uri) -> String,
) {
    TextInput("Name", value.name.toString(), { change(value.copy(name = it)) }, disabled = busy)
    TextInput(
        "Description",
        value.description.toString(),
        { change(value.copy(description = it)) },
        disabled = busy,
        multiline = true,
    )
    ToggleInput(
        "Encrypted",
        value.encrypted,
        { change(value.copy(encrypted = it)) },
        disabled = busy || value.id.isNotEmpty(),
    )
    if (value.id.isNotEmpty())
        TextInput(
            "File name",
            value.fileName.toString(),
            { change(value.copy(fileName = it)) },
            disabled = busy || true,
        )
    if (value.id.isEmpty()) {
        OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = pick) { Text("Choose file") }
        Text(file?.let(fileName) ?: "No file selected")
    }
}
