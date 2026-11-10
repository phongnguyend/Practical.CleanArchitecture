package com.classifiedads.mobile.features.files

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable
fun StoredFileSummary(value: StoredFile, changed: JSONObject = JSONObject()) {
    RecordValue("File name", value.fileName, changed.optBoolean("fileName"))
    RecordValue("Description", value.description, changed.optBoolean("description"))
    RecordValue("Size", "${value.size} bytes", changed.optBoolean("size"))
    RecordValue("Uploaded time", localDate(value.uploadedTime), changed.optBoolean("uploadedTime"))
}

@Composable
fun StoredFileDetails(value: StoredFile, changed: JSONObject = JSONObject()) {
    RecordValue("Name", value.name, changed.optBoolean("name"))
    RecordValue("File name", value.fileName, changed.optBoolean("fileName"))
    RecordValue("Description", value.description, changed.optBoolean("description"))
    RecordValue("Size", "${value.size} bytes", changed.optBoolean("size"))
    RecordValue("Uploaded time", localDate(value.uploadedTime), changed.optBoolean("uploadedTime"))
    RecordValue("Encrypted", if (value.encrypted) "Yes" else "No", changed.optBoolean("encrypted"))
    RecordValue("File location", value.fileLocation, changed.optBoolean("fileLocation"))
}

@Composable
fun StoredFileHistory(value: StoredFile, changed: JSONObject = JSONObject()) {
    RecordValue("Name", value.name, changed.optBoolean("name"))
    RecordValue("Description", value.description, changed.optBoolean("description"))
    RecordValue("File name", value.fileName, changed.optBoolean("fileName"))
    RecordValue("File location", value.fileLocation, changed.optBoolean("fileLocation"))
}
