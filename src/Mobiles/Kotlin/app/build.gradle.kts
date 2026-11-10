import java.security.cert.CertificateFactory

plugins {
    id("com.android.application")
    id("org.jetbrains.kotlin.android")
    id("org.jetbrains.kotlin.plugin.compose")
}
fun setting(name: String, fallback: String) = "\"" + providers.gradleProperty(name).getOrElse(fallback).replace("\\", "\\\\").replace("\"", "\\\"") + "\""
// Opt in to one local public certificate; generated resources are debug-only.
val debugCertificatePath = providers.gradleProperty("debugCertificatePath")
val localCertificateResources = layout.buildDirectory.dir("generated/localCertificate/res")
val generateLocalCertificateResources = tasks.register("generateLocalCertificateResources") {
    inputs.property("certificatePath", debugCertificatePath.orElse(""))
    if (debugCertificatePath.isPresent) inputs.file(debugCertificatePath)
    outputs.dir(localCertificateResources)
    doLast {
        val output = localCertificateResources.get().asFile
        output.resolve("raw/local_development_certificate.cer").delete()
        output.resolve("xml/local_certificate_security_config.xml").delete()
        if (debugCertificatePath.isPresent) {
            val source = file(debugCertificatePath.get())
            val certificate = source.inputStream().use {
                CertificateFactory.getInstance("X.509").generateCertificate(it)
            }
            output.resolve("raw/local_development_certificate.cer").apply {
                parentFile.mkdirs()
                writeBytes(certificate.encoded)
            }
            output.resolve("xml/local_certificate_security_config.xml").apply {
                parentFile.mkdirs()
                writeText("""
                    <network-security-config>
                        <base-config cleartextTrafficPermitted="false"><trust-anchors><certificates src="system" /></trust-anchors></base-config>
                        <domain-config cleartextTrafficPermitted="false">
                            <domain>localhost</domain>
                            <trust-anchors><certificates src="system" /><certificates src="@raw/local_development_certificate" /></trust-anchors>
                        </domain-config>
                    </network-security-config>
                """.trimIndent())
            }
        }
    }
}
android {
    namespace = "com.classifiedads.mobile"
    compileSdk = 36
    defaultConfig {
        applicationId = "com.classifiedads.mobile.kotlin"
        minSdk = 26
        targetSdk = 36
        versionCode = 1
        versionName = "1.0.0"
        manifestPlaceholders["appAuthRedirectScheme"] = "classifiedads.kotlin"
        buildConfigField("String", "API_URL", setting("apiUrl", "https://localhost:44312/api/"))
        buildConfigField("String", "AUTHORITY", setting("authority", "https://localhost:44367"))
        buildConfigField("String", "NOTIFICATION_URL", setting("notificationUrl", "https://localhost:44312/hubs/notification"))
    }
    buildFeatures { compose = true; buildConfig = true }
    buildTypes.getByName("debug") {
        manifestPlaceholders["debugNetworkSecurityConfig"] = if (debugCertificatePath.isPresent)
            "@xml/local_certificate_security_config" else "@xml/debug_network_security_config"
    }
    sourceSets.getByName("debug").res.srcDir(localCertificateResources)
    compileOptions { sourceCompatibility = JavaVersion.VERSION_17; targetCompatibility = JavaVersion.VERSION_17 }
    kotlinOptions { jvmTarget = "17" }
}
tasks.configureEach {
    if (name == "preDebugBuild") dependsOn(generateLocalCertificateResources)
}
dependencies {
    testImplementation("junit:junit:4.13.2")
    testImplementation("org.json:json:20240303")
    implementation(platform("androidx.compose:compose-bom:2025.06.01"))
    implementation("androidx.activity:activity-compose:1.10.1")
    implementation("androidx.compose.material3:material3")
    implementation("androidx.compose.ui:ui-tooling-preview")
    debugImplementation("androidx.compose.ui:ui-tooling")
    implementation("androidx.lifecycle:lifecycle-viewmodel-compose:2.9.1")
    implementation("androidx.lifecycle:lifecycle-runtime-compose:2.9.1")
    implementation("androidx.core:core-ktx:1.16.0")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-android:1.10.2")
    implementation("org.jetbrains.kotlinx:kotlinx-coroutines-rx3:1.10.2")
    implementation("net.openid:appauth:0.11.1")
    implementation("com.squareup.okhttp3:okhttp:4.12.0")
    implementation("com.microsoft.signalr:signalr:10.0.0")
}
