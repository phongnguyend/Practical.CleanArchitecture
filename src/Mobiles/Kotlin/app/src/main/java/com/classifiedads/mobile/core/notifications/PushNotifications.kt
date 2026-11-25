package com.classifiedads.mobile.core.notifications

import android.app.NotificationChannel
import android.app.NotificationManager
import android.content.Context
import android.content.Intent
import com.classifiedads.mobile.BuildConfig
import com.classifiedads.mobile.core.auth.Session
import com.classifiedads.mobile.core.network.Api
import com.google.android.gms.tasks.Task
import com.google.firebase.messaging.FirebaseMessaging
import com.google.firebase.messaging.FirebaseMessagingService
import com.google.firebase.messaging.RemoteMessage
import java.util.UUID
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException
import kotlinx.coroutines.suspendCancellableCoroutine
import kotlinx.coroutines.flow.MutableStateFlow

object PushEvents {
    val target = MutableStateFlow<Pair<String, String>?>(null)
    val received = MutableStateFlow(0L)
    val tokenChanged = MutableStateFlow(0L)
    private val sections = setOf("home", "products", "files", "users", "roles", "configurationentries", "auditlogentries", "notifications")

    fun open(intent: Intent?) {
        val id = intent?.getStringExtra("notificationId") ?: return
        if (!id.matches(Regex("[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}"))) {
            return
        }
        val section = intent.getStringExtra("section")
        target.value = id to if (section in sections) section!! else "notifications"
    }
}

class PushMessagingService : FirebaseMessagingService() {
    override fun onNewToken(token: String) {
        // The current authenticated foreground session registers the refreshed token.
        PushEvents.tokenChanged.value = System.currentTimeMillis()
    }

    override fun onMessageReceived(message: RemoteMessage) {
        if (BuildConfig.PUSH_NOTIFICATIONS_ENABLED) {
            PushEvents.received.value = System.currentTimeMillis()
        }
    }
}

class PushNotifications(context: Context, private val session: Session, private val api: Api) {
    private val preferences = context.getSharedPreferences("push", Context.MODE_PRIVATE)
    private val manager = context.getSystemService(NotificationManager::class.java)
    private var generation = 0

    private suspend fun <T> Task<T>.result(): T = suspendCancellableCoroutine { continuation ->
        addOnSuccessListener { value ->
            if (continuation.isActive) {
                continuation.resume(value)
            }
        }
        addOnFailureListener { error ->
            if (continuation.isActive) {
                continuation.resumeWithException(error)
            }
        }
    }

    suspend fun sync() {
        if (!BuildConfig.PUSH_NOTIFICATIONS_ENABLED || !session.signedIn || !preferences.getBoolean("enabled", false)) {
            return
        }
        val epoch = generation
        val token = FirebaseMessaging.getInstance().token.result()
        if (epoch != generation || !session.signedIn) {
            return
        }
        val id = preferences.getString("installation", null) ?: UUID.randomUUID().toString().also {
            preferences.edit().putString("installation", it).apply()
        }
        api.json("notifications/devices/$id", "PUT", org.json.JSONObject().put("app", "Kotlin").put("platform", "android").put("token", token))
    }

    suspend fun enable() {
        check(BuildConfig.PUSH_NOTIFICATIONS_ENABLED) { "Push notifications are not configured for this build." }
        val status = api.json("notifications/status") as org.json.JSONObject
        check(status.optBoolean("enabled")) { "Push notifications are not enabled on the server." }
        check(session.signedIn) { "Unlock your workspace before enabling notifications." }
        manager.createNotificationChannel(NotificationChannel("classifiedads_updates", "Classified Ads updates", NotificationManager.IMPORTANCE_DEFAULT))
        FirebaseMessaging.getInstance().isAutoInitEnabled = true
        preferences.edit().putBoolean("enabled", true).apply()
        sync()
    }

    suspend fun disable() {
        generation++
        PushEvents.target.value = null
        val id = preferences.getString("installation", null)
        preferences.edit().clear().apply()
        try {
            if (id != null && session.signedIn) {
                api.json("notifications/devices/$id", "DELETE")
            }
        } finally {
            if (BuildConfig.PUSH_NOTIFICATIONS_ENABLED) {
                FirebaseMessaging.getInstance().isAutoInitEnabled = false
                FirebaseMessaging.getInstance().deleteToken().result()
                manager.cancelAll()
            }
        }
    }
}
