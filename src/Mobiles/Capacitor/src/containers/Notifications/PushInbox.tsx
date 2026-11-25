import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { disablePush, enablePush, InboxMessage, pushApi, pushConfigured } from "../../core/push";
import ActionIcon from "../../components/ActionIcon/ActionIcon";

export default function PushInbox() {
  const navigate = useNavigate();
  const [items, setItems] = useState<InboxMessage[]>([]);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [error, setError] = useState("");
  const [busy, setBusy] = useState(false);
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
    const result = await pushApi(`inbox?page=${page}`);
    setItems(result.items);
    setTotal(result.total);
  }
  useEffect(() => {
    void run(refresh);
  }, [page]);
  useEffect(() => {
    void pushApi("status").then((status) => setTesting(status.allowTestMessages)).catch(() => undefined);
  }, []);
  return <section className="mb-4">
    <h2>Notification inbox</h2>
    {!pushConfigured() && <p>Push notifications are not configured for this build.</p>}
    <div className="d-grid gap-2 mb-3">
      <button className="btn btn-primary" disabled={busy || !pushConfigured()} onClick={() => void run(enablePush)}><ActionIcon action="notification" />Enable notifications</button>
      <button className="btn btn-secondary" disabled={busy || !pushConfigured()} onClick={() => void run(disablePush)}><ActionIcon action="close" />Disable notifications</button>
      {testing && <button className="btn btn-secondary" disabled={busy} onClick={() => void run(async () => {
        await pushApi("test", "POST");
        await refresh();
      })}><ActionIcon action="notification" />Send test notification</button>}
      <button className="btn btn-secondary" disabled={busy} onClick={() => void run(refresh)}><ActionIcon action="refresh" />Refresh inbox</button>
    </div>
    {!!error && <p role="alert">{error}</p>}
    {!items.length && <p>No saved notifications yet.</p>}
    {items.map((item) => <article className="card p-3 mb-3" key={item.id}>
      <h3>{item.title}</h3><p>{item.body}</p>
      <p>{new Date(item.createdAt).toLocaleString()} / {item.readAt ? "Read" : "Unread"}</p>
      <button className="btn btn-secondary" disabled={busy} onClick={() => void run(async () => {
        await pushApi(`inbox/${item.id}/read`, "PUT");
        navigate(`/${item.section === "auditlogentries" ? "auditlogs" : item.section}`);
      })}><ActionIcon action="view" />Open notification</button>
    </article>)}
    <p>Page {page} / {total} notifications</p>
    <div className="d-flex gap-2">
      <button className="btn btn-secondary" disabled={busy || page <= 1} onClick={() => setPage(page - 1)}><ActionIcon action="previous" />Previous</button>
      <button className="btn btn-secondary" disabled={busy || page * 20 >= total} onClick={() => setPage(page + 1)}><ActionIcon action="next" />Next</button>
    </div>
  </section>;
}
