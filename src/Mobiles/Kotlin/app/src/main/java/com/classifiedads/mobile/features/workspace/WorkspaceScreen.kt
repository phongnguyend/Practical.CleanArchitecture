package com.classifiedads.mobile.features.workspace

import com.classifiedads.mobile.features.admin.AdminScreen
import com.classifiedads.mobile.features.notifications.PushInbox

import com.classifiedads.mobile.shared.ui.ActionLabel
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.compose.LocalActivity
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.*
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.Alignment
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.semantics.LiveRegionMode
import androidx.compose.ui.semantics.liveRegion
import androidx.compose.ui.semantics.semantics
import com.classifiedads.mobile.R
import androidx.core.view.WindowCompat
import androidx.compose.ui.unit.dp
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleEventObserver
import androidx.lifecycle.compose.LocalLifecycleOwner
import androidx.lifecycle.viewmodel.compose.viewModel
import com.classifiedads.mobile.features.auditlogs.AuditLogsScreen
import com.classifiedads.mobile.features.configurationentries.ConfigurationEntryScreen
import com.classifiedads.mobile.features.files.StoredFileScreen
import com.classifiedads.mobile.features.products.ProductScreen
import com.classifiedads.mobile.features.roles.RoleScreen
import com.classifiedads.mobile.features.users.UserScreen
import com.classifiedads.mobile.shared.ui.HomeInfo
import com.classifiedads.mobile.shared.ui.AppTheme
import com.classifiedads.mobile.shared.ui.HomeButton
import kotlinx.coroutines.launch

val destinations =
    listOf(
        "home" to "Home",
        "products" to "Products",
        "files" to "Files",
        "users" to "Users",
        "roles" to "Roles",
        "configurationentries" to "Configuration Entries",
        "auditlogentries" to "Audit logs",
        "notifications" to "Notifications",
        "admin" to "Admin",
    )

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun MobileApp(model: WorkspaceViewModel = viewModel()) {
    val darkTheme = model.dark ?: isSystemInDarkTheme()
    val activity = LocalActivity.current
    SideEffect {
        activity?.window?.let { window ->
            WindowCompat.getInsetsController(window, window.decorView).apply {
                isAppearanceLightStatusBars = !darkTheme
                isAppearanceLightNavigationBars = !darkTheme
            }
        }
    }
    val owner = LocalLifecycleOwner.current
    val snackbar = remember { SnackbarHostState() }
    var appearance by remember { mutableStateOf(false) }
    var signOut by remember { mutableStateOf(false) }
    val login =
        rememberLauncherForActivityResult(ActivityResultContracts.StartActivityForResult()) { result
            ->
            model.run {
                model.session.complete(result.data)
                model.signedIn = model.session.signedIn
            }
        }
    val logout =
        rememberLauncherForActivityResult(ActivityResultContracts.StartActivityForResult()) {}
    DisposableEffect(owner, model.signedIn) {
        val observer = LifecycleEventObserver { _, event ->
            if (event == Lifecycle.Event.ON_START) {
                model.setNotifications(true)
            } else if (event == Lifecycle.Event.ON_PAUSE) {
                model.setNotifications(false)
                if (model.session.lock()) {
                    model.signedIn = false
                    model.feed = emptyList()
                    model.message = ""
                    snackbar.currentSnackbarData?.dismiss()
                    model.section = "home"
                }
            }
        }
        owner.lifecycle.addObserver(observer)
        model.setNotifications(owner.lifecycle.currentState.isAtLeast(Lifecycle.State.STARTED))
        onDispose {
            owner.lifecycle.removeObserver(observer)
            model.setNotifications(false)
        }
    }
    LaunchedEffect(model.message) {
        if (model.message.isNotEmpty()) {
            snackbar.showSnackbar(model.message)
            model.message = ""
        }
    }
    AppTheme(darkTheme) {
        Scaffold(
            topBar = {
                TopAppBar(
                    title = { Text("CLASSIFIED ADS (Kotlin)", style = MaterialTheme.typography.titleMedium) },
                    colors = TopAppBarDefaults.topAppBarColors(containerColor = MaterialTheme.colorScheme.background),
                    actions = {
                        Box {
                            IconButton(onClick = { appearance = true }) {
                                Icon(
                                    painterResource(R.drawable.action_theme),
                                    contentDescription = "Theme: ${if (model.dark == null) "System" else if (darkTheme) "Dark" else "Light"}",
                                    tint = MaterialTheme.colorScheme.primary,
                                    modifier = Modifier.size(22.dp),
                                )
                            }
                            DropdownMenu(
                                expanded = appearance,
                                onDismissRequest = { appearance = false },
                            ) {
                                listOf("System" to null, "Light" to false, "Dark" to true)
                                    .forEach { (label, value) ->
                                        DropdownMenuItem(
                                            text = { ActionLabel(label) },
                                            onClick = {
                                                model.chooseTheme(value)
                                                appearance = false
                                            },
                                        )
                                    }
                            }
                        }
                    },
                )
            },
            snackbarHost = { SnackbarHost(snackbar) },
        ) { padding ->
            Column(Modifier.padding(padding)) {
                if (model.busy) {
                    LinearProgressIndicator(Modifier.fillMaxWidth())
                }
                if (model.error.isNotEmpty()) {
                    Surface(
                        Modifier.fillMaxWidth().padding(horizontal = 16.dp, vertical = 8.dp),
                        color = MaterialTheme.colorScheme.errorContainer,
                        contentColor = MaterialTheme.colorScheme.onErrorContainer,
                        shape = MaterialTheme.shapes.small,
                    ) {
                        Row(Modifier.padding(start = 12.dp), verticalAlignment = Alignment.CenterVertically) {
                            Text(model.error, Modifier.weight(1f).padding(vertical = 12.dp).semantics {
                                liveRegion = LiveRegionMode.Polite
                            }, style = MaterialTheme.typography.bodyMedium)
                            IconButton(onClick = { model.error = "" }) {
                                Icon(painterResource(R.drawable.action_close), contentDescription = "Dismiss error", modifier = Modifier.size(20.dp))
                            }
                        }
                    }
                }
                if (!model.signedIn) {
                    Column(
                        Modifier.fillMaxWidth().verticalScroll(rememberScrollState()).padding(16.dp),
                        verticalArrangement = Arrangement.spacedBy(16.dp),
                    ) {
                        Text(
                            if (model.session.biometricEnabled) "Workspace locked" else "Your workspace, anywhere.",
                            style = MaterialTheme.typography.headlineSmall,
                        )
                        if (model.session.biometricEnabled) {
                            OutlinedButton(modifier = Modifier.fillMaxWidth(), enabled = !model.busy,
                                onClick = { model.run {
                                    model.session.unlock(checkNotNull(activity))
                                    model.signedIn = model.session.signedIn
                                } }) {
                                ActionLabel("Unlock with biometrics")
                            }
                        }
                        Button(modifier = Modifier.fillMaxWidth().heightIn(min = 48.dp), shape = MaterialTheme.shapes.small,
                            enabled = !model.busy,
                            onClick = { model.run { try {
                                    model.push.disable()
                                } catch (_: Exception) {
                                    // Local token cleanup still runs if the API is offline.
                                }
                                login.launch(model.session.loginIntent()) } },
                        ) {
                            ActionLabel("Sign in")
                        }
                    }
                } else {
                    Row(Modifier.horizontalScroll(rememberScrollState()).padding(horizontal = 16.dp, vertical = 4.dp), horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        destinations.forEach { (key, title) ->
                            FilterChip(
                                modifier = Modifier.heightIn(min = 44.dp),
                                selected = model.section == key,
                                onClick = { model.section = key },
                                label = { ActionLabel(title) },
                                shape = androidx.compose.foundation.shape.RoundedCornerShape(22.dp),
                                border = null,
                                colors = FilterChipDefaults.filterChipColors(
                                    selectedContainerColor = MaterialTheme.colorScheme.primary,
                                    selectedLabelColor = MaterialTheme.colorScheme.onPrimary,
                                    containerColor = MaterialTheme.colorScheme.secondaryContainer,
                                ),
                            )
                        }
                    }
                    key(model.section) {
                        Column(
                            Modifier.fillMaxWidth().weight(1f).verticalScroll(rememberScrollState()).padding(16.dp),
                            verticalArrangement = Arrangement.spacedBy(12.dp),
                        ) {
                            when (model.section) {
                                "products" -> ProductScreen(model.api)
                                "files" -> StoredFileScreen(model.api)
                                "users" -> UserScreen(model.api)
                                "roles" -> RoleScreen(model.api)
                                "configurationentries" -> ConfigurationEntryScreen(model.api)
                                "auditlogentries" -> AuditLogsScreen(model.api)
                                "admin" -> AdminScreen(model)
                                "notifications" -> {
                                    Text("Notifications", style = MaterialTheme.typography.headlineSmall)
                                    PushInbox(model)
                                    Text(model.notificationStatus)
                                    if (model.feed.isEmpty()) {
                                        Text("Live messages appear here while the app is active.")
                                    }
                                    model.feed.forEach { Text(it) }
                                }
                                else -> {
                                    HomeInfo()
                                    destinations.filter { (key, _) -> key !in setOf("home", "notifications", "auditlogentries") }.forEach { (key, title) ->
                                        HomeButton(title) {
                                            model.section = key
                                        }
                                    }
                                    if (model.session.biometricEnabled || model.session.biometricAvailable()) {
                                        HomeButton(if (model.session.biometricEnabled) "Disable biometric login" else "Enable biometric login") {
                                            model.run {
                                                if (model.session.biometricEnabled) {
                                                    model.session.disableBiometrics(checkNotNull(activity))
                                                } else {
                                                    model.session.enableBiometrics(checkNotNull(activity))
                                                }
                                            }
                                        }
                                    }
                                    if (model.session.biometricEnabled) {
                                        HomeButton("Lock app") {
                                            model.session.lock()
                                            model.setNotifications(false)
                                            model.signedIn = false
                                            model.feed = emptyList()
                                            model.message = ""
                                        }
                                    }
                                    HomeButton("Sign out") { signOut = true }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (signOut) {
            AlertDialog(
                onDismissRequest = { signOut = false },
                title = { Text("Sign out of this device?") },
                dismissButton = { OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { signOut = false }) { ActionLabel("Cancel") } },
                confirmButton = {
                    Button(shape = MaterialTheme.shapes.small,
                        onClick = {
                            signOut = false
                            model.run {
                                model.setNotifications(false)
                                try {
                                    model.push.disable()
                                } catch (_: Exception) {
                                    // Sign-out remains available while offline.
                                }
                                val intent = model.session.logoutIntent()
                                model.signedIn = false
                                model.feed = emptyList()
                                model.section = "home"
                                if (intent != null) {
                                    logout.launch(intent)
                                }
                            }
                        }
                    ) {
                        ActionLabel("Confirm")
                    }
                },
            )
        }
    }
}
