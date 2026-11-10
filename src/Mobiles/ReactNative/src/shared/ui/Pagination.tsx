import { Text, View } from "react-native";
import { Button, useStyles } from "./ui";

export function Pagination({ page, total, pageSize, busy = false, onPage }: {
  page: number; total: number; pageSize: number; busy?: boolean; onPage: (page: number) => void;
}) {
  const s = useStyles();
  return <View style={{ gap: 8, paddingVertical: 12 }}>
    <Text style={[s.text, { textAlign: "center" }]}>Page {page} | {total} records</Text>
    <View style={{ flexDirection: "row", gap: 12 }}>
      <Button title="Previous" style={{ flex: 1, minHeight: 48 }} disabled={busy || page <= 1} onPress={() => onPage(page - 1)} />
      <Button title="Next" style={{ flex: 1, minHeight: 48 }} disabled={busy || page * pageSize >= total} onPress={() => onPage(page + 1)} />
    </View>
  </View>;
}
