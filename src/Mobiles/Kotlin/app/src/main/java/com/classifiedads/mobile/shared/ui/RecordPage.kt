package com.classifiedads.mobile.shared.ui

import android.net.Uri
import androidx.activity.compose.rememberLauncherForActivityResult
import androidx.activity.result.contract.ActivityResultContracts
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.text.selection.SelectionContainer
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.shared.model.*
import kotlinx.coroutines.launch
import org.json.JSONArray
import org.json.JSONObject

private data class RecordConfirmation(
    val text: String,
    val destructive: Boolean = false,
    val action: suspend () -> Unit,
)

@Composable
fun RecordValue(label: String, value: Any?, changed: Boolean = false) {
    Column(Modifier.padding(vertical = 6.dp)) {
        Text(
            label + if (changed) " · Changed" else "",
            style = MaterialTheme.typography.labelMedium,
        )
        SelectionContainer {
            Text(
                value?.toString()?.ifEmpty { "—" } ?: "—",
                color =
                    if (changed) MaterialTheme.colorScheme.error
                    else MaterialTheme.colorScheme.onSurface,
            )
        }
    }
}

@Composable
fun TextInput(
    label: String,
    value: String,
    changed: (String) -> Unit,
    disabled: Boolean = false,
    multiline: Boolean = false,
    secret: Boolean = false,
    numeric: Boolean = false,
) {
    OutlinedTextField(
        value = value,
        onValueChange = changed,
        label = { Text(label) },
        enabled = !disabled,
        modifier = Modifier.fillMaxWidth(),
        singleLine = !multiline,
        visualTransformation =
            if (secret) androidx.compose.ui.text.input.PasswordVisualTransformation()
            else androidx.compose.ui.text.input.VisualTransformation.None,
        keyboardOptions =
            androidx.compose.foundation.text.KeyboardOptions(
                keyboardType =
                    if (numeric) androidx.compose.ui.text.input.KeyboardType.Number
                    else androidx.compose.ui.text.input.KeyboardType.Text
            ),
    )
}

@Composable
fun ToggleInput(
    label: String,
    value: Boolean,
    changed: (Boolean) -> Unit,
    disabled: Boolean = false,
) {
    Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.SpaceBetween) {
        Text(label)
        Switch(checked = value, onCheckedChange = changed, enabled = !disabled)
    }
}

fun localDate(value: String) =
    runCatching {
            java.time.OffsetDateTime.parse(value)
                .atZoneSameInstant(java.time.ZoneId.systemDefault())
                .format(
                    java.time.format.DateTimeFormatter.ofLocalizedDateTime(
                        java.time.format.FormatStyle.MEDIUM
                    )
                )
        }
        .getOrDefault(value)

@OptIn(ExperimentalLayoutApi::class)
@Composable
fun <T : Entity> RecordPage(
    title: String,
    repository: RecordRepository<T>,
    empty: () -> T,
    label: (T) -> String,
    searchText: (T) -> String,
    serialize: (T) -> JSONObject,
    validate: (T) -> String?,
    form: @Composable (T, (T) -> Unit, Uri?, () -> Unit, Boolean) -> Unit,
    summary: @Composable (T) -> Unit,
    details: @Composable (T) -> Unit,
    history: (suspend (String) -> JSONArray)? = null,
    historyBody: (@Composable (JSONObject) -> Unit)? = null,
    toolbar: @Composable (() -> Unit) -> Unit = {},
    actions: @Composable (T) -> Unit = {},
    createTitle: String = "Add new",
) {
    var rows by remember { mutableStateOf(emptyList<T>()) }
    var loading by remember { mutableStateOf(true) }
    var working by remember { mutableStateOf(false) }
    var error by remember { mutableStateOf("") }
    var notice by remember { mutableStateOf("") }
    var search by remember { mutableStateOf("") }
    var page by remember { mutableIntStateOf(1) }
    var refresh by remember { mutableIntStateOf(0) }
    var editor by remember { mutableStateOf<T?>(null) }
    var initial by remember { mutableStateOf("") }
    var picked by remember { mutableStateOf<Uri?>(null) }
    var detail by remember { mutableStateOf<T?>(null) }
    var audits by remember { mutableStateOf<JSONArray?>(null) }
    var confirmation by remember { mutableStateOf<RecordConfirmation?>(null) }
    val scope = rememberCoroutineScope()
    val busy = loading || working
    fun run(action: suspend () -> Unit) {
        if (working) return
        working = true
        error = ""
        scope.launch {
            try {
                action()
            } catch (e: Exception) {
                error = e.message ?: e.toString()
            } finally {
                working = false
            }
        }
    }
    fun edit(value: T) {
        initial = serialize(value).toString()
        editor = value
        picked = null
        error = ""
    }
    fun closeEditor() {
        val current = editor ?: return
        if (picked != null || serialize(current).toString() != initial)
            confirmation = RecordConfirmation("Discard unsaved changes?") { editor = null }
        else editor = null
    }
    val picker =
        rememberLauncherForActivityResult(ActivityResultContracts.OpenDocument()) { picked = it }
    LaunchedEffect(repository, refresh) {
        loading = true
        try {
            rows = repository.list()
            error = ""
        } catch (e: Exception) {
            error = e.message ?: e.toString()
        } finally {
            loading = false
        }
    }
    Column(verticalArrangement = Arrangement.spacedBy(16.dp)) {
        Text(title, style = MaterialTheme.typography.headlineSmall)
        TextInput(
            "Search current records",
            search,
            {
                search = it
                page = 1
            },
        )
        FlowRow(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
            OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { refresh++ }) { Text("Refresh") }
            Button(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { edit(empty()) }) { Text(createTitle) }
            toolbar { refresh++ }
        }
        if (busy) LinearProgressIndicator(Modifier.fillMaxWidth())
        if (error.isNotEmpty()) Text(error, color = MaterialTheme.colorScheme.error)
        if (notice.isNotEmpty()) Text(notice)
        val filtered = rows.filter { searchText(it).contains(search, true) }
        if (filtered.isEmpty() && !busy) Text("No records found.")
        filtered.drop((page - 1) * 20).take(20).forEach { value ->
            OutlinedCard(Modifier.fillMaxWidth(), shape = MaterialTheme.shapes.medium) {
                Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(8.dp)) {
                    Text(label(value), style = MaterialTheme.typography.titleLarge)
                    summary(value)
                    FlowRow(horizontalArrangement = Arrangement.spacedBy(8.dp)) {
                        OutlinedButton(shape = MaterialTheme.shapes.small,
                            enabled = !busy,
                            onClick = { run { detail = repository.get(value.id) } },
                        ) {
                            Text("View")
                        }
                        OutlinedButton(shape = MaterialTheme.shapes.small,
                            enabled = !busy,
                            onClick = { run { edit(repository.get(value.id)) } },
                        ) {
                            Text("Edit")
                        }
                        OutlinedButton(shape = MaterialTheme.shapes.small,
                            enabled = !busy,
                            colors = dangerButtonColors(),
                            border = dangerButtonBorder(),
                            onClick = {
                                confirmation =
                                    RecordConfirmation("Delete ${label(value)}? This cannot be undone.", destructive = true)
                                        {
                                            repository.remove(value.id)
                                            refresh++
                                            notice = "Deleted"
                                        }
                            },
                        ) {
                            Text("Delete")
                        }
                        if (history != null)
                            OutlinedButton(shape = MaterialTheme.shapes.small,
                                enabled = !busy,
                                onClick = { run { audits = history(value.id) } },
                            ) {
                                Text("History")
                            }
                        actions(value)
                    }
                }
            }
        }
        Row {
            if (page > 1) OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { page-- }) { Text("Previous") }
            Text("Page $page · ${filtered.size} records")
            if (page * 20 < filtered.size) OutlinedButton(shape = MaterialTheme.shapes.small, onClick = { page++ }) { Text("Next") }
        }
    }
    if (confirmation != null || editor != null || detail != null || audits != null)
        Sheet(
            if (confirmation != null) "Confirm action"
            else if (editor != null) "${if(editor!!.id.isEmpty())"Add" else "Edit"} $title"
            else if (audits != null) "Audit history" else "Details",
            {
                if (!busy) {
                    if (confirmation != null) confirmation = null
                    else if (editor != null) closeEditor()
                    else {
                        detail = null
                        audits = null
                    }
                }
            },
            footer = {
                if (confirmation != null) {
                    OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { confirmation = null }) {
                        Text("Cancel")
                    }
                    Button(shape = MaterialTheme.shapes.small,
                        enabled = !busy,
                        colors = if (confirmation!!.destructive) dangerButtonColors() else ButtonDefaults.buttonColors(),
                        border = if (confirmation!!.destructive) dangerButtonBorder() else null,
                        onClick = {
                            val action = confirmation!!.action
                            confirmation = null
                            run(action)
                        },
                    ) {
                        Text("Confirm")
                    }
                } else if (editor != null) {
                    OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { closeEditor() }) { Text("Cancel") }
                    Button(shape = MaterialTheme.shapes.small,
                        enabled = !busy,
                        onClick = {
                            val value = editor!!
                            val message = validate(value)
                            if (message != null) error = message
                            else
                                run {
                                    repository.save(value, picked)
                                    editor = null
                                    refresh++
                                    notice = "Saved successfully"
                                }
                        },
                    ) {
                        Text("Save")
                    }
                } else {
                    OutlinedButton(shape = MaterialTheme.shapes.small,
                        onClick = {
                            detail = null
                            audits = null
                        }
                    ) {
                        Text("Back")
                    }
                    detail?.let { value ->
                        Button(shape = MaterialTheme.shapes.small,
                            onClick = {
                                detail = null
                                edit(value)
                            }
                        ) {
                            Text("Edit")
                        }
                    }
                }
            },
        ) {
            when {
                confirmation != null -> Text(confirmation!!.text)
                editor != null ->
                    form(editor!!, { editor = it }, picked, { picker.launch(arrayOf("*/*")) }, busy)
                detail != null -> details(detail!!)
                audits != null -> {
                    if (audits!!.length() == 0) Text("No audit entries.")
                    audits!!.objects().forEach { entry ->
                        Card {
                            Column(Modifier.padding(12.dp)) {
                                Text(
                                    "${entry.optString("createdDateTime")} · ${entry.optString("userName")} · ${entry.optString("action")}"
                                )
                                historyBody?.invoke(entry)
                                JsonViewer(auditSnapshot(entry))
                            }
                        }
                    }
                }
            }
            if (error.isNotEmpty()) Text(error, color = MaterialTheme.colorScheme.error)
            if (busy) LinearProgressIndicator()
        }
}

@Composable
fun ImportButton(
    suffix: String,
    fileName: (Uri) -> String,
    submit: suspend (Uri) -> Unit,
    done: () -> Unit,
) {
    var open by remember { mutableStateOf(false) }
    var file by remember { mutableStateOf<Uri?>(null) }
    var busy by remember { mutableStateOf(false) }
    var error by remember { mutableStateOf("") }
    val scope = rememberCoroutineScope()
    val picker =
        rememberLauncherForActivityResult(ActivityResultContracts.OpenDocument()) { file = it }
    OutlinedButton(shape = MaterialTheme.shapes.small,
        onClick = {
            file = null
            error = ""
            open = true
        }
    ) {
        Text("Import")
    }
    if (open)
        Sheet(
            "Import $suffix",
            { if (!busy) open = false },
            footer = {
                OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { open = false }) { Text("Cancel") }
                Button(shape = MaterialTheme.shapes.small,
                    enabled = !busy,
                    onClick = {
                        val chosen = file
                        if (chosen == null || !fileName(chosen).lowercase().endsWith(suffix))
                            error = "Choose a $suffix file."
                        else {
                            busy = true
                            scope.launch {
                                try {
                                    submit(chosen)
                                    done()
                                    open = false
                                } catch (e: Exception) {
                                    error = e.message ?: e.toString()
                                } finally {
                                    busy = false
                                }
                            }
                        }
                    },
                ) {
                    Text("Import")
                }
            },
        ) {
            OutlinedButton(shape = MaterialTheme.shapes.small, enabled = !busy, onClick = { picker.launch(arrayOf("*/*")) }) {
                Text("Choose file")
            }
            Text(file?.let(fileName) ?: "No file selected")
            if (error.isNotEmpty()) Text(error)
        }
}

@Composable
fun OperationButton(title: String, action: suspend () -> Unit) {
    val scope = rememberCoroutineScope()
    var busy by remember { mutableStateOf(false) }
    var error by remember { mutableStateOf("") }
    Column {
        OutlinedButton(shape = MaterialTheme.shapes.small,
            enabled = !busy,
            onClick = {
                busy = true
                scope.launch {
                    try {
                        action()
                        error = ""
                    } catch (e: Exception) {
                        error = e.message ?: e.toString()
                    } finally {
                        busy = false
                    }
                }
            },
        ) {
            Text(title)
        }
        if (error.isNotEmpty()) Text(error, color = MaterialTheme.colorScheme.error)
    }
}
