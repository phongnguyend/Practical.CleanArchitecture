"use client";
import "./Membership.css";
import { useEffect, useState } from "react";
import rolesApi from "./axios";
import usersApi from "../Users/axios";
import ActionIcon from "../../components/ActionIcon/ActionIcon";
import { confirmModal } from "../../components/Modal/modalRuntime";

type Member = { id: string; userName: string; email?: string };
type Role = { id: string; name: string };

export function RoleMembers({ roleId }: { roleId: string }) {
  const [members, setMembers] = useState<Member[]>([]);
  const [users, setUsers] = useState<Member[]>([]);
  const [selected, setSelected] = useState("");
  const [search, setSearch] = useState("");
  const [busy, setBusy] = useState(true);
  const [error, setError] = useState("");
  const [revision, setRevision] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    setBusy(true);
    setError("");
    Promise.all([rolesApi.get<Member[]>(`${roleId}/users`, { signal: controller.signal }), usersApi.get<Member[]>("", { signal: controller.signal })])
      .then(([assigned, available]) => {
        setMembers(assigned.data);
        setUsers(available.data);
      }).catch(() => {
        if (!controller.signal.aborted) {
          setError("Unable to load role users. Please try again.");
        }
      }).finally(() => {
        if (!controller.signal.aborted) {
          setBusy(false);
        }
      });
    return () => controller.abort();
  }, [roleId, revision]);
  const change = async (user: Member, assign: boolean) => {
    if (busy) {
      return;
    }
    if (!assign && !await confirmModal(`Remove ${user.userName} from this role?`, { title: "Remove assigned role", confirmText: "Remove" })) {
      return;
    }
    setBusy(true);
    setError("");
    try {
      const path = `${roleId}/users/${user.id}`;
      if (assign) {
        await rolesApi.put(path);
      } else {
        await rolesApi.delete(path);
      }
      setMembers((await rolesApi.get<Member[]>(`${roleId}/users`)).data);
      setSelected("");
    } catch {
      setError("Unable to update role membership. Refresh and try again.");
    } finally {
      setBusy(false);
    }
  };
  const candidates = users.filter(user => !members.some(member => member.id === user.id)
    && `${user.userName} ${user.email || ""}`.toLowerCase().includes(search.toLowerCase()));
  return <section className="membership-section" aria-label="Role users">
    <div className="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
      <h2 className="h5 mb-0">Role users</h2>
      <button type="button" className="btn btn-outline-secondary" disabled={busy} onClick={() => setRevision(value => value + 1)}><ActionIcon action="refresh" />Refresh users</button>
    </div>
    <p className="membership-description">Manage who belongs to this role. Search by username or email, then select a user to assign.</p>
    {error && <p role="alert" className="alert alert-danger">{error}</p>}
    {busy && <p role="status">Loading role users...</p>}
    <div className="membership-assignment">
      <label htmlFor="role-user-search">Search users</label>
      <input id="role-user-search" placeholder="Username or email" className="form-control" value={search} onChange={event => { setSearch(event.target.value); setSelected(""); }} />
      <label htmlFor="role-user-select">User to assign</label>
      <select id="role-user-select" className="form-select" value={selected} disabled={busy} onChange={event => setSelected(event.target.value)}>
        <option value="">Select a user</option>
        {candidates.map(user => <option key={user.id} value={user.id}>{user.userName}{user.email ? ` (${user.email})` : ""}</option>)}
      </select>
      <button type="button" className="btn btn-primary" disabled={busy || !selected} onClick={() => {
        const user = candidates.find(candidate => candidate.id === selected);
        if (user) {
          void change(user, true);
        }
      }}><ActionIcon action="add" />Assign user</button>
    </div>
    {!busy && !error && members.length === 0 && <p>No users assigned.</p>}
    <ul className="list-group">
      {members.map(user => <li key={user.id} className="list-group-item d-flex flex-wrap align-items-center justify-content-between gap-3">
        <span style={{ overflowWrap: "anywhere" }}>{user.userName}{user.email && <small className="d-block">{user.email}</small>}</span>
        <button type="button" className="btn btn-outline-danger" disabled={busy} aria-label={`Remove ${user.userName} from role`} onClick={() => void change(user, false)}><ActionIcon action="delete" />Remove</button>
      </li>)}
    </ul>
  </section>;
}

export function AssignedRoles({ userId }: { userId: string }) {
  const [roles, setRoles] = useState<Role[]>([]);
  const [busy, setBusy] = useState(true);
  const [error, setError] = useState("");
  const [revision, setRevision] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    setBusy(true);
    setError("");
    usersApi.get<Role[]>(`${userId}/roles`, { signal: controller.signal }).then(response => setRoles(response.data)).catch(() => {
      if (!controller.signal.aborted) {
        setError("Unable to load assigned roles. Please try again.");
      }
    }).finally(() => {
      if (!controller.signal.aborted) {
        setBusy(false);
      }
    });
    return () => controller.abort();
  }, [userId, revision]);
  return <section className="membership-section" aria-label="Assigned roles">
    <div className="d-flex flex-wrap align-items-center justify-content-between gap-2 mb-3">
      <h2 className="h5 mb-0">Assigned roles</h2>
      <button type="button" className="btn btn-outline-secondary" disabled={busy} onClick={() => setRevision(value => value + 1)}><ActionIcon action="refresh" />Refresh roles</button>
    </div>
    {busy && <p role="status">Loading assigned roles...</p>}
    {error && <p className="alert alert-danger" role="alert">{error}</p>}
    {!busy && !error && roles.length === 0 && <p>No roles assigned.</p>}
    <ul className="assigned-roles">{roles.map(role => <li className="assigned-role" key={role.id}>{role.name}</li>)}</ul>
  </section>;
}
