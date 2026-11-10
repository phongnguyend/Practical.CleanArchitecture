package com.classifiedads.mobile.features.products

import android.net.Uri
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*

@Composable
fun ProductForm(
    value: Product,
    change: (Product) -> Unit,
    file: Uri?,
    pick: () -> Unit,
    busy: Boolean,
    fileName: (Uri) -> String,
) {
    TextInput("Name", value.name.toString(), { change(value.copy(name = it)) }, disabled = busy)
    TextInput("Code", value.code.toString(), { change(value.copy(code = it)) }, disabled = busy)
    TextInput(
        "Description",
        value.description.toString(),
        { change(value.copy(description = it)) },
        disabled = busy,
        multiline = true,
    )
}
