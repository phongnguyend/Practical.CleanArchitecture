package com.classifiedads.mobile.core.auth

import android.app.Activity
import android.content.Context
import android.hardware.biometrics.BiometricManager
import android.hardware.biometrics.BiometricPrompt
import android.os.Build
import android.os.CancellationSignal
import android.security.keystore.KeyGenParameterSpec
import android.security.keystore.KeyProperties
import android.util.Base64
import java.security.KeyPairGenerator
import java.security.KeyStore
import java.security.SecureRandom
import java.security.spec.MGF1ParameterSpec
import javax.crypto.Cipher
import javax.crypto.spec.GCMParameterSpec
import javax.crypto.spec.OAEPParameterSpec
import javax.crypto.spec.PSource
import javax.crypto.spec.SecretKeySpec
import kotlinx.coroutines.suspendCancellableCoroutine
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException
import org.json.JSONObject

/** Public-key envelope encryption permits token rotation without a second prompt.
 * The private key can decrypt only inside a strong-biometric CryptoObject operation. */
class BiometricVault(private val context: Context) {
    private val preferences = context.getSharedPreferences("biometric_session", Context.MODE_PRIVATE)
    private val alias = "classifiedads.biometric.session"
    private val store get() = KeyStore.getInstance("AndroidKeyStore").apply { load(null) }
    private val oaep = OAEPParameterSpec("SHA-256", "MGF1", MGF1ParameterSpec.SHA1, PSource.PSpecified.DEFAULT)
    var enabled: Boolean
        get() = preferences.getBoolean("enabled", false)
        set(value) {
            check(preferences.edit().putBoolean("enabled", value).commit())
        }

    fun available(): Boolean = Build.VERSION.SDK_INT >= 30 &&
        context.getSystemService(BiometricManager::class.java)
            .canAuthenticate(BiometricManager.Authenticators.BIOMETRIC_STRONG) == BiometricManager.BIOMETRIC_SUCCESS

    fun create() {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.R) {
            throw IllegalStateException("Biometric login requires Android 11 or later.")
        }
        check(available()) { "Enroll a strong biometric on this device first (Android 11 or later)." }
        val generator = KeyPairGenerator.getInstance(KeyProperties.KEY_ALGORITHM_RSA, "AndroidKeyStore")
        generator.initialize(KeyGenParameterSpec.Builder(alias, KeyProperties.PURPOSE_DECRYPT)
            .setKeySize(2048)
            .setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_RSA_OAEP)
            .setDigests(KeyProperties.DIGEST_SHA256, KeyProperties.DIGEST_SHA1)
            .setUserAuthenticationRequired(true)
            .setUserAuthenticationParameters(0, KeyProperties.AUTH_BIOMETRIC_STRONG)
            .setInvalidatedByBiometricEnrollment(true)
            .build())
        generator.generateKeyPair()
    }

    fun save(value: String) {
        val key = ByteArray(32).also { SecureRandom().nextBytes(it) }
        try {
            val cipher = Cipher.getInstance("AES/GCM/NoPadding")
            cipher.init(Cipher.ENCRYPT_MODE, SecretKeySpec(key, "AES"))
            val wrapping = Cipher.getInstance("RSA/ECB/OAEPWithSHA-256AndMGF1Padding")
            wrapping.init(Cipher.ENCRYPT_MODE, store.getCertificate(alias).publicKey, oaep)
            val data = JSONObject().put("key", encode(wrapping.doFinal(key)))
                .put("iv", encode(cipher.iv)).put("data", encode(cipher.doFinal(value.toByteArray(Charsets.UTF_8))))
            check(preferences.edit().putString("data", data.toString()).commit())
        } finally {
            key.fill(0)
        }
    }

    suspend fun read(activity: Activity): String {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.R) {
            throw IllegalStateException("Biometric login requires Android 11 or later. Sign in with Identity Server.")
        }
        check(available()) { "Biometrics unavailable. Sign in with Identity Server." }
        val data = JSONObject(checkNotNull(preferences.getString("data", null)))
        val cipher = Cipher.getInstance("RSA/ECB/OAEPWithSHA-256AndMGF1Padding")
        cipher.init(Cipher.DECRYPT_MODE, store.getKey(alias, null), oaep)
        return suspendCancellableCoroutine { continuation ->
            val cancellation = CancellationSignal()
            continuation.invokeOnCancellation { cancellation.cancel() }
            activity.runOnUiThread {
                val prompt = BiometricPrompt.Builder(activity)
                    .setTitle("Unlock Classified Ads")
                    .setAllowedAuthenticators(BiometricManager.Authenticators.BIOMETRIC_STRONG)
                    .setNegativeButton("Cancel", activity.mainExecutor) { _, _ ->
                        if (continuation.isActive) {
                            continuation.resumeWithException(IllegalStateException("Biometric unlock cancelled."))
                        }
                    }.build()
                prompt.authenticate(BiometricPrompt.CryptoObject(cipher), cancellation, activity.mainExecutor,
                    object : BiometricPrompt.AuthenticationCallback() {
                        override fun onAuthenticationError(code: Int, message: CharSequence) {
                            if (continuation.isActive) {
                                continuation.resumeWithException(IllegalStateException("Biometric unlock unavailable or cancelled. Try again or sign in."))
                            }
                        }

                        override fun onAuthenticationSucceeded(result: BiometricPrompt.AuthenticationResult) {
                            if (!continuation.isActive) {
                                return
                            }
                            try {
                                val key = result.cryptoObject!!.cipher!!.doFinal(decode(data.getString("key")))
                                try {
                                    val decrypt = Cipher.getInstance("AES/GCM/NoPadding")
                                    decrypt.init(Cipher.DECRYPT_MODE, SecretKeySpec(key, "AES"), GCMParameterSpec(128, decode(data.getString("iv"))))
                                    continuation.resume(String(decrypt.doFinal(decode(data.getString("data"))), Charsets.UTF_8))
                                } finally {
                                    key.fill(0)
                                }
                            } catch (error: Exception) {
                                continuation.resumeWithException(IllegalStateException("Credentials changed. Sign in again.", error))
                            }
                        }
                    })
            }
        }
    }

    fun clear() {
        check(preferences.edit().clear().commit())
        store.deleteEntry(alias)
    }

    private fun encode(value: ByteArray) = Base64.encodeToString(value, Base64.NO_WRAP)
    private fun decode(value: String) = Base64.decode(value, Base64.NO_WRAP)
}
