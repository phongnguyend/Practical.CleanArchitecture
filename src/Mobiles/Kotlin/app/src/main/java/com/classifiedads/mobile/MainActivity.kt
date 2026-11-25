package com.classifiedads.mobile

import android.os.Bundle
import android.content.Intent
import com.classifiedads.mobile.core.notifications.PushEvents
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import com.classifiedads.mobile.features.workspace.MobileApp

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        PushEvents.open(intent)
        setContent { MobileApp() }
    }
    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)
        setIntent(intent)
        PushEvents.open(intent)
    }
}
