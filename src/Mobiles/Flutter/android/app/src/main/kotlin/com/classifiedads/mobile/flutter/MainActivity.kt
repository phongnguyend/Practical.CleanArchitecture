package com.classifiedads.mobile.flutter

import io.flutter.embedding.android.FlutterFragmentActivity
import io.flutter.embedding.engine.FlutterEngine
import io.flutter.plugin.common.MethodChannel
import java.security.KeyStore

class MainActivity : FlutterFragmentActivity() {
    override fun configureFlutterEngine(flutterEngine: FlutterEngine) {
        super.configureFlutterEngine(flutterEngine)
        MethodChannel(flutterEngine.dartExecutor.binaryMessenger, "classifiedads/biometrics")
            .setMethodCallHandler { call, result ->
                if (call.method != "clear") {
                    result.notImplemented()
                } else {
                    try {
                        // flutter_secure_storage 11's dedicated namespace. Erase without
                        // initializing its biometric cipher, which may already be invalid.
                        val namespace = "classifiedads_biometric"
                        check(getSharedPreferences(namespace, MODE_PRIVATE).edit().clear().commit())
                        check(getSharedPreferences("FlutterSecureKeyStorage:$namespace", MODE_PRIVATE).edit().clear().commit())
                        val store = KeyStore.getInstance("AndroidKeyStore").apply { load(null) }
                        store.deleteEntry("$packageName.FlutterSecureStoragePluginKey.$namespace")
                        result.success(null)
                    } catch (error: Exception) {
                        result.error("biometric_cleanup", "Unable to erase the saved biometric session.", null)
                    }
                }
            }
    }
}
