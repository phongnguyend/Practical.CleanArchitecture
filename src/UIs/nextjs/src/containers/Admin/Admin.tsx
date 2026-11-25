"use client";

import Link from "next/link";
import NotificationPlaygrounds from "./NotificationPlaygrounds";
import ActionIcon from "../../components/ActionIcon/ActionIcon";

export default function Admin({ view = "admin" }: { view?: "admin" | "playgrounds" | "mobilepush" | "email" | "sms" }) {
  if (view === "mobilepush" || view === "email" || view === "sms") {
    return <NotificationPlaygrounds key={view} channel={view} />;
  }
  return <section className="card"><div className="card-body">
    <nav aria-label="Breadcrumb" className="mb-3 d-flex flex-wrap gap-2"><Link href="/admin">Admin</Link>{view === "playgrounds" && <><span>/</span><span aria-current="page">Playgrounds</span></>}</nav>
    <h1>{view === "admin" ? "Admin" : "Playgrounds"}</h1>
    <p>{view === "admin" ? "Administration tools and integration playgrounds." : "Exercise notification integrations using the existing queues."}</p>
    <div className="d-flex flex-wrap gap-2">
      {view === "admin" ? <Link className="btn btn-outline-primary" href="/admin/playgrounds"><ActionIcon action="settings" />Playgrounds</Link> : <>
        <Link className="btn btn-outline-primary" href="/admin/playgrounds/mobile-push"><ActionIcon action="notification" />Mobile Push</Link>
        <Link className="btn btn-outline-primary" href="/admin/playgrounds/email"><ActionIcon action="notification" />Email</Link>
        <Link className="btn btn-outline-primary" href="/admin/playgrounds/sms"><ActionIcon action="notification" />SMS</Link>
      </>}
    </div>
  </div></section>;
}
