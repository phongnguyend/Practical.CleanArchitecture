package com.classifiedads.mobile.features.configurationentries

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.platform.LocalContext
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.ui.*

@Composable
fun ConfigurationEntryScreen(api: Api) {
    val repository = remember(api) { ConfigurationEntryRepository(api) }
    val context = LocalContext.current
    var showImages by remember { mutableStateOf(false) }
    var ratingMessage by remember { mutableStateOf("") }
    RecordPage(
        title = "Configuration",
        repository = repository,
        empty = { ConfigurationEntry() },
        label = { it.key },
        searchText = { it.toJson().toString() },
        serialize = { it.toJson() },
        validate = { it.validate() },
        form = { value, change, file, pick, busy ->
            ConfigurationEntryForm(value, change, file, pick, busy, api::fileName)
        },
        summary = { value -> ConfigurationEntrySummary(value) },
        details = { ConfigurationEntryDetails(it) },
        toolbar = { reload ->
            ImportButton(".xlsx", api::fileName, repository::importFile, reload)
            OperationButton("Export XLSX") {
                context.startActivity(
                    api.download("configurationentries/ExportAsExcel", "ConfigurationEntries.xlsx")
                )
            }
        },
    )
}
