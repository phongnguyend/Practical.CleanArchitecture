package com.classifiedads.mobile.core.diagnostics

import android.content.Context
import com.classifiedads.mobile.BuildConfig
import com.google.firebase.FirebaseApp
import com.google.firebase.crashlytics.FirebaseCrashlytics
import java.util.concurrent.CancellationException

object CrashReporting {
    private var client: FirebaseCrashlytics? = null

    fun initialize(context: Context) {
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
    }
}
