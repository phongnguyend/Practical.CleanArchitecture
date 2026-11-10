package com.classifiedads.mobile.features.configurationentries

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun ConfigurationEntrySummary(value: ConfigurationEntry, changed: JSONObject = JSONObject()) {
    RecordValue(
        "Value",
        if (value.isSensitive) "******" else value.value,
        changed.optBoolean("value"),
    )
    RecordValue("Description", value.description, changed.optBoolean("description"))
    RecordValue(
        "Updated time",
        localDate(value.updatedDateTime),
        changed.optBoolean("updatedDateTime"),
    )
}

@Composable
fun ConfigurationEntryDetails(value: ConfigurationEntry, changed: JSONObject = JSONObject()) {
    RecordValue("Key", value.key, changed.optBoolean("key"))
    RecordValue(
        "Value",
        if (value.isSensitive) "******" else value.value,
        changed.optBoolean("value"),
    )
    RecordValue("Description", value.description, changed.optBoolean("description"))
    RecordValue(
        "Updated time",
        localDate(value.updatedDateTime),
        changed.optBoolean("updatedDateTime"),
    )
    RecordValue(
        "Sensitive",
        if (value.isSensitive) "Yes" else "No",
        changed.optBoolean("isSensitive"),
    )
}

@Composable
fun ConfigurationEntryHistory(value: ConfigurationEntry, changed: JSONObject = JSONObject()) {}
