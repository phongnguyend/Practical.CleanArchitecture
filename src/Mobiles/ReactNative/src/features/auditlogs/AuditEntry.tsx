import { useState } from "react";
import { Text, View } from "react-native";
import { Button, useStyles } from "../../shared/ui/ui";
import { JsonTools } from "../../shared/ui/record-ui";

export type AuditLog = {
  id: string;
  createdDateTime: string;
  userName?: string;
  action?: string;
  log?: unknown;
};

export default function AuditEntry({ entry }: { entry: AuditLog }) {
  const s = useStyles();
  const [expanded, setExpanded] = useState(false);
  let data = entry.log;
  if (typeof data === "string") {
    try {
      data = JSON.parse(data);
    } catch {
      // Keep non-JSON events readable.
    }
  }
  const display = (value: unknown) => value == null || value === ""
    ? "Not provided"
    : typeof value === "object" ? JSON.stringify(value, null, 2) : String(value);
  const date = new Date(entry.createdDateTime);
  const action = entry.action?.trim().toUpperCase().split(/[_\s-]/)[0];
  const [color, backgroundColor] =
    ["DELETE", "DELETED"].includes(action ?? "") ? ["#b91c1c", "#fee2e2"] :
    ["UPDATE", "UPDATED"].includes(action ?? "") ? ["#9a3412", "#ffedd5"] :
    ["ADD", "ADDED", "CREATE", "CREATED"].includes(action ?? "") ? ["#166534", "#dcfce7"] :
    ["#334155", "#e2e8f0"];
  return (
    <View style={[s.card, { gap: 12, padding: 16 }]}>
      <Text style={[s.text, { color, backgroundColor, fontWeight: "700", alignSelf: "flex-start", borderRadius: 6, paddingHorizontal: 10, paddingVertical: 4 }]}>{entry.action || "Event"}</Text>
      <Text style={s.muted}>{Number.isNaN(date.getTime()) ? entry.createdDateTime : date.toLocaleString()}</Text>
      <Text style={s.text}>By {entry.userName || "Unknown user"}</Text>
      <Button title={expanded ? "Hide event details" : "View event details"} icon="View"
        onPress={() => setExpanded(!expanded)} />
      {expanded && <View style={{ gap: 12 }}>
        {data && typeof data === "object" && !Array.isArray(data)
          ? Object.entries(data).map(([key, value]) => (
            <View key={key} style={{ gap: 6 }}>
              <Text style={s.muted}>{key.replace(/([a-z])([A-Z])/g, "$1 $2")}</Text>
              <Text selectable style={s.text}>{display(value)}</Text>
            </View>
          ))
          : <Text selectable style={s.text}>{display(data)}</Text>}
        <Text style={s.muted}>Full data</Text>
        <JsonTools value={data} />
      </View>}
    </View>
  );
}
