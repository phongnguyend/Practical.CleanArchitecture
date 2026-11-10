import DateTimePicker from "@react-native-community/datetimepicker";
import * as Clipboard from "expo-clipboard";
import * as WebBrowser from "expo-web-browser";
import { useEffect, useState } from "react";
import { Image, Platform, Pressable, Text, View } from "react-native";
import { Row } from "../models/record";
import { Button, HomeButton, JsonTree, useStyles } from "./ui";

export function HomeInfo() {
  const s = useStyles();
  const [now, setNow] = useState(() => new Date());
  const [error, setError] = useState("");
  useEffect(() => {
    const timer = setInterval(() => setNow(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);
  return (
    <View style={s.card}>
      <Text style={s.heading}>Welcome to Classified Ads</Text>
      <Text style={s.text}>React Native · {now.toLocaleString()}</Text>
      <Text style={s.text}>Developed by Phong Nguyen · @phongnguyend</Text>
      <HomeButton
        title="Practical.CleanArchitecture"
        onPress={() => {
          void WebBrowser.openBrowserAsync(
            "https://github.com/phongnguyend/Practical.CleanArchitecture",
          ).catch(() => setError("Unable to open the project link."));
        }}
      />
      {!!error && <Text style={s.error}>{error}</Text>}
    </View>
  );
}

export function ProductImage({ row }: { row: Row }) {
  const s = useStyles();
  const [failed, setFailed] = useState(false);
  return row.imageUrl && !failed ? (
    <Image
      source={{ uri: row.imageUrl }}
      accessibilityLabel={row.name}
      onError={() => setFailed(true)}
      style={{ width: 120, height: 100, borderRadius: 12 }}
      resizeMode="contain"
    />
  ) : (
    <View
      accessibilityLabel="Product image placeholder"
      style={[s.card, { width: 120, alignItems: "center" }]}
    >
      <Text style={s.heading}>CA</Text>
    </View>
  );
}
export function Rating({
  value,
  onPress,
}: {
  value: number;
  onPress: () => void;
}) {
  const s = useStyles();
  return (
    <Pressable
      accessibilityRole="button"
      accessibilityLabel={`Rating ${value} out of 5`}
      onPress={onPress}
    >
      <Text style={s.text}>
        {"★".repeat(Math.max(0, Math.min(5, Math.round(value))))}
        {"☆".repeat(Math.max(0, 5 - Math.min(5, Math.round(value))))} {value}/5
      </Text>
    </Pressable>
  );
}
export function JsonTools({ value }: { value: unknown }) {
  const [expanded, setExpanded] = useState(false),
    [copied, setCopied] = useState(false);
  let parsed = value;
  try {
    if (typeof value === "string") parsed = JSON.parse(value);
  } catch {
    /* Show non-JSON logs as text. */
  }
  return (
    <View style={{ gap: 8 }}>
      <Button
        title={expanded ? "Hide JSON" : "Show JSON"}
        onPress={() => setExpanded(!expanded)}
      />
      <Button
        title={copied ? "Copied" : "Copy JSON"}
        onPress={() => {
          void Clipboard.setStringAsync(
            typeof value === "string" ? value : JSON.stringify(value, null, 2),
          ).then(() => setCopied(true));
        }}
      />
      {expanded && <JsonTree value={parsed} />}
    </View>
  );
}
export function DateField({
  value,
  onChange,
  disabled,
}: {
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
}) {
  const s = useStyles();
  const [mode, setMode] = useState<"date" | "time" | null>(null);
  const parsed = new Date(value);
  const date = Number.isNaN(parsed.getTime()) ? new Date() : parsed;
  return (
    <View style={{ gap: 8 }}>
      <Text style={s.text}>{value ? date.toLocaleString() : "Not set"}</Text>
      <View style={s.actions}>
        <Button
          title="Choose date"
          disabled={disabled}
          onPress={() => setMode("date")}
        />
        <Button
          title="Choose time"
          disabled={disabled}
          onPress={() => setMode("time")}
        />
        <Button
          title="Clear"
          disabled={disabled}
          onPress={() => onChange("")}
        />
      </View>
      {mode && (
        <>
          <DateTimePicker
            value={date}
            mode={mode}
            onChange={(event, selected) => {
              if (Platform.OS === "android") setMode(null);
              if (event.type === "set" && selected)
                onChange(selected.toISOString());
            }}
          />
          {Platform.OS === "ios" && (
            <Button title="Done" onPress={() => setMode(null)} />
          )}
        </>
      )}
    </View>
  );
}
