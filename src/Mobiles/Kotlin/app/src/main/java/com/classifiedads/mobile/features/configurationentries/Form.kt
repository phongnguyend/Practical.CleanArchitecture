package com.classifiedads.mobile.features.configurationentries

import android.net.Uri
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*

@Composable
fun ConfigurationEntryForm(
    value: ConfigurationEntry,
    change: (ConfigurationEntry) -> Unit,
    file: Uri?,
    pick: () -> Unit,
    busy: Boolean,
    fileName: (Uri) -> String,
) {
    TextInput("Key", value.key.toString(), { change(value.copy(key = it)) }, disabled = busy)
    TextInput(
        "Value",
        value.value.toString(),
        { change(value.copy(value = it)) },
        disabled = busy,
        secret = value.isSensitive,
    )
    TextInput(
        "Description",
        value.description.toString(),
        { change(value.copy(description = it)) },
        disabled = busy,
        multiline = true,
    )
    ToggleInput(
        "Sensitive",
        value.isSensitive,
        { change(value.copy(isSensitive = it)) },
        disabled = busy,
    )
}
