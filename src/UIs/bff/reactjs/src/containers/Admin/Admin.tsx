import { Link } from "react-router-dom";
import NotificationPlaygrounds from "./NotificationPlaygrounds";
import ActionIcon from "../../components/ActionIcon/ActionIcon";

export default function Admin({ view = "admin" }: { view?: "admin" | "playgrounds" | "mobilepush" | "email" | "sms" }) {
  const channel = view === "mobilepush" || view === "email" || view === "sms";
  return <section className="card"><div className="card-body">
    <nav aria-label="Admin sections" className="nav nav-tabs mb-3">
      <Link to="/admin/playgrounds" className={`nav-link${view !== "admin" ? " active" : ""}`} aria-current={view !== "admin" ? "page" : undefined}><ActionIcon action="settings" />Playgrounds</Link>
    </nav>
    {view !== "admin" && <nav aria-label="Playground channels" className="nav nav-tabs mb-3 flex-wrap">
      {([['mobilepush', 'mobile-push', 'Mobile Push'], ['email', 'email', 'Email'], ['sms', 'sms', 'SMS']] as const).map(([value, path, label]) =>
        <Link key={value} to={`/admin/playgrounds/${path}`} className={`nav-link${view === value ? " active" : ""}`} aria-current={view === value ? "page" : undefined}><ActionIcon action="notification" />{label}</Link>)}
    </nav>}
    {channel ? <NotificationPlaygrounds key={view} channel={view} /> : <>
      <h1>{view === "admin" ? "Admin" : "Playgrounds"}</h1>
      <p>{view === "admin" ? "Administration tools and integration playgrounds." : "Choose a notification playground to inspect its queue and compose a message."}</p>
    </>}
  </div></section>;
}
