package com.classifiedads.mobile.core.network

import android.content.Context
import android.content.Intent
import android.net.Uri
import androidx.core.content.FileProvider
import com.classifiedads.mobile.BuildConfig
import com.classifiedads.mobile.core.auth.Session
import com.classifiedads.mobile.shared.model.apiError
import java.io.File
import java.io.IOException
import java.util.concurrent.TimeUnit
import kotlin.coroutines.resume
import kotlin.coroutines.resumeWithException
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.suspendCancellableCoroutine
import kotlinx.coroutines.withContext
import okhttp3.*
import okhttp3.MediaType.Companion.toMediaType
import okhttp3.RequestBody.Companion.toRequestBody
import org.json.JSONObject

class Api(private val context: Context, private val session: Session) {
    private val http = OkHttpClient.Builder().callTimeout(60, TimeUnit.SECONDS).build()

    private suspend fun request(
        path: String,
        method: String = "GET",
        body: RequestBody? = null,
    ): ByteArray {
        val request =
            Request.Builder()
                .url(BuildConfig.API_URL.trimEnd('/') + "/" + path.trimStart('/'))
                .header("Authorization", "Bearer ${session.token()}")
                .method(method, body)
                .build()
        val response =
            suspendCancellableCoroutine<Response> { continuation ->
                val call = http.newCall(request)
                continuation.invokeOnCancellation { call.cancel() }
                call.enqueue(
                    object : Callback {
                        override fun onFailure(call: Call, error: IOException) {
                            if (continuation.isActive) continuation.resumeWithException(error)
                        }

                        override fun onResponse(call: Call, response: Response) {
                            if (continuation.isActive) continuation.resume(response)
                            else response.close()
                        }
                    }
                )
            }
        return withContext(Dispatchers.IO) {
            response.use {
                val bytes = it.body?.bytes() ?: byteArrayOf()
                check(it.isSuccessful) { apiError(it.code, String(bytes)) }
                bytes
            }
        }
    }

    suspend fun json(path: String, method: String = "GET", body: JSONObject? = null): Any? {
        val bytes =
            request(path, method, body?.toString()?.toRequestBody("application/json".toMediaType()))
        return if (bytes.isEmpty()) null else org.json.JSONTokener(String(bytes)).nextValue()
    }

    suspend fun upload(path: String, uri: Uri, values: JSONObject = JSONObject()): Any? {
        val name = fileName(uri)
        val bytes =
            withContext(Dispatchers.IO) {
                context.contentResolver.openInputStream(uri)?.use { it.readBytes() }
                    ?: error("Cannot open selected file")
            }
        val form =
            MultipartBody.Builder()
                .setType(MultipartBody.FORM)
                .addFormDataPart(
                    "formFile",
                    name,
                    bytes.toRequestBody(
                        (context.contentResolver.getType(uri) ?: "application/octet-stream")
                            .toMediaType()
                    ),
                )
        values.keys().forEach { key ->
            if (!values.isNull(key)) form.addFormDataPart(key, values.get(key).toString())
        }
        val result = request(path, "POST", form.build())
        return if (result.isEmpty()) null else org.json.JSONTokener(String(result)).nextValue()
    }

    fun fileName(uri: Uri): String =
        context.contentResolver
            .query(uri, arrayOf(android.provider.OpenableColumns.DISPLAY_NAME), null, null, null)
            ?.use { if (it.moveToFirst()) it.getString(0) else "upload.bin" } ?: "upload.bin"

    suspend fun download(path: String, name: String): Intent {
        val bytes = request(path)
        val file =
            withContext(Dispatchers.IO) {
                val folder = File(context.cacheDir, "exports").apply { mkdirs() }
                // Remove old exports; retain the new file while the receiving app reads it.
                folder
                    .listFiles()
                    ?.filter { System.currentTimeMillis() - it.lastModified() > 86400000 }
                    ?.forEach { it.delete() }
                File(folder, name.replace(Regex("[^a-zA-Z0-9._-]"), "_")).apply {
                    writeBytes(bytes)
                }
            }
        val uri = FileProvider.getUriForFile(context, "${BuildConfig.APPLICATION_ID}.files", file)
        return Intent.createChooser(
                Intent(Intent.ACTION_SEND)
                    .setType(context.contentResolver.getType(uri) ?: "application/octet-stream")
                    .putExtra(Intent.EXTRA_STREAM, uri)
                    .addFlags(Intent.FLAG_GRANT_READ_URI_PERMISSION),
                "Share download",
            )
            .addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
    }
}
