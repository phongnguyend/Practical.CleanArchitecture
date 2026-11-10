import React, { createContext, useContext, useState } from "react";
import {
  KeyboardAvoidingView,
  Modal,
  Platform,
  Pressable,
  ScrollView,
  StyleSheet,
  Text,
  View,
  type StyleProp,
  type ViewStyle,
} from "react-native";
import { SafeAreaView } from "react-native-safe-area-context";
export const Theme = createContext(false);
export function useStyles() {
  return makeStyles(useContext(Theme));
}
export function Button({
  title,
  onPress,
  disabled,
  danger,
  variant = "secondary",
  style,
}: {
  title: string;
  onPress: () => void;
  disabled?: boolean;
  danger?: boolean;
  variant?: "primary" | "secondary";
  style?: StyleProp<ViewStyle>;
}) {
  const s = useStyles();
  return (
    <Pressable
      accessibilityRole="button"
      disabled={disabled}
      onPress={onPress}
      accessibilityState={{ disabled: !!disabled }}
      style={({ pressed }) => [
        s.button,
        style,
        variant === "primary" && s.primaryButton,
        danger && s.dangerButton,
        pressed && !disabled && { opacity: 0.75 },
        disabled && { opacity: 0.5 },
      ]}
    >
      <Text style={[
        s.secondaryButtonText,
        variant === "primary" && s.buttonText,
        danger && s.dangerButtonText,
      ]}>{title}</Text>
    </Pressable>
  );
}
export function HomeButton({ title, onPress }: { title: string; onPress: () => void }) {
  return <Button title={title} onPress={onPress} style={{ width: "100%", minHeight: 48, paddingVertical: 12 }} />;
}
export function Sheet({
  title,
  close,
  children,
  footer,
}: {
  title: string;
  close: () => void;
  children: React.ReactNode;
  footer?: React.ReactNode;
}) {
  const s = useStyles();
  return (
    <Modal transparent animationType="slide" onRequestClose={close}>
      <SafeAreaView style={s.overlay}>
        <KeyboardAvoidingView
          behavior={Platform.OS === "ios" ? "padding" : "height"}
          style={{ width: "100%", flexShrink: 1 }}
        >
          <View accessibilityViewIsModal style={s.sheet}>
            <View style={s.header}>
              <Text style={s.heading}>{title}</Text>
              <Button title="Close" onPress={close} />
            </View>
            <ScrollView
              keyboardShouldPersistTaps="handled"
              style={{ flexShrink: 1 }}
              contentContainerStyle={s.body}
            >
              {children}
            </ScrollView>
            <View style={s.footer}>
              {footer || <Button title="Done" onPress={close} />}
            </View>
          </View>
        </KeyboardAvoidingView>
      </SafeAreaView>
    </Modal>
  );
}
export function JsonTree({
  value,
  name = "root",
}: {
  value: any;
  name?: string;
}) {
  const s = useStyles();
  const [open, setOpen] = useState(name === "root");
  const [limit, setLimit] = useState(50);
  if (value === null || typeof value !== "object")
    return (
      <Text selectable style={s.json}>
        {name}: {JSON.stringify(value)}
      </Text>
    );
  const entries = Object.entries(value);
  return (
    <View style={{ paddingLeft: 8 }}>
      <Pressable
        accessibilityRole="button"
        accessibilityState={{ expanded: open }}
        onPress={() => setOpen(!open)}
      >
        <Text style={s.json}>
          {open ? "▼" : "▶"} {name} · {entries.length}{" "}
          {Array.isArray(value) ? "items" : "properties"}
        </Text>
      </Pressable>
      {open &&
        entries
          .slice(0, limit)
          .map(([key, item]) => <JsonTree key={key} name={key} value={item} />)}
      {open && entries.length > limit && (
        <Button title="Show more" onPress={() => setLimit(limit + 50)} />
      )}
    </View>
  );
}
const makeStyles = (dark: boolean) =>
  StyleSheet.create({
    screen: { flex: 1, backgroundColor: dark ? "#0b1220" : "#f3f6fb" },
    header: {
      flexDirection: "row",
      alignItems: "center",
      justifyContent: "space-between",
      padding: 16,
      gap: 8,
    },
    eyebrow: {
      color: "#818cf8",
      fontSize: 11,
      fontWeight: "700",
      letterSpacing: 2,
    },
    heading: {
      color: dark ? "#f1f5f9" : "#172554",
      fontSize: 19,
      fontWeight: "700",
      flexShrink: 1,
    },
    title: {
      fontSize: 30,
      fontWeight: "800",
      color: dark ? "#e2e8f0" : "#172554",
      marginBottom: 12,
    },
    text: { color: dark ? "#e2e8f0" : "#334155", fontSize: 15 },
    muted: { color: dark ? "#94a3b8" : "#64748b", marginVertical: 6 },
    body: { padding: 16, gap: 16, paddingBottom: 32 },
    navigation: { flexGrow: 0, flexShrink: 0 },
    nav: { paddingHorizontal: 16, gap: 8, paddingBottom: 12, alignItems: "center" },
    chip: {
      paddingHorizontal: 16,
      paddingVertical: 10,
      borderRadius: 22,
      backgroundColor: dark ? "#1e293b" : "#e2e8f0",
      minHeight: 44,
      justifyContent: "center",
    },
    selected: { backgroundColor: "#4f46e5" },
    card: {
      padding: 16,
      borderRadius: 18,
      backgroundColor: dark ? "#172033" : "#fff",
      borderWidth: 1,
      borderColor: dark ? "#263449" : "#e2e8f0",
      gap: 6,
    },
    actions: {
      flexDirection: "row",
      flexWrap: "wrap",
      alignItems: "center",
      gap: 8,
    },
    button: {
      backgroundColor: dark ? "#1e293b" : "#fff",
      borderWidth: 1,
      borderColor: dark ? "#475569" : "#cbd5e1",
      borderRadius: 10,
      paddingVertical: 11,
      paddingHorizontal: 14,
      minHeight: 44,
      alignItems: "center",
      justifyContent: "center",
    },
    primaryButton: { backgroundColor: "#4f46e5", borderColor: "#4f46e5" },
    secondaryButtonText: { color: dark ? "#e2e8f0" : "#334155", fontWeight: "600", fontSize: 14 },
    dangerButton: { backgroundColor: dark ? "#451a25" : "#fff1f2", borderColor: dark ? "#881337" : "#fecdd3" },
    dangerButtonText: { color: dark ? "#fda4af" : "#be123c" },
    buttonText: { color: "#fff", fontWeight: "600", fontSize: 14 },
    sectionTitle: { fontSize: 26, fontWeight: "700", color: dark ? "#f1f5f9" : "#0f172a" },
    input: {
      borderWidth: 1,
      borderColor: dark ? "#475569" : "#cbd5e1",
      color: dark ? "#fff" : "#0f172a",
      borderRadius: 10,
      padding: 13,
      marginTop: 6,
      minHeight: 48,
      backgroundColor: dark ? "#0f172a" : "#fff",
      fontSize: 16,
    },
    error: { padding: 12, color: dark ? "#fda4af" : "#b91c1c" },
    overlay: {
      flex: 1,
      backgroundColor: "#0009",
      justifyContent: "center",
      padding: 16,
    },
    sheet: {
      maxHeight: "92%",
      borderRadius: 20,
      backgroundColor: dark ? "#172033" : "#fff",
      overflow: "hidden",
    },
    footer: {
      padding: 16,
      flexDirection: "row",
      flexWrap: "wrap",
      justifyContent: "flex-end",
      gap: 10,
      borderTopWidth: 1,
      borderTopColor: "#64748b44",
    },
    toast: {
      backgroundColor: "#4338ca",
      padding: 16,
      margin: 16,
      borderRadius: 14,
    },
    json: {
      fontFamily: "monospace",
      color: dark ? "#c4b5fd" : "#6d28d9",
      paddingVertical: 6,
    },
  });
