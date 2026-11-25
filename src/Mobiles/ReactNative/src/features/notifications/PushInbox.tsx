import { useEffect, useState } from "react";
import { Text, View } from "react-native";
import { api } from "../../core/network/api";
import { disablePush, enablePush, InboxMessage, pushConfigured } from "../../core/notifications/push";
import { Button, useStyles } from "../../shared/ui/ui";

export default function PushInbox({ navigate }: { navigate: (section: string) => void }) {
  const s = useStyles();
  const [items, setItems] = useState<InboxMessage[]>([]);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [testing, setTesting] = useState(false);
  async function run(action: () => Promise<unknown>) {
    setBusy(true);
    setError("");
    try {
      await action();
    } catch (e) {
      setError(e instanceof Error ? e.message : "Notification request failed.");
    } finally {
      setBusy(false);
    }
  }
  async function refresh() {
    const data = await api(`notifications/inbox?page=${page}`);
    setItems(data.items);
    setTotal(data.total);
  }
  useEffect(() => {
    let active = true;
    void api(`notifications/inbox?page=${page}`).then((data) => {
      if (active) {
        setItems(data.items);
        setTotal(data.total);
      }
    }).catch(() => {
      if (active) {
        setError("Unable to load notifications. Refresh to try again.");
      }
    });
    return () => {
      active = false;
    };
  }, [page]);
  useEffect(() => { void api("notifications/status").then((status) => setTesting(status.allowTestMessages)).catch(() => undefined); }, []);
  return <View style={s.card}>
    <Text style={s.heading}>Notification inbox</Text>
    {!pushConfigured() && <Text style={s.text}>Push notifications are not configured for this build.</Text>}
    <Button title="Enable notifications" icon="Notifications" disabled={busy || !pushConfigured()} onPress={() => void run(enablePush)} />
    <Button title="Disable notifications" icon="Close" disabled={busy || !pushConfigured()} onPress={() => void run(disablePush)} />
    {testing && <Button title="Send test notification" icon="Notifications" disabled={busy} onPress={() => void run(async () => { await api("notifications/test", "POST"); await refresh(); })} />}
    <Button title="Refresh inbox" icon="Refresh" disabled={busy} onPress={() => void run(refresh)} />
    {!!error && <Text accessibilityRole="alert" style={s.error}>{error}</Text>}
    {!items.length && <Text style={s.text}>No saved notifications yet.</Text>}
    {items.map((item) => <View key={item.id} style={s.card}>
      <Text style={s.heading}>{item.title}</Text><Text style={s.text}>{item.body}</Text>
      <Text style={s.text}>{new Date(item.createdAt).toLocaleString()} · {item.readAt ? "Read" : "Unread"}</Text>
      <Button title="Open notification" icon="View" disabled={busy} onPress={() => void run(async () => { await api(`notifications/inbox/${item.id}/read`, "PUT"); navigate(item.section); })} />
    </View>)}
    <Text style={s.text}>Page {page} · {total} notifications</Text>
    <Button title="Previous" disabled={busy || page <= 1} onPress={() => setPage(page - 1)} />
    <Button title="Next" disabled={busy || page * 20 >= total} onPress={() => setPage(page + 1)} />
  </View>;
}
