package com.classifiedads.mobile.features.roles

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import com.classifiedads.mobile.shared.ui.*
import org.json.JSONObject

@Composable fun RoleSummary(value: Role, changed: JSONObject = JSONObject()) {}

@Composable
fun RoleDetails(value: Role, changed: JSONObject = JSONObject()) {
    RecordValue("Name", value.name, changed.optBoolean("name"))
    RecordValue("Normalized name", value.normalizedName, changed.optBoolean("normalizedName"))
}

@Composable fun RoleHistory(value: Role, changed: JSONObject = JSONObject()) {}
