package com.classifiedads.mobile.core.auth

import android.content.Context
import android.content.Intent
import android.net.Uri
import com.classifiedads.mobile.BuildConfig
import java.io.IOException
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException
import kotlinx.coroutines.suspendCancellableCoroutine
import kotlinx.coroutines.sync.Mutex
import kotlinx.coroutines.sync.withLock
import net.openid.appauth.*

class Session(private val context: Context) {
    private val vault = TokenVault(context)
    private val biometric = BiometricVault(context)
    private var generation = 0
    val biometricEnabled get() = biometric.enabled
    fun biometricAvailable() = biometric.available()
    fun lock(): Boolean {
        generation++
        if (!biometricEnabled) {
            return false
        }
        state = AuthState()
        return true
    }
    suspend fun enableBiometrics(activity: android.app.Activity) {
        check(signedIn) { "Please sign in first." }
        val epoch = generation
        val value = state.jsonSerializeString()
        biometric.create()
        biometric.save(value)
        check(biometric.read(activity) == value && epoch == generation)
        biometric.enabled = true
        vault.save(null)
    }
    suspend fun disableBiometrics(activity: android.app.Activity) {
        check(signedIn) { "Unlock before changing biometric settings." }
        val epoch = generation
        biometric.read(activity)
        check(epoch == generation && signedIn) { "Session changed. Unlock again." }
        vault.save(state.jsonSerializeString())
        biometric.clear()
    }
    suspend fun unlock(activity: android.app.Activity) {
        check(biometricEnabled)
        val epoch = generation
        val data = biometric.read(activity)
        check(epoch == generation) { "Session changed. Unlock again." }
        state = AuthState.jsonDeserialize(data)
        try {
            token()
        } catch (error: Exception) {
            state = AuthState()
            throw error
        }
    }
    private fun persist(value: AuthState) {
        if (biometricEnabled) {
            biometric.save(value.jsonSerializeString())
        } else {
            vault.save(value.jsonSerializeString())
        }
    }
    private val service = AuthorizationService(context)
    private var state =
        (if (biometricEnabled) null else vault.read())?.let { runCatching { AuthState.jsonDeserialize(it) }.getOrNull() }
            ?: AuthState()
    private val mutex = Mutex()
    val signedIn
        get() = state.isAuthorized

    suspend fun loginIntent(): Intent {
        biometric.clear()
        vault.save(null)
        state = AuthState()
        val configuration =
            suspendCancellableCoroutine<AuthorizationServiceConfiguration> { continuation ->
                AuthorizationServiceConfiguration.fetchFromIssuer(
                    Uri.parse(BuildConfig.AUTHORITY)
                ) { config, error ->
                    if (continuation.isActive) {
                        if (config != null) {
                            continuation.resume(config)
                        } else {
                            continuation.resumeWithException(error ?: IOException("Discovery failed"))
                        }
                    }
                }
            }
        val request =
            AuthorizationRequest.Builder(
                    configuration,
                    "ClassifiedAds.Mobile.Kotlin",
                    ResponseTypeValues.CODE,
                    Uri.parse("classifiedads.kotlin:/oauthredirect"),
                )
                .setScopes("openid", "profile", "offline_access", "ClassifiedAds.WebAPI")
                .build()
        return service.getAuthorizationRequestIntent(request)
    }

    suspend fun complete(intent: Intent?) {
        requireNotNull(intent) { "Sign-in cancelled" }
        val response = AuthorizationResponse.fromIntent(intent)
        val error = AuthorizationException.fromIntent(intent)
        if (response == null) {
            throw error ?: IOException("Sign-in cancelled")
        }
        state = AuthState(response, error)
        suspendCancellableCoroutine<Unit> { continuation ->
            service.performTokenRequest(response.createTokenExchangeRequest()) { tokens, failure ->
                try {
                    state.update(tokens, failure)
                    persist(state)
                    if (continuation.isActive) {
                        if (failure != null) {
                            continuation.resumeWithException(failure)
                        } else {
                            continuation.resume(Unit)
                        }
                    }
                } catch (error: Exception) {
                    if (continuation.isActive) {
                        continuation.resumeWithException(error)
                    }
                }
            }
        }
    }

    suspend fun token(): String = mutex.withLock {
        check(signedIn) { "Please sign in." }
        val previous = state
        suspendCancellableCoroutine { continuation ->
            previous.performActionWithFreshTokens(service) { token, _, failure ->
                if (state !== previous) {
                    if (continuation.isActive) {
                        continuation.resumeWithException(IOException("Session locked. Please unlock again."))
                    }
                    return@performActionWithFreshTokens
                }
                try {
                    persist(previous)
                } catch (error: Exception) {
                    if (continuation.isActive) {
                        continuation.resumeWithException(error)
                    }
                    return@performActionWithFreshTokens
                }
                if (continuation.isActive) {
                    if (token != null && state === previous) {
                        continuation.resume(token)
                    } else {
                        continuation.resumeWithException(failure ?: IOException("Session expired or locked. Sign in again."))
                    }
                }
            }
        }
    }

    fun logoutIntent(): Intent? {
        val configuration = state.authorizationServiceConfiguration
        val token = state.idToken
        val intent =
            if (configuration?.endSessionEndpoint != null && token != null)
                service.getEndSessionRequestIntent(
                    EndSessionRequest.Builder(configuration)
                        .setIdTokenHint(token)
                        .setPostLogoutRedirectUri(Uri.parse("classifiedads.kotlin:/logout"))
                        .build()
                )
            else null
        state = AuthState()
        vault.save(null)
        biometric.clear()
        return intent
    }

    fun close() = service.dispose()
}
