package com.classifiedads.mobile.features.workspace

import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.compose.LocalActivity
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.*
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
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
            if (event == Lifecycle.Event.ON_START) model.setNotifications(true)
            else if (event == Lifecycle.Event.ON_STOP) model.setNotifications(false)
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
                    title = { Text("CLASSIFIED ADS", style = MaterialTheme.typography.titleLarge) },
                    colors = TopAppBarDefaults.topAppBarColors(containerColor = MaterialTheme.colorScheme.background),
                    actions = {
                        Box {
                            OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { appearance = true }) { Text("Theme") }
                            DropdownMenu(
                                expanded = appearance,
                                onDismissRequest = { appearance = false },
                            ) {
                                listOf("System" to null, "Light" to false, "Dark" to true)
                                    .forEach { (label, value) ->
                                        DropdownMenuItem(
                                            text = { Text(label) },
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
                if (model.busy) LinearProgressIndicator()
                if (model.error.isNotEmpty()) Text(model.error)
                if (!model.signedIn)
                    Column(Modifier.padding(24.dp)) {
                        Text(
                            "Your workspace, anywhere.",
                            style = MaterialTheme.typography.headlineLarge,
                        )
                        Button(shape = MaterialTheme.shapes.small,
                            enabled = !model.busy,
                            onClick = { model.run { login.launch(model.session.loginIntent()) } },
                        ) {
                            Text("Sign in")
                        }
                    }
                else {
                    Row(Modifier.horizontalScroll(rememberScrollState()).padding(horizontal = 16.dp, vertical = 4.dp), horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        destinations.forEach { (key, title) ->
                            FilterChip(
                                selected = model.section == key,
                                onClick = { model.section = key },
                                label = { Text(title) },
                                shape = androidx.compose.foundation.shape.RoundedCornerShape(22.dp),
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
                            Modifier.verticalScroll(rememberScrollState()).padding(16.dp),
                            verticalArrangement = Arrangement.spacedBy(16.dp),
                        ) {
                            when (model.section) {
                                "products" -> ProductScreen(model.api)
                                "files" -> StoredFileScreen(model.api)
                                "users" -> UserScreen(model.api)
                                "roles" -> RoleScreen(model.api)
                                "configurationentries" -> ConfigurationEntryScreen(model.api)
                                "auditlogentries" -> AuditLogsScreen(model.api)
                                "notifications" -> {
                                    Text("Notifications", style = MaterialTheme.typography.headlineSmall)
                                    Text(model.notificationStatus)
                                    if (model.feed.isEmpty())
                                        Text("Live messages appear here while the app is active.")
                                    model.feed.forEach { Text(it) }
                                }
                                else -> {
                                    HomeInfo()
                                    destinations.drop(1).take(5).forEach { (key, title) ->
                                        HomeButton(title) { model.section = key }
                                    }
                                    HomeButton("Sign out") { signOut = true }
                                }
                            }
                        }
                    }
                }
            }
        }
        if (signOut)
            AlertDialog(
                onDismissRequest = { signOut = false },
                title = { Text("Sign out of this device?") },
                dismissButton = { OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { signOut = false }) { Text("Cancel") } },
                confirmButton = {
                    Button(shape = MaterialTheme.shapes.small,
                        onClick = {
                            signOut = false
                            model.run {
                                model.setNotifications(false)
                                val intent = model.session.logoutIntent()
                                model.signedIn = false
                                model.feed = emptyList()
                                model.section = "home"
                                if (intent != null) logout.launch(intent)
                            }
                        }
                    ) {
                        Text("Confirm")
                    }
                },
            )
    }
}
