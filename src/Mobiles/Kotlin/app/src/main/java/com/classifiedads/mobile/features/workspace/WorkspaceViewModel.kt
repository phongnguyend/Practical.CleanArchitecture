package com.classifiedads.mobile.features.workspace

import android.app.Application
import androidx.compose.runtime.*
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.classifiedads.mobile.BuildConfig
import com.classifiedads.mobile.core.auth.Session
import com.classifiedads.mobile.core.network.Api
import com.microsoft.signalr.*
import io.reactivex.rxjava3.core.Single
import kotlinx.coroutines.*
import kotlinx.coroutines.rx3.await

class WorkspaceViewModel(application: Application) : AndroidViewModel(application) {
    val session = Session(application)
    val api = Api(application, session)
    var signedIn by mutableStateOf(session.signedIn)
    var section by mutableStateOf("home")
    var busy by mutableStateOf(false)
    var error by mutableStateOf("")
    var message by mutableStateOf("")
    var feed by mutableStateOf(listOf<String>())
    var notificationStatus by mutableStateOf("Connecting…")
    private var hub: HubConnection? = null
    private var notifications: Job? = null
    private val preferences =
        application.getSharedPreferences("appearance", android.content.Context.MODE_PRIVATE)
    var dark by
        mutableStateOf<Boolean?>(
            when (preferences.getString("theme", "system")) {
                "dark" -> true
                "light" -> false
                else -> null
            }
        )

    fun chooseTheme(value: Boolean?) {
        dark = value
        preferences
            .edit()
            .putString(
                "theme",
                when (value) {
                    true -> "dark"
                    false -> "light"
                    else -> "system"
                },
            )
            .apply()
    }

    fun run(action: suspend () -> Unit) {
        if (busy) return
        busy = true
        error = ""
        viewModelScope.launch {
            try {
                action()
            } catch (e: Exception) {
                error = e.message ?: e.toString()
            } finally {
                busy = false
            }
        }
    }

    fun setNotifications(active: Boolean) {
        notifications?.cancel()
        hub?.stop()?.subscribe({}, {})
        hub = null
        if (!active || !signedIn) return
        val connection =
            HubConnectionBuilder.create(BuildConfig.NOTIFICATION_URL)
                .withAccessTokenProvider(Single.fromCallable { runBlocking { session.token() } })
                .build()
        hub = connection
        connection.on(
            "ReceiveMessage",
            { text: String ->
                viewModelScope.launch {
                    message = text
                    feed = (listOf(text) + feed).take(100)
                }
            },
            String::class.java,
        )
        notifications =
            viewModelScope.launch(Dispatchers.IO) {
                while (isActive) {
                    try {
                        if (connection.connectionState == HubConnectionState.DISCONNECTED)
                            connection.start().await()
                        withContext(Dispatchers.Main) { notificationStatus = "Connected" }
                    } catch (e: CancellationException) {
                        throw e
                    } catch (_: Exception) {
                        withContext(Dispatchers.Main) { notificationStatus = "Reconnecting…" }
                    }
                    delay(10000)
                }
            }
    }

    override fun onCleared() {
        setNotifications(false)
        session.close()
        super.onCleared()
    }
}
