package com.classifiedads.mobile.features.roles

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.platform.LocalContext
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.ui.*

@Composable
fun RoleScreen(api: Api) {
    val repository = remember(api) { RoleRepository(api) }
    val context = LocalContext.current
    var showImages by remember { mutableStateOf(false) }
    var ratingMessage by remember { mutableStateOf("") }
    RecordPage(
        title = "Roles",
        repository = repository,
        empty = { Role() },
        label = { it.name },
        searchText = { it.toJson().toString() },
        serialize = { it.toJson() },
        validate = { it.validate() },
        form = { value, change, file, pick, busy ->
            RoleForm(value, change, file, pick, busy, api::fileName)
        },
        summary = { value -> RoleSummary(value) },
        details = { RoleDetails(it) },
    )
}
