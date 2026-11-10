package com.classifiedads.mobile.features.files

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.platform.LocalContext
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.model.auditSnapshot
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun StoredFileScreen(api: Api) {
    val repository = remember(api) { StoredFileRepository(api) }
    val context = LocalContext.current
    var showImages by remember { mutableStateOf(false) }
    var ratingMessage by remember { mutableStateOf("") }
    RecordPage(
        title = "Files",
        repository = repository,
        empty = { StoredFile() },
        label = { it.name },
        searchText = { it.toJson().toString() },
        serialize = { it.toJson() },
        validate = { it.validate() },
        form = { value, change, file, pick, busy ->
            StoredFileForm(value, change, file, pick, busy, api::fileName)
        },
        summary = { value -> StoredFileSummary(value) },
        details = { StoredFileDetails(it) },
        history = repository::history,
        historyBody = { entry ->
            StoredFileHistory(
                StoredFile.fromJson(auditSnapshot(entry)),
                entry.optJSONObject("highLight") ?: JSONObject(),
            )
        },
        createTitle = "Upload file",
        actions = { value ->
            OperationButton("Download") {
                context.startActivity(
                    api.download(
                        "files/${android.net.Uri.encode(value.id)}/download",
                        value.fileName.ifEmpty { "download.bin" },
                    )
                )
            }
        },
    )
}
