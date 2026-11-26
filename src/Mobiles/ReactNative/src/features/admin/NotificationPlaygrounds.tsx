import { useEffect, useState } from "react";
import { Text, TextInput, View } from "react-native";
import { api } from "../../core/network/api";
import { Button, Sheet, useStyles } from "../../shared/ui/ui";

type Queue = { deviceCount?: number; provider: string; items: { pending?: number; completed?: number; failed?: number; id: string; title: string; body: string; recipient: string; status: string; attempts: number }[] };
export default function NotificationPlaygrounds({ channel }: { channel: "mobilepush" | "email" | "sms" }) {
  const s = useStyles();
  const push = channel === "mobilepush";
  const [section, setSection] = useState("notifications");
  const [choosing, setChoosing] = useState(false);
  const email = channel === "email";
  const path = push ? "notifications/playground" : `notifications/playgrounds/${channel}`;
  const [from, setFrom] = useState("");
  const [recipient, setRecipient] = useState("");
  const [subject, setSubject] = useState("");
  const [body, setBody] = useState("");
  const [info, setInfo] = useState<Queue>();
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const [busy, setBusy] = useState(false);
  async function refresh() {
    setInfo(await api(path));
  }
  useEffect(() => {
    let active = true;
    void api(path).then(data => {
      if (active) {
        setInfo(data);
      }
    }).catch(() => {
      if (active) {
        setError("Unable to load the queue. Please try again.");
      }
    });
    return () => {
      active = false;
    };
  }, [path]);
  async function run(action: () => Promise<void>) {
    setBusy(true);
    setError("");
    try {
      await action();
    } catch {
      setError("Unable to access the queue. Check the fields and try again.");
    } finally {
      setBusy(false);
    }
  }
  function queue() {
    setMessage("");
    if ((!push && !recipient.trim()) || !body.trim() || ((email || push) && !subject.trim()) || (email && !from.trim())) {
      setError(push ? "Title and message are required." : "Complete all required fields.");
      return;
    }
    const pattern = /^[^\s@;]+@[^\s@;]+\.[^\s@;]+$/;
    if (!push && (email ? !pattern.test(from.trim()) || !pattern.test(recipient.trim()) : !/^\+[1-9][0-9]{7,14}$/.test(recipient.trim()))) {
      setError(email ? "Enter valid From and To email addresses." : "Enter a phone number in international format, for example +15551234567.");
      return;
    }
    void run(async () => {
      await api(path, "POST", push ? { title: subject.trim(), body: body.trim(), section } : { from: from.trim(), recipient: recipient.trim(), subject: subject.trim(), body: body.trim() });
      setMessage("Message queued successfully.");
      await refresh();
    });
  }
  return <View style={s.card}>
    <Text style={s.heading}>{push ? "Mobile Push" : email ? "Email" : "SMS"}</Text>
    <Text style={s.text}>{push ? "Queue a notification for your own registered devices." : `Queue a message for delivery by the existing ${email ? "Email" : "SMS"} worker.`}</Text>
    {info && <Text style={s.text}>Provider: {info.provider}. {push && `Registered devices: ${info.deviceCount}. `} {info.provider === "Fake" ? "Delivery is simulated; no message is sent." : "Queued messages may be delivered to the specified recipient."}</Text>}
    {email && <><Text style={s.text}>From</Text><TextInput accessibilityLabel="From" style={s.input} value={from} onChangeText={setFrom} keyboardType="email-address" autoCapitalize="none" maxLength={254} editable={!busy} /></>}
    {!push && <><Text style={s.text}>{email ? "To" : "Phone number"}</Text>
    <TextInput accessibilityLabel={email ? "To" : "Phone number"} style={s.input} value={recipient} onChangeText={setRecipient} keyboardType={email ? "email-address" : "phone-pad"} autoCapitalize="none" maxLength={email ? 254 : 16} editable={!busy} />
    {!email && <Text style={s.text}>International format, for example +15551234567.</Text>}</>}
    {(email || push) && <><Text style={s.text}>{push ? "Title" : "Subject"}</Text><TextInput accessibilityLabel={push ? "Title" : "Subject"} style={s.input} value={subject} onChangeText={setSubject} maxLength={push ? 100 : 200} editable={!busy} /></>}
    <Text style={s.text}>Message</Text><TextInput accessibilityLabel="Message" style={[s.input, { minHeight: 120, textAlignVertical: "top" }]} multiline value={body} onChangeText={setBody} maxLength={push ? 500 : email ? 10000 : 1600} editable={!busy} />
    <Text style={s.text}>{body.length}/{push ? 500 : email ? 10000 : 1600} characters</Text>
    {push && <><Text style={s.text}>Destination</Text><Button title={`Destination: ${section}`} icon="View" disabled={busy} onPress={() => setChoosing(true)} />
      {choosing && <Sheet title="Destination" close={() => setChoosing(false)}>{["notifications", "home", "products", "files", "users", "roles", "configurationentries", "auditlogentries"].map(value => <Button key={value} title={value} icon="View" onPress={() => {
        setSection(value);
        setChoosing(false);
      }} />)}</Sheet>}</>}
    <Button title="Queue message" icon="Notifications" disabled={busy || !info} onPress={queue} />
    <Button title="Refresh queue" icon="Refresh" disabled={busy} onPress={() => void run(refresh)} />
    {!!error && <Text accessibilityRole="alert" style={s.error}>{error}</Text>}
    {!!message && <Text accessibilityRole="alert" style={s.text}>{message}</Text>}
    <Text style={s.heading}>Recent messages</Text>
    {info?.items.length === 0 && <Text style={s.text}>No queued messages yet.</Text>}
    {info?.items.map(item => <View key={item.id} style={s.card}><Text style={s.heading}>{item.title}</Text>{!push && <Text style={s.text}>To: {item.recipient}</Text>}<Text style={s.text}>{item.body}</Text><Text style={s.text}>{push ? `Pending: ${item.pending} / Completed: ${item.completed} / Failed: ${item.failed}` : `${item.status} / Attempts: ${item.attempts}`}</Text></View>)}
  </View>;
}
