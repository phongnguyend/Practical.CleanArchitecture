import { Pagination } from "../../shared/ui/Pagination";
import { useCallback, useEffect, useState } from "react";
import { ActivityIndicator, Text, TextInput, View } from "react-native";
import { api } from "../../core/services";
import { JsonTools } from "../../shared/ui/record-ui";
import { Button, useStyles } from "../../shared/ui/ui";
type AuditLog = {
  id: string;
  createdDateTime: string;
  userName: string;
  action: string;
  log: unknown;
};
export default function AuditLogsScreen() {
  const s = useStyles();
  const [page, setPage] = useState(1),
    [rows, setRows] = useState<AuditLog[]>([]),
    [total, setTotal] = useState(0),
    [search, setSearch] = useState(""),
    [error, setError] = useState(""),
    [busy, setBusy] = useState(true),
    [refresh, setRefresh] = useState(0);
  const load = useCallback(
    () => api(`auditlogentries/paged?page=${page}&pageSize=5`),
    [page],
  );
  useEffect(() => {
    let active = true;
    load()
      .then((result) => {
        if (active) {
          setRows(result.items || []);
          setTotal(result.totalItems || 0);
          setError("");
        }
      })
      .catch((e) => {
        if (active) setError(String(e));
      })
      .finally(() => {
        if (active) setBusy(false);
      });
    return () => {
      active = false;
    };
  }, [load, refresh]);
  return (
    <>
      <Text accessibilityRole="header" style={s.sectionTitle}>Audit logs</Text>
      <TextInput
        accessibilityLabel="Search current records"
        placeholder="Search records"
        value={search}
        onChangeText={setSearch}
        style={s.input}
      />
      <Button
        title="Refresh"
        disabled={busy}
        onPress={() => {
          setBusy(true);
          setRefresh(refresh + 1);
        }}
      />
      {busy && <ActivityIndicator />}
      {rows
        .filter((row) =>
          JSON.stringify(row).toLowerCase().includes(search.toLowerCase()),
        )
        .map((row) => (
          <View key={row.id} style={s.card}>
            <Text style={s.text}>
              {new Date(row.createdDateTime).toLocaleString()} · {row.userName}{" "}
              · {row.action}
            </Text>
            <JsonTools value={row.log} />
          </View>
        ))}
      {!!error && <Text style={s.error}>{error}</Text>}
      <Pagination page={page} total={total} pageSize={5} busy={busy}
        onPage={(value) => { setBusy(true); setPage(value); }} />
    </>
  );
}
