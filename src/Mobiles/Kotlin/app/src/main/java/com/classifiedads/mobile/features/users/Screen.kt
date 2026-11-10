package com.classifiedads.mobile.features.users

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.platform.LocalContext
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.auditSnapshot
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun UserScreen(api: Api) {
    val repository = remember(api) { UserRepository(api) }
    val context = LocalContext.current
    var showImages by remember { mutableStateOf(false) }
    var ratingMessage by remember { mutableStateOf("") }
    RecordPage(
        title = "Users",
        repository = repository,
        empty = { User() },
        label = { it.userName },
        searchText = { it.toJson().toString() },
        serialize = { it.toJson() },
        validate = { it.validate() },
        form = { value, change, file, pick, busy ->
            UserForm(value, change, file, pick, busy, api::fileName)
        },
        summary = { value -> UserSummary(value) },
        details = { UserDetails(it) },
        history = repository::history,
        historyBody = { entry ->
            UserHistory(
                User.fromJson(auditSnapshot(entry)),
                entry.optJSONObject("highLight") ?: JSONObject(),
            )
        },
        actions = { value -> UserActions(value, api) },
    )
}
