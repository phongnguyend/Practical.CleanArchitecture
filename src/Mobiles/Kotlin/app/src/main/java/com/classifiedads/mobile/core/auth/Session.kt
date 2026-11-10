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
    private val service = AuthorizationService(context)
    private var state =
        vault.read()?.let { runCatching { AuthState.jsonDeserialize(it) }.getOrNull() }
            ?: AuthState()
    private val mutex = Mutex()
    val signedIn
        get() = state.isAuthorized

    suspend fun loginIntent(): Intent {
        val configuration =
            suspendCancellableCoroutine<AuthorizationServiceConfiguration> { continuation ->
                AuthorizationServiceConfiguration.fetchFromIssuer(
                    Uri.parse(BuildConfig.AUTHORITY)
                ) { config, error ->
                    if (continuation.isActive) {
                        if (config != null) continuation.resume(config)
                        else
                            continuation.resumeWithException(
                                error ?: IOException("Discovery failed")
                            )
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
        if (response == null) throw error ?: IOException("Sign-in cancelled")
        state = AuthState(response, error)
        suspendCancellableCoroutine<Unit> { continuation ->
            service.performTokenRequest(response.createTokenExchangeRequest()) { tokens, failure ->
                state.update(tokens, failure)
                vault.save(state.jsonSerializeString())
                if (continuation.isActive) {
                    if (failure != null) continuation.resumeWithException(failure)
                    else continuation.resume(Unit)
                }
            }
        }
    }

    suspend fun token(): String = mutex.withLock {
        check(signedIn) { "Please sign in." }
        suspendCancellableCoroutine { continuation ->
            state.performActionWithFreshTokens(service) { token, _, failure ->
                vault.save(state.jsonSerializeString())
                if (continuation.isActive) {
                    if (token != null) continuation.resume(token)
                    else
                        continuation.resumeWithException(
                            failure ?: IOException("Session expired. Sign in again.")
                        )
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
        return intent
    }

    fun close() = service.dispose()
}
