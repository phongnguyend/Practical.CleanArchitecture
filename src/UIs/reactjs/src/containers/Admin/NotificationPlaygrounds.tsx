import { useEffect, useState } from "react";
import axios from "axios";
import env from "../../environments";
import addAuthInterceptors from "../Auth/authInterceptors";
import ActionIcon from "../../components/ActionIcon/ActionIcon";

const client = axios.create({ baseURL: env.ResourceServer.Endpoint + "notifications/" });
addAuthInterceptors(client);
type Item = { pending?: number; completed?: number; failed?: number; id: string; title: string; body: string; recipient: string; createdAt: string; status: string; attempts: number };
type Queue = { deviceCount?: number; provider: string; items: Item[] };

export default function NotificationPlaygrounds({ channel }: { channel: "mobilepush" | "email" | "sms" }) {
  const push = channel === "mobilepush";
  const path = push ? "playground" : `playgrounds/${channel}`;
  const [section, setSection] = useState("notifications");
  const email = channel === "email";
  const name = push ? "Mobile Push" : email ? "Email" : "SMS";
  const [from, setFrom] = useState("");
  const [recipient, setRecipient] = useState("");
  const [subject, setSubject] = useState("");
  const [body, setBody] = useState("");
  const [info, setInfo] = useState<Queue>();
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  async function refresh() {
    try {
      setInfo((await client.get<Queue>(path)).data);
      setError("");
    } catch {
      setError("Unable to load the queue. Please try again.");
    }
  }
  useEffect(() => {
    let active = true;
    void client.get<Queue>(path).then(({ data }) => {
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
  async function queue(event: React.FormEvent) {
    event.preventDefault();
    if (busy) {
      return;
    }
    setSuccess("");
    if ((!push && !recipient.trim()) || !body.trim() || ((email || push) && !subject.trim()) || (email && !from.trim())) {
      setError(push ? "Title and message are required." : "Complete all required fields.");
      return;
    }
    if (!push && (email ? !/^[^\s@;]+@[^\s@;]+\.[^\s@;]+$/.test(recipient.trim()) || !/^[^\s@;]+@[^\s@;]+\.[^\s@;]+$/.test(from.trim()) : !/^\+[1-9][0-9]{7,14}$/.test(recipient.trim()))) {
      setError(email ? "Enter valid From and To email addresses." : "Enter a phone number in international format, for example +15551234567.");
      return;
    }
    setBusy(true);
    setError("");
    try {
      const result = await client.post<{ id: string }>(path, push ? { title: subject.trim(), body: body.trim(), section } : { from: from.trim(), recipient: recipient.trim(), subject: subject.trim(), body: body.trim() });
      setSuccess(`Message queued. ID: ${result.data.id}`);
      await refresh();
    } catch {
      setError("Unable to queue the message. Check the fields and try again.");
    } finally {
      setBusy(false);
    }
  }
  return <div>
    <h1>{name}</h1><p>{push ? "Queue a notification for your own registered devices." : `Queue a message for delivery by the existing ${name} worker.`}</p>
    {info && <div role="status" className="alert alert-info">Provider: <strong>{info.provider}</strong>. {push && <>Registered devices: {info.deviceCount}. </>} {info.provider === "Fake" ? "Delivery is simulated; no message is sent." : "Queued messages may be delivered to the specified recipient."}</div>}
    {error && <p role="alert" className="alert alert-danger">{error}</p>}
    {success && <p role="status" className="alert alert-success">{success}</p>}
    <form noValidate onSubmit={queue} className="d-grid gap-3" style={{ maxWidth: 720 }}>
      {email && <div><label htmlFor="message-from" className="form-label">From</label><input id="message-from" type="email" className="form-control" maxLength={254} value={from} onChange={event => setFrom(event.target.value)} disabled={busy} required /></div>}
      {!push && <div><label htmlFor="message-recipient" className="form-label">{email ? "To" : "Phone number"}</label><input id="message-recipient" type={email ? "email" : "tel"} className="form-control" maxLength={email ? 254 : 16} value={recipient} onChange={event => setRecipient(event.target.value)} disabled={busy} required />{!email && <small>International format, for example +15551234567.</small>}</div>}
      {(email || push) && <div><label htmlFor="message-subject" className="form-label">{push ? "Title" : "Subject"}</label><input id="message-subject" className="form-control" maxLength={push ? 100 : 200} value={subject} onChange={event => setSubject(event.target.value)} disabled={busy} required /></div>}
      <div><label htmlFor="message-body" className="form-label">Message</label><textarea id="message-body" className="form-control" rows={5} maxLength={push ? 500 : email ? 10000 : 1600} value={body} onChange={event => setBody(event.target.value)} disabled={busy} required /><small>{body.length}/{push ? 500 : email ? 10000 : 1600} characters</small></div>
      {push && <div><label htmlFor="message-destination" className="form-label">Destination</label><select id="message-destination" className="form-select" value={section} onChange={event => setSection(event.target.value)} disabled={busy}>{["notifications", "home", "products", "files", "users", "roles", "configurationentries", "auditlogentries"].map(value => <option key={value} value={value}>{value}</option>)}</select></div>}
      <div className="d-flex flex-wrap gap-2"><button type="submit" className="btn btn-primary" disabled={busy || !info}><ActionIcon action="notification" />{busy ? "Queueing…" : "Queue message"}</button><button type="button" className="btn btn-outline-secondary" disabled={busy} onClick={() => void refresh()}><ActionIcon action="refresh" />Refresh queue</button></div>
    </form>
    <h2 className="mt-4">Recent messages</h2>
    {info?.items.length === 0 && <p>No queued messages yet.</p>}
    <div className="d-grid gap-3">{info?.items.map(item => <article key={item.id} className="border rounded p-3">
      <h3 className="h5">{item.title}</h3>{!push && <p>To: {item.recipient}</p>}<p style={{ whiteSpace: "pre-wrap", overflowWrap: "anywhere" }}>{item.body}</p>
      <p className="mb-0">{push ? `Pending: ${item.pending} / Completed: ${item.completed} / Failed: ${item.failed}` : `${item.status} / Attempts: ${item.attempts}`} / {new Date(item.createdAt).toLocaleString()}</p>
    </article>)}</div>
  </div>;
}
