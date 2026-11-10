package com.classifiedads.mobile.shared.ui

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.R

@Composable
fun ActionLabel(label: String) {
    val icon = when (label) {
        "Home" -> R.drawable.action_home
        "Products" -> R.drawable.action_products
        "Files" -> R.drawable.action_files
        "Users" -> R.drawable.action_users
        "Roles" -> R.drawable.action_roles
        "Configuration Entries" -> R.drawable.action_configuration
        "Audit logs" -> R.drawable.action_history
        "Notifications" -> R.drawable.action_notifications
        "Practical.CleanArchitecture" -> R.drawable.action_link
        "Sign out" -> R.drawable.action_logout
        "Add new" -> R.drawable.action_add
        "Edit" -> R.drawable.action_edit
        "Delete" -> R.drawable.action_delete
        "View" -> R.drawable.action_view
        "History" -> R.drawable.action_history
        "Refresh" -> R.drawable.action_refresh
        "Save" -> R.drawable.action_save
        "Cancel" -> R.drawable.action_close
        "Close" -> R.drawable.action_close
        "Confirm" -> R.drawable.action_check
        "Back" -> R.drawable.action_back
        "Previous" -> R.drawable.action_back
        "Download" -> R.drawable.action_download
        "Upload" -> R.drawable.action_upload
        "Upload file" -> R.drawable.action_upload
        "Import" -> R.drawable.action_upload
        "Export CSV" -> R.drawable.action_download
        "Export PDF" -> R.drawable.action_download
        "Copy JSON" -> R.drawable.action_copy
        "Copied" -> R.drawable.action_check
        "Show JSON" -> R.drawable.action_view
        "Hide JSON" -> R.drawable.action_view
        "Theme" -> R.drawable.action_theme
        "Next" -> R.drawable.action_next
        "Choose file" -> R.drawable.action_files
        "Show more" -> R.drawable.action_add
        "Choose date/time" -> R.drawable.action_calendar
        "Choose date and time" -> R.drawable.action_calendar
        "Choose date" -> R.drawable.action_calendar
        "Choose time" -> R.drawable.action_history
        "Clear" -> R.drawable.action_close
        "Sign in" -> R.drawable.action_link
        "Done" -> R.drawable.action_check
        "Set password" -> R.drawable.action_lock
        "Reset email" -> R.drawable.action_mail
        "Confirm email" -> R.drawable.action_mail
        "Keep editing" -> R.drawable.action_edit
        "Discard" -> R.drawable.action_delete
        "Export XLSX" -> R.drawable.action_download
        "Show Image" -> R.drawable.action_image
        "Hide Image" -> R.drawable.action_image
        "system" -> R.drawable.action_configuration
        "light" -> R.drawable.action_theme
        "dark" -> R.drawable.action_theme
        "System" -> R.drawable.action_configuration
        "Light" -> R.drawable.action_theme
        "Dark" -> R.drawable.action_theme
        else -> null
    }
    Row(verticalAlignment = Alignment.CenterVertically, horizontalArrangement = Arrangement.spacedBy(8.dp)) {
        if (icon != null) Icon(painterResource(icon), contentDescription = null, modifier = Modifier.size(18.dp))
        Text(label)
    }
}
