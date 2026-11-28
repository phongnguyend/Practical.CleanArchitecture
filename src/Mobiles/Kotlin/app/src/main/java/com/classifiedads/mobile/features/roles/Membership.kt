package com.classifiedads.mobile.features.roles

import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.semantics.clearAndSetSemantics
import androidx.compose.ui.semantics.contentDescription
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.R
import com.classifiedads.mobile.core.network.Api
import com.classifiedads.mobile.shared.ui.Sheet
import kotlinx.coroutines.CancellationException
import kotlinx.coroutines.launch
import org.json.JSONArray
import org.json.JSONObject

@OptIn(ExperimentalLayoutApi::class)
@Composable
fun Membership(api: Api, roleId: String? = null, userId: String? = null) {
    val scope = rememberCoroutineScope()
    var items by remember(roleId, userId) { mutableStateOf<List<JSONObject>>(emptyList()) }
    var users by remember(roleId, userId) { mutableStateOf<List<JSONObject>>(emptyList()) }
    var search by remember { mutableStateOf("") }
    var selected by remember { mutableStateOf<JSONObject?>(null) }
    var removing by remember { mutableStateOf<JSONObject?>(null) }
    var busy by remember { mutableStateOf(false) }
    var error by remember { mutableStateOf("") }
    fun rows(value: Any?): List<JSONObject> {
        val array = value as JSONArray
        return (0 until array.length()).map { array.getJSONObject(it) }
    }
    suspend fun load() {
        busy = true
        error = ""
        try {
            items = rows(api.json(if (roleId != null) {
                "roles/$roleId/users"
            } else {
                "users/$userId/roles"
            }))
            if (roleId != null) {
                users = rows(api.json("users"))
            }
        } catch (cancelled: CancellationException) {
            throw cancelled
        } catch (_: Exception) {
            error = "Unable to load role membership. Please try again."
        } finally {
            busy = false
        }
    }
    suspend fun change(user: JSONObject, assigned: Boolean) {
        if (busy) {
            return
        }
        busy = true
        error = ""
        try {
            api.json("roles/$roleId/users/${user.getString("id")}", if (assigned) {
                "PUT"
            } else {
                "DELETE"
            }, if (assigned) {
                JSONObject()
            } else {
                null
            })
            selected = null
            search = ""
            removing = null
            load()
        } catch (cancelled: CancellationException) {
            throw cancelled
        } catch (_: Exception) {
            removing = null
            error = "Unable to update role membership. Refresh and try again."
        } finally {
            busy = false
        }
    }
    LaunchedEffect(roleId, userId) { load() }
    val candidates = users.filter { user -> items.none { it.getString("id") == user.getString("id") }
        && "${user.optString("userName")} ${user.optString("email")}".contains(search, ignoreCase = true) }
    Column(Modifier.fillMaxWidth(), verticalArrangement = Arrangement.spacedBy(16.dp)) {
        HorizontalDivider()
        Row(Modifier.fillMaxWidth(), horizontalArrangement = Arrangement.spacedBy(12.dp), verticalAlignment = androidx.compose.ui.Alignment.CenterVertically) {
            Text(if (roleId != null) {
                "Role users"
            } else {
                "Assigned roles"
            }, style = MaterialTheme.typography.titleMedium, modifier = Modifier.weight(1f))
            OutlinedIconButton(onClick = { scope.launch { load() } }, enabled = !busy) {
                Icon(painterResource(R.drawable.action_refresh), contentDescription = if (roleId != null) {
                    "Refresh users"
                } else {
                    "Refresh roles"
                }, modifier = Modifier.size(20.dp))
            }
        }
        if (busy) {
            Text("Loading...")
        }
        if (error.isNotEmpty()) {
            Text(error, color = MaterialTheme.colorScheme.error)
        }
        if (roleId != null) {
            Text("Search by username or email to assign someone to this role.")
            OutlinedTextField(value = search, onValueChange = { search = it; selected = null }, label = { Text("Search users") }, modifier = Modifier.fillMaxWidth())
            Text("User to assign: ${selected?.optString("userName") ?: "Select a user"}")
            if (search.isNotBlank() && selected == null) {
                candidates.take(5).forEach { user ->
                    MembershipButton("Select ${user.optString("userName")}", R.drawable.action_users, !busy) { selected = user }
                }
            }
            if (candidates.isEmpty() && search.isNotBlank()) {
                Text("No available users match your search.")
            }
            if (candidates.size > 5 && search.isNotBlank() && selected == null) {
                Text("Keep typing to narrow the results.")
            }
            MembershipButton("Assign user", R.drawable.action_add, !busy && selected != null) { selected?.let { user -> scope.launch { change(user, true) } } }
        }
        if (!busy && error.isEmpty() && items.isEmpty()) {
            Text(if (roleId != null) {
                "No users assigned."
            } else {
                "No roles assigned."
            })
        }
        items.forEach { item ->
            HorizontalDivider()
            Column(Modifier.fillMaxWidth(), verticalArrangement = Arrangement.spacedBy(12.dp)) {
                Text(item.optString(if (roleId != null) {
                    "userName"
                } else {
                    "name"
                }))
                if (roleId != null) {
                    Text(item.optString("email"))
                    MembershipButton("Remove ${item.optString("userName")} from role", R.drawable.action_delete, !busy, true) {
                        removing = item
                    }
                }
            }
        }
    }
    removing?.let { user ->
        Sheet("Remove assigned role", close = {
            if (!busy) {
                removing = null
            }
        }, footer = {
            MembershipButton("Cancel", R.drawable.action_close, !busy) { removing = null }
            MembershipButton("Remove", R.drawable.action_delete, !busy, true) { scope.launch { change(user, false) } }
        }) {
            Text("Remove ${user.optString("userName")} from this role?")
        }
    }
}

@Composable
private fun MembershipButton(label: String, icon: Int, enabled: Boolean, danger: Boolean = false, action: () -> Unit) {
    OutlinedButton(onClick = action, enabled = enabled, modifier = Modifier.fillMaxWidth().heightIn(min = 48.dp),
        colors = ButtonDefaults.outlinedButtonColors(contentColor = if (danger) {
                MaterialTheme.colorScheme.error
            } else {
                MaterialTheme.colorScheme.primary
            })) {
        Icon(painterResource(icon), contentDescription = null, modifier = Modifier.size(18.dp))
        Spacer(Modifier.width(8.dp))
        Text(if (danger && label.startsWith("Remove ")) {
            "Remove"
        } else {
            label
        }, modifier = Modifier.clearAndSetSemantics { contentDescription = label })
    }
}
