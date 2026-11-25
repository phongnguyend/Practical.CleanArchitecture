import { useState } from "react";
import { Text, View } from "react-native";
import { Button, useStyles } from "../../shared/ui/ui";
import NotificationPlaygrounds from "./NotificationPlaygrounds";

export default function AdminScreen() {
  const s = useStyles();
  const [page, setPage] = useState<"admin" | "playgrounds" | "mobilepush" | "email" | "sms">("admin");
  if (page !== "admin" && page !== "playgrounds") {
    return <NotificationPlaygrounds key={page} channel={page} back={() => setPage("playgrounds")} />;
  }
  return <View style={s.card}>
    <Text style={s.heading}>{page === "admin" ? "Admin" : "Playgrounds"}</Text>
    {page === "admin" ? <Button title="Playgrounds" icon="Admin" onPress={() => setPage("playgrounds")} /> : <>
      <Button title="Back" icon="Back" onPress={() => setPage("admin")} />
      <Button title="Mobile Push" icon="Notifications" onPress={() => setPage("mobilepush")} />
      <Button title="Email" icon="Email" onPress={() => setPage("email")} />
      <Button title="SMS" icon="SMS" onPress={() => setPage("sms")} />
    </>}
  </View>;
}
