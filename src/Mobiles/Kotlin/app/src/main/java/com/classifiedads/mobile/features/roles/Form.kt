package com.classifiedads.mobile.features.roles

import android.net.Uri
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*

@Composable
fun RoleForm(
    value: Role,
    change: (Role) -> Unit,
    file: Uri?,
    pick: () -> Unit,
    busy: Boolean,
    fileName: (Uri) -> String,
) {
    TextInput("Name", value.name.toString(), { change(value.copy(name = it)) }, disabled = busy)
}
