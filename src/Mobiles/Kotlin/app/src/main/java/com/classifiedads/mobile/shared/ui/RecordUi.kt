package com.classifiedads.mobile.shared.ui

import android.app.DatePickerDialog
import android.app.TimePickerDialog
import android.graphics.BitmapFactory
import androidx.compose.foundation.Image
import androidx.compose.foundation.layout.*
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.asImageBitmap
import androidx.compose.ui.platform.LocalContext
import androidx.compose.ui.unit.dp
import com.classifiedads.mobile.shared.model.*
import java.net.HttpURLConnection
import java.net.URI
import java.time.Instant
import java.util.Calendar
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.delay
import kotlinx.coroutines.withContext
import org.json.JSONObject

@Composable
fun HomeInfo() {
    val context = LocalContext.current
    var now by remember { mutableStateOf(java.time.LocalDateTime.now()) }
    LaunchedEffect(Unit) {
        while (true) {
            delay(1000)
            now = java.time.LocalDateTime.now()
        }
    }
    OutlinedCard(Modifier.fillMaxWidth(), shape = MaterialTheme.shapes.medium) {
        Column(Modifier.padding(16.dp), verticalArrangement = Arrangement.spacedBy(6.dp)) {
            Text("Welcome to Classified Ads", style = MaterialTheme.typography.titleLarge)
            Text("Kotlin / Compose · $now")
            Text("Developed by Phong Nguyen · @phongnguyend")
            var error by remember { mutableStateOf("") }
            HomeButton(label = "Practical.CleanArchitecture",
                onClick = {
                    runCatching {
                            context.startActivity(
                                android.content.Intent(
                                    android.content.Intent.ACTION_VIEW,
                                    android.net.Uri.parse(
                                        "https://github.com/phongnguyend/Practical.CleanArchitecture"
                                    ),
                                )
                            )
                        }
                        .onFailure { error = "Unable to open the project link." }
                }
            )
            if (error.isNotBlank()) Text(error, color = MaterialTheme.colorScheme.error)
        }
    }
}

@Composable
fun ProductPicture(row: JSONObject) {
    val url = row.optString("imageUrl")
    val bitmap by
        produceState<android.graphics.Bitmap?>(null, url) {
            value =
                withContext(Dispatchers.IO) {
                    runCatching {
                            if (!url.startsWith("https://") && !url.startsWith("http://"))
                                return@runCatching null
                            val connection = URI(url).toURL().openConnection() as HttpURLConnection
                            try {
                                connection.connectTimeout = 10000
                                connection.readTimeout = 10000
                                val bytes =
                                    connection.inputStream.use { input ->
                                        val output = java.io.ByteArrayOutputStream()
                                        val buffer = ByteArray(8192)
                                        while (output.size() < 2 * 1024 * 1024) {
                                            val count =
                                                input.read(
                                                    buffer,
                                                    0,
                                                    minOf(
                                                        buffer.size,
                                                        2 * 1024 * 1024 - output.size(),
                                                    ),
                                                )
                                            if (count < 0) break
                                            output.write(buffer, 0, count)
                                        }
                                        output.toByteArray()
                                    }
                                val bounds =
                                    BitmapFactory.Options().apply { inJustDecodeBounds = true }
                                BitmapFactory.decodeByteArray(bytes, 0, bytes.size, bounds)
                                val options =
                                    BitmapFactory.Options().apply {
                                        inSampleSize =
                                            (maxOf(bounds.outWidth, bounds.outHeight) / 256)
                                                .coerceAtLeast(1)
                                    }
                                BitmapFactory.decodeByteArray(bytes, 0, bytes.size, options)
                            } finally {
                                connection.disconnect()
                            }
                        }
                        .getOrNull()
                }
        }
    if (bitmap != null)
        Image(bitmap!!.asImageBitmap(), row.optString("name"), Modifier.size(120.dp, 100.dp))
    else
        Surface(
            Modifier.size(120.dp, 100.dp),
            color = MaterialTheme.colorScheme.surfaceVariant,
            shape = MaterialTheme.shapes.medium,
        ) {
            Box(Modifier.padding(24.dp)) {
                Text("CA", style = MaterialTheme.typography.headlineLarge)
            }
        }
}

@Composable
fun DateField(label: String, value: String, enabled: Boolean, changed: (String) -> Unit) {
    val context = LocalContext.current
    Column {
        Text(label)
        Text(if (value.isBlank()) "Not set" else localDate(value))
        Row {
            OutlinedButton(shape = MaterialTheme.shapes.small,
                enabled = enabled,
                onClick = {
                    val calendar = Calendar.getInstance()
                    runCatching { calendar.timeInMillis = Instant.parse(value).toEpochMilli() }
                    DatePickerDialog(
                            context,
                            { _, year, month, day ->
                                TimePickerDialog(
                                        context,
                                        { _, hour, minute ->
                                            calendar.set(year, month, day, hour, minute, 0)
                                            calendar.set(Calendar.MILLISECOND, 0)
                                            changed(calendar.toInstant().toString())
                                        },
                                        calendar.get(Calendar.HOUR_OF_DAY),
                                        calendar.get(Calendar.MINUTE),
                                        true,
                                    )
                                    .show()
                            },
                            calendar.get(Calendar.YEAR),
                            calendar.get(Calendar.MONTH),
                            calendar.get(Calendar.DAY_OF_MONTH),
                        )
                        .show()
                },
            ) {
                Text("Choose date and time")
            }
            OutlinedButton(shape = MaterialTheme.shapes.small, enabled = enabled, onClick = { changed("") }) { Text("Clear") }
        }
    }
}
