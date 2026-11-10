import { Switch, Text, TextInput, View } from "react-native";
import { DateField } from "./record-ui";
import { useStyles } from "./ui";
export function Input({
  label,
  value,
  onChange,
  disabled,
  secret,
  multiline,
  numeric,
  email,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
  secret?: boolean;
  multiline?: boolean;
  numeric?: boolean;
  email?: boolean;
}) {
  const s = useStyles();
  return (
    <View style={{ gap: 6 }}>
      <Text style={s.text}>{label}</Text>
      <TextInput
        accessibilityLabel={label}
        value={value}
        onChangeText={onChange}
        editable={!disabled}
        secureTextEntry={secret}
        multiline={multiline}
        autoCorrect={false}
        autoCapitalize="none"
        keyboardType={
          numeric ? "number-pad" : email ? "email-address" : "default"
        }
        style={s.input}
      />
    </View>
  );
}
export function Toggle({
  label,
  value,
  onChange,
  disabled,
}: {
  label: string;
  value: boolean;
  onChange: (value: boolean) => void;
  disabled?: boolean;
}) {
  const s = useStyles();
  return (
    <View>
      <Text style={s.text}>{label}</Text>
      <Switch
        accessibilityLabel={label}
        value={value}
        onValueChange={onChange}
        disabled={disabled}
      />
    </View>
  );
}
export function DateInput({
  label,
  value,
  onChange,
  disabled,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  disabled?: boolean;
}) {
  const s = useStyles();
  return (
    <View>
      <Text style={s.text}>{label}</Text>
      <DateField value={value} onChange={onChange} disabled={disabled} />
    </View>
  );
}
export function Detail({
  label,
  value,
  changed = false,
}: {
  label: string;
  value: unknown;
  changed?: boolean;
}) {
  const s = useStyles();
  return (
    <View>
      <Text style={s.muted}>
        {label}
        {changed ? " · Changed" : ""}
      </Text>
      <Text
        selectable
        style={[s.text, changed && { color: "#dc2626", fontWeight: "700" }]}
      >
        {value === null || value === undefined || value === ""
          ? "—"
          : String(value)}
      </Text>
    </View>
  );
}
export function dateText(value: string) {
  const date = new Date(value);
  return value && !Number.isNaN(date.getTime()) ? date.toLocaleString() : value;
}
