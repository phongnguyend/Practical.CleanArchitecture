import java.security.cert.CertificateFactory
import java.util.Base64

plugins {
    id("com.android.application")
    // The Flutter Gradle Plugin must be applied after the Android and Kotlin Gradle plugins.
    id("dev.flutter.flutter-gradle-plugin")
}

// Opt in to one local public certificate; generated resources are debug-only.
val debugCertificatePath = providers.gradleProperty("debugCertificatePath")
val debugCertificateBase64 = providers.gradleProperty("dart-defines").map { definitions ->
    definitions.split(',').map { String(Base64.getDecoder().decode(it)) }
        .firstOrNull { it.startsWith("DEBUG_CERTIFICATE_BASE64=") }
        ?.substringAfter('=') ?: ""
}.orElse("")
val hasDebugCertificate = debugCertificatePath.isPresent || debugCertificateBase64.get().isNotEmpty()
val localCertificateResources = layout.buildDirectory.dir("generated/localCertificate/res")
val generateLocalCertificateResources = tasks.register("generateLocalCertificateResources") {
    inputs.property("certificatePath", debugCertificatePath.orElse(""))
    inputs.property("certificateBase64", debugCertificateBase64)
    if (debugCertificatePath.isPresent) inputs.file(debugCertificatePath)
    outputs.dir(localCertificateResources)
    doLast {
        val output = localCertificateResources.get().asFile
        output.resolve("raw/local_development_certificate.cer").delete()
        output.resolve("xml/local_certificate_security_config.xml").delete()
        if (hasDebugCertificate) {
            val source = if (debugCertificatePath.isPresent) file(debugCertificatePath.get()).inputStream()
                else Base64.getDecoder().decode(debugCertificateBase64.get()).inputStream()
            val certificate = source.use {
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
    buildTypes.getByName("debug") {
        manifestPlaceholders["debugNetworkSecurityConfig"] = if (hasDebugCertificate)
            "@xml/local_certificate_security_config" else "@xml/debug_network_security_config"
    }
    sourceSets.getByName("debug").res.srcDir(localCertificateResources.get().asFile)

    namespace = "com.classifiedads.mobile.classified_ads_mobile"
    compileSdk = flutter.compileSdkVersion
    ndkVersion = flutter.ndkVersion

    compileOptions {
        sourceCompatibility = JavaVersion.VERSION_17
        targetCompatibility = JavaVersion.VERSION_17
    }

    defaultConfig {
        // TODO: Specify your own unique Application ID (https://developer.android.com/studio/build/application-id.html).
        applicationId = "com.classifiedads.mobile.classified_ads_mobile"
        // You can update the following values to match your application needs.
        // For more information, see: https://flutter.dev/to/review-gradle-config.
        minSdk = 24
        manifestPlaceholders["appAuthRedirectScheme"] = "classifiedads.flutter"
        targetSdk = flutter.targetSdkVersion
        // Uses the version code from pubspec.yaml. When using split APKs, 1000 * ABI_VERSION
        // is added automatically by Flutter. (https://developer.android.com/studio/build/configure-apk-splits#configure-APK-versions)
        // You can force using the value of versionCode by specifying the `-P force-version-code-ignoring-abi=true`
        // flag during build.
        versionCode = flutter.versionCode
        versionName = flutter.versionName
    }

    buildTypes {
        release {
            // TODO: Add your own signing config for the release build.
            // Signing with the debug keys for now, so `flutter run --release` works.
            signingConfig = signingConfigs.getByName("debug")
        }
    }
}

kotlin {
    compilerOptions {
        jvmTarget = org.jetbrains.kotlin.gradle.dsl.JvmTarget.JVM_17
    }
}

flutter {
    source = "../.."
}

tasks.configureEach {
    if (name == "preDebugBuild") dependsOn(generateLocalCertificateResources)
}
