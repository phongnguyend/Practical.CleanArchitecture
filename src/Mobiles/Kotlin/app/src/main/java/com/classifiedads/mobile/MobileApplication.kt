package com.classifiedads.mobile

import android.app.Application
import com.classifiedads.mobile.core.diagnostics.CrashReporting

class MobileApplication : Application() {
    override fun onCreate() {
        super.onCreate()
        CrashReporting.initialize(this)
    }
}
