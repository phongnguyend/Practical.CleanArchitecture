import 'package:flutter/material.dart';

ButtonStyle dangerButtonStyle(BuildContext context) {
  final dark = Theme.of(context).brightness == Brightness.dark;
  return TextButton.styleFrom(
    foregroundColor: Color(dark ? 0xfffda4af : 0xffbe123c),
    backgroundColor: Color(dark ? 0xff451a25 : 0xfffff1f2),
    side: BorderSide(color: Color(dark ? 0xff881337 : 0xfffecdd3)),
    minimumSize: const Size(44, 44),
    padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 11),
    shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
    textStyle: const TextStyle(fontSize: 14, fontWeight: FontWeight.w600),
  );
}

// Keep these visual tokens aligned with Kotlin AppTheme and React Native ui.tsx.
ThemeData appTheme(Brightness brightness) {
  final dark = brightness == Brightness.dark;
  final background = Color(dark ? 0xff0b1220 : 0xfff3f6fb);
  final surface = Color(dark ? 0xff172033 : 0xffffffff);
  final text = Color(dark ? 0xffe2e8f0 : 0xff334155);
  final border = Color(dark ? 0xff475569 : 0xffcbd5e1);
  final scheme =
      ColorScheme.fromSeed(
        seedColor: const Color(0xff4f46e5),
        brightness: brightness,
      ).copyWith(
        primary: const Color(0xff4f46e5),
        onPrimary: Colors.white,
        surface: surface,
        onSurface: text,
        outline: border,
        surfaceTint: Colors.transparent,
        primaryContainer: const Color(0xff4f46e5),
        onPrimaryContainer: Colors.white,
      );
  final base = ThemeData(useMaterial3: true, colorScheme: scheme);
  final shape = RoundedRectangleBorder(borderRadius: BorderRadius.circular(10));
  final button = ButtonStyle(
    minimumSize: const WidgetStatePropertyAll(Size(44, 44)),
    padding: const WidgetStatePropertyAll(
      EdgeInsets.symmetric(horizontal: 14, vertical: 11),
    ),
    shape: WidgetStatePropertyAll(shape),
    textStyle: const WidgetStatePropertyAll(
      TextStyle(fontSize: 14, fontWeight: FontWeight.w600),
    ),
  );
  return base.copyWith(
    scaffoldBackgroundColor: background,
    textTheme: base.textTheme.copyWith(
      bodyLarge: TextStyle(fontSize: 16, color: text),
      bodyMedium: TextStyle(fontSize: 15, color: text),
      titleLarge: TextStyle(
        fontSize: 19,
        fontWeight: FontWeight.w700,
        color: text,
      ),
      headlineSmall: TextStyle(
        fontSize: 26,
        fontWeight: FontWeight.w700,
        color: text,
      ),
    ),
    appBarTheme: AppBarThemeData(
      backgroundColor: background,
      surfaceTintColor: Colors.transparent,
      titleTextStyle: TextStyle(
        fontSize: 19,
        fontWeight: FontWeight.w700,
        color: text,
      ),
    ),
    cardTheme: CardThemeData(
      color: surface,
      elevation: 0,
      margin: const EdgeInsets.only(bottom: 12),
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(18),
        side: BorderSide(color: border),
      ),
    ),
    inputDecorationTheme: InputDecorationThemeData(
      filled: true,
      fillColor: dark ? const Color(0xff0f172a) : Colors.white,
      contentPadding: const EdgeInsets.all(13),
      border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
      enabledBorder: OutlineInputBorder(
        borderRadius: BorderRadius.circular(10),
        borderSide: BorderSide(color: border),
      ),
    ),
    filledButtonTheme: FilledButtonThemeData(style: button),
    textButtonTheme: TextButtonThemeData(
      style: button.copyWith(
        foregroundColor: WidgetStatePropertyAll(text),
        side: WidgetStatePropertyAll(BorderSide(color: border)),
        backgroundColor: WidgetStatePropertyAll(surface),
      ),
    ),
    outlinedButtonTheme: OutlinedButtonThemeData(style: button),
    chipTheme: ChipThemeData(
      backgroundColor: dark ? const Color(0xff1e293b) : const Color(0xffe2e8f0),
      selectedColor: scheme.primary,
      showCheckmark: false,
      labelStyle: TextStyle(fontSize: 14, color: text),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(22)),
      side: BorderSide.none,
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 10),
    ),
    dialogTheme: DialogThemeData(
      backgroundColor: surface,
      surfaceTintColor: Colors.transparent,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
    ),
  );
}
