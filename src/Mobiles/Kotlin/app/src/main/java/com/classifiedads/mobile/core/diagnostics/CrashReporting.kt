package com.classifiedads.mobile.core.diagnostics

import io.sentry.Sentry
import io.sentry.android.core.SentryAndroid
import android.content.Context
import com.classifiedads.mobile.BuildConfig
import com.google.firebase.FirebaseApp
import com.google.firebase.crashlytics.FirebaseCrashlytics
import java.util.concurrent.CancellationException

object CrashReporting {
    private var client: FirebaseCrashlytics? = null

    fun initialize(context: Context) {
        if (BuildConfig.SENTRY_ENABLED && BuildConfig.SENTRY_DSN.isNotBlank() && !BuildConfig.DEBUG) {
            SentryAndroid.init(context) { options ->
                options.dsn = BuildConfig.SENTRY_DSN
                options.environment = BuildConfig.SENTRY_ENVIRONMENT
                if (BuildConfig.SENTRY_RELEASE.isNotBlank()) {
                    options.release = BuildConfig.SENTRY_RELEASE
                }
                options.isSendDefaultPii = false
                options.maxBreadcrumbs = 0
                options.tracesSampleRate = 0.0
                options.setBeforeSend { event, _ ->
                    event.user = null
                    event.request = null
                    event.message = null
                    event.breadcrumbs?.clear()
                    event.extras?.clear()
                    event.exceptions?.forEach { exception ->
                        exception.value = exception.type ?: "Unexpected error"
                    }
                    event
                }
            }
        }
        if (FirebaseApp.initializeApp(context) == null) {
            return
        }
        val crashlytics = FirebaseCrashlytics.getInstance()
        crashlytics.setCrashlyticsCollectionEnabled(BuildConfig.CRASHLYTICS_ENABLED)
        if (BuildConfig.CRASHLYTICS_ENABLED) {
            crashlytics.setCustomKey("framework", "Kotlin")
            client = crashlytics
        }
    }

    // Call only for unexpected failures. Never attach API bodies or user input.
    fun record(error: Throwable) {
        if (error is CancellationException) {
            return
        }
        val safeError = RuntimeException(error.javaClass.simpleName)
        safeError.stackTrace = error.stackTrace
        client?.recordException(safeError)
        if (Sentry.isEnabled()) {
            Sentry.captureException(error)
        }
    }
}
