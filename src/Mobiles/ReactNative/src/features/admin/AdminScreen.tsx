import { useState } from "react";
import { Image, Pressable, Text, View } from "react-native";
import { useStyles } from "../../shared/ui/ui";
import { actionIcons } from "../../shared/ui/action-icons";
import NotificationPlaygrounds from "./NotificationPlaygrounds";

type Page = "admin" | "playgrounds" | "mobilepush" | "email" | "sms";
export default function AdminScreen() {
  const s = useStyles();
  const [page, setPage] = useState<Page>("admin");
  function tab(value: Page, label: string, icon: string, selected = page === value) {
    return <Pressable key={value} accessibilityRole="tab" accessibilityLabel={label} accessibilityState={{ selected }} onPress={() => setPage(value)}
      style={({ pressed }) => ({ minHeight: 48, padding: 10, flexGrow: 1, alignItems: "center", justifyContent: "center", borderBottomWidth: selected ? 3 : 1, borderBottomColor: selected ? "#6366f1" : "#94a3b8", opacity: pressed ? 0.7 : 1 })}>
      <Image source={actionIcons[icon]} accessible={false} style={{ width: 18, height: 18, tintColor: s.secondaryButtonText.color, marginBottom: 4 }} />
      <Text style={[s.secondaryButtonText, { fontWeight: selected ? "700" : "400" }]}>{label}</Text>
    </Pressable>;
  }
  return <View style={{ gap: 16 }}>
    <View style={{ flexDirection: "row" }}>{tab("playgrounds", "Playgrounds", "Admin", page !== "admin")}</View>
    {page !== "admin" && <View style={{ flexDirection: "row", flexWrap: "wrap" }}>
      {tab("mobilepush", "Mobile Push", "Notifications")}
      {tab("email", "Email", "Email")}
      {tab("sms", "SMS", "SMS")}
    </View>}
    {page === "admin" || page === "playgrounds" ? <View style={s.card}>
      <Text style={s.heading}>{page === "admin" ? "Admin" : "Playgrounds"}</Text>
      <Text style={s.secondaryButtonText}>Choose a playground to inspect its queue and compose a message.</Text>
    </View> : <NotificationPlaygrounds key={page} channel={page} />}
  </View>;
}
