import JsonViewer from "../JsonViewer/JsonViewer";
import CopyToClipboard from "../CopyToClipboard/CopyToClipboard";
import "./AuditEntries.css";

type Entry = {
  id: string;
  createdDateTime: string;
  userName?: string;
  action?: string;
  log?: unknown;
  data?: Record<string, unknown>;
  highLight?: Record<string, boolean>;
};

function parse(value: unknown): unknown {
  if (typeof value === "string") {
    try {
      return JSON.parse(value);
    } catch {
      return value;
    }
  }
  return value;
}

function display(value: unknown): string {
  if (value === null || value === undefined || value === "") {
    return "Not provided";
  }
  return typeof value === "object" ? JSON.stringify(value, null, 2) : String(value);
}

export default function AuditEntries({ entries, fields }: {
  entries: Entry[];
  fields?: string[];
}) {
  if (!entries.length) {
    return <p className="audit-empty" role="status">No audit logs found.</p>;
  }
  return <div className="audit-entries">
    {entries.map((entry) => {
      const data = entry.data ?? parse(entry.log);
      const values = data && typeof data === "object" && !Array.isArray(data)
        ? data as Record<string, unknown> : null;
      const keys = fields ?? Object.keys(values ?? {});
      const date = new Date(entry.createdDateTime);
      const timestamp = Number.isNaN(date.getTime()) ? entry.createdDateTime : date.toLocaleString();
      return <article className="audit-entry" key={entry.id} aria-label={`${entry.action ?? "Event"} by ${entry.userName || "Unknown user"}`}>
        <header className="audit-entry-heading">
          <strong className="audit-action">{entry.action || "Event"}</strong>
          <time dateTime={entry.createdDateTime}>{timestamp}</time>
        </header>
        <p className="audit-user">By <strong>{entry.userName || "Unknown user"}</strong></p>
        <details className="audit-details" open={fields ? true : undefined}>
          <summary>{fields ? "Record details" : "View event details"}</summary>
          {values ? <dl className="audit-fields">
            {keys.map((key) => <div className={entry.highLight?.[key] ? "audit-field audit-field-changed" : "audit-field"} key={key}>
              <dt>{key.replace(/([a-z])([A-Z])/g, "$1 $2")}{entry.highLight?.[key] && <span className="audit-changed">Changed</span>}</dt>
              <dd>{display(values[key])}</dd>
            </div>)}
          </dl> : <pre className="audit-raw">{display(data)}</pre>}
          <div className="audit-tools" aria-label="Audit data actions">
            <span>Full data</span>
            <JsonViewer jsonData={data} />
            <CopyToClipboard text={typeof entry.log === "string" ? entry.log : JSON.stringify(data ?? null, null, 2)} />
          </div>
        </details>
      </article>;
    })}
  </div>;
}
