package com.classifiedads.mobile.shared.ui

import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.BorderStroke
import androidx.compose.material3.*
import androidx.compose.runtime.Composable
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.TextStyle
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp

@Composable
fun dangerButtonColors(): ButtonColors {
    val dark = MaterialTheme.colorScheme.background == Color(0xff0b1220)
    return ButtonDefaults.outlinedButtonColors(
        containerColor = Color(if (dark) 0xff451a25 else 0xfffff1f2),
        contentColor = Color(if (dark) 0xfffda4af else 0xffbe123c),
    )
}

@Composable
fun dangerButtonBorder() = BorderStroke(1.dp,
    Color(if (MaterialTheme.colorScheme.background == Color(0xff0b1220)) 0xff881337 else 0xfffecdd3))

// Shared visual tokens: Flutter app_theme.dart and React Native ui.tsx.
@Composable
fun AppTheme(dark: Boolean, content: @Composable () -> Unit) {
    val background = Color(if (dark) 0xff0b1220 else 0xfff3f6fb)
    val surface = Color(if (dark) 0xff172033 else 0xffffffff)
    val text = Color(if (dark) 0xffe2e8f0 else 0xff334155)
    val scheme = (if (dark) darkColorScheme() else lightColorScheme()).copy(
        primary = Color(0xff4f46e5), onPrimary = Color.White,
        primaryContainer = Color(0xff4f46e5), onPrimaryContainer = Color.White,
        secondaryContainer = Color(if (dark) 0xff1e293b else 0xffe2e8f0),
        onSecondaryContainer = text,
        background = background, onBackground = text,
        surface = surface, onSurface = text, surfaceTint = Color.Transparent,
        surfaceContainer = surface, surfaceContainerLow = surface,
        surfaceContainerHigh = surface, surfaceContainerHighest = surface,
        outline = Color(if (dark) 0xff475569 else 0xffcbd5e1),
        outlineVariant = Color(if (dark) 0xff263449 else 0xffe2e8f0),
    )
    MaterialTheme(
        colorScheme = scheme,
        shapes = Shapes(small = RoundedCornerShape(10.dp), medium = RoundedCornerShape(18.dp), large = RoundedCornerShape(20.dp)),
        typography = Typography(
            bodyLarge = TextStyle(fontSize = 16.sp, lineHeight = 24.sp),
            bodyMedium = TextStyle(fontSize = 15.sp, lineHeight = 22.sp),
            titleLarge = TextStyle(fontSize = 19.sp, fontWeight = FontWeight.Bold),
            headlineSmall = TextStyle(fontSize = 26.sp, fontWeight = FontWeight.Bold),
            labelLarge = TextStyle(fontSize = 14.sp, fontWeight = FontWeight.SemiBold),
        ),
        content = content,
    )
}
