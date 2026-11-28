import { useEffect, useState } from "react";
import { Text, View } from "react-native";
import { api } from "../../core/services";
import { Button, Sheet, useStyles } from "../../shared/ui/ui";
import { Input } from "../../shared/ui/fields";
type Item = { id: string; name?: string; userName?: string; email?: string };
export function Membership({ roleId, userId }: { roleId?: string; userId?: string }) {
  const s = useStyles();
  const [items, setItems] = useState<Item[]>([]);
  const [users, setUsers] = useState<Item[]>([]);
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<Item | null>(null);
  const [removing, setRemoving] = useState<Item | null>(null);
  const [busy, setBusy] = useState(true);
  const [error, setError] = useState("");
  const [revision, setRevision] = useState(0);
  useEffect(() => {
    let active = true;
    Promise.all([api(roleId ? `roles/${roleId}/users` : `users/${userId}/roles`), roleId ? api("users") : Promise.resolve([])])
      .then(([members, available]) => {
        if (active) {
          setItems(members);
          setUsers(available);
        }
      }).catch(() => {
        if (active) {
          setError("Unable to load role membership. Please try again.");
        }
      }).finally(() => {
        if (active) {
          setBusy(false);
        }
      });
    return () => {
      active = false;
    };
  }, [roleId, userId, revision]);
  async function change(user: Item, assigned: boolean) {
    if (busy) {
      return;
    }
    setBusy(true);
    setError("");
    try {
      await api(`roles/${roleId}/users/${user.id}`, assigned ? "PUT" : "DELETE");
      setItems(await api(`roles/${roleId}/users`));
      setSelected(null);
      setSearch("");
      setRemoving(null);
    } catch {
      setRemoving(null);
      setError("Unable to update role membership. Refresh and try again.");
    } finally {
      setBusy(false);
    }
  }
  const candidates = users.filter(user => !items.some(item => item.id === user.id)
    && `${user.userName} ${user.email || ""}`.toLowerCase().includes(search.toLowerCase()));
  return <View style={{ gap: 12, marginTop: 16, paddingTop: 16, borderTopWidth: 1, borderColor: s.card.borderColor }}>
    <View style={{ flexDirection: "row", alignItems: "center", justifyContent: "space-between", gap: 12 }}>
    <Text style={[s.text, { fontWeight: "700", flex: 1 }]} accessibilityRole="header">{roleId ? "Role users" : "Assigned roles"}</Text>
    <Button title={roleId ? "Refresh users" : "Refresh roles"} icon="Refresh" iconOnly disabled={busy} onPress={() => {
      setBusy(true);
      setError("");
      setRevision(value => value + 1);
    }} />
    {busy && <Text style={s.text}>Loading...</Text>}
    </View>
    {!!error && <Text style={s.text} accessibilityRole="alert">{error}</Text>}
    {roleId && <>
      <Text style={s.text}>Search by username or email to assign someone to this role.</Text>
      <Input label="Search users" value={search} onChange={value => {
        setSearch(value);
        setSelected(null);
      }} />
      {search.trim().length > 0 && candidates.length === 0 && <Text style={s.text}>No available users match your search.</Text>}
      <Text style={s.text}>User to assign: {selected ? selected.userName : "Select a search result"}</Text>
      {search.trim().length > 0 && !selected && candidates.slice(0, 5).map(user => <Button key={user.id} title={`Select ${user.userName}`} icon="Users" disabled={busy} onPress={() => setSelected(user)} />)}
      {search.trim().length > 0 && !selected && candidates.length > 5 && <Text style={s.text}>Keep typing to narrow the results.</Text>}
      <Button title="Assign user" icon="Add new" variant="primary" disabled={busy || !selected} onPress={() => {
        if (selected) {
          void change(selected, true);
        }
      }} />
    </>}
    {!busy && !error && items.length === 0 && <Text style={s.text}>{roleId ? "No users assigned." : "No roles assigned."}</Text>}
    {items.map(item => <View key={item.id} style={{ gap: 12, paddingTop: 12, borderTopWidth: 1, borderColor: s.card.borderColor }}>
      <Text style={s.text}>{item.userName || item.name}</Text>
      {!!item.email && <Text style={s.text}>{item.email}</Text>}
      {roleId && <Button title="Remove" accessibilityLabel={`Remove ${item.userName} from role`} icon="Delete" danger disabled={busy} onPress={() => setRemoving(item)} />}
    </View>)}
    {removing && <Sheet title="Remove assigned role" close={() => {
      if (!busy) {
        setRemoving(null);
      }
    }} footer={<>
      <Button title="Cancel" disabled={busy} onPress={() => setRemoving(null)} />
      <Button title="Remove" icon="Delete" danger disabled={busy} onPress={() => void change(removing, false)} />
    </>}><Text style={s.text}>Remove {removing.userName} from this role?</Text></Sheet>}
  </View>;
}
