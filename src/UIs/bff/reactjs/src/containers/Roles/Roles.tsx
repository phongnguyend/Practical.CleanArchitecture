import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Modal, Button } from "react-bootstrap";
import axios from "./axios";
import ActionIcon from "../../components/ActionIcon/ActionIcon";

interface Role { id: string; name: string; normalizedName?: string }

export default function Roles({ mode }: { mode: "list" | "view" }) {
  const { id } = useParams();
  const [roles, setRoles] = useState<Role[]>([]);
  const [role, setRole] = useState<Role>({ id: "", name: "" });
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);
  const [showAddModal, setShowAddModal] = useState(false);
  const [newName, setNewName] = useState("");
  const [editingRole, setEditingRole] = useState<Role | null>(null);
  const [editName, setEditName] = useState("");

  const load = useCallback(async () => {
    try {
      setError("");
      if (mode === "list") setRoles((await axios.get<Role[]>("")).data);
      else if (id) setRole((await axios.get<Role>(id)).data);
    } catch (err) {
      setError("Unable to load roles. Please try again.");
    }
  }, [id, mode]);

  useEffect(() => { void load(); }, [load]);

  const saveEdit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!editingRole || !editName.trim()) { setError("Enter a role name."); return; }
    setSaving(true);
    setError("");
    try {
      await axios.put<Role>(editingRole.id, { ...editingRole, name: editName.trim() });
      setEditingRole(null);
      await load();
    } catch (err) { setError("Unable to save the role. Please try again."); }
    finally { setSaving(false); }
  };

  const openEdit = (item: Role) => {
    setError("");
    setEditName(item.name);
    setEditingRole(item);
  };

  const remove = async (item: Role) => {
    if (!window.confirm(`Delete role ${item.name}?`)) return;
    try {
      await axios.delete(item.id);
      await load();
    } catch (err) { setError("Unable to delete the role. Please try again."); }
  };

  const add = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!newName.trim()) { setError("Enter a role name."); return; }
    setSaving(true);
    setError("");
    try {
      await axios.post<Role>("", { name: newName.trim() });
      setShowAddModal(false);
      setNewName("");
      await load();
    } catch (err) { setError("Unable to add the role. Please try again."); }
    finally { setSaving(false); }
  };

  return <div className="card mt-2">
    <div className="card-header"><ActionIcon action="roles" />{mode === "list" ? "Roles" : "Role Details"}
      {mode === "list" && <button className="btn btn-primary float-end" type="button" onClick={() => { setError(""); setNewName(""); setShowAddModal(true); }}><ActionIcon action="add" />Add Role</button>}
    </div>
    <div className="card-body">
      {error && !showAddModal && !editingRole && <div className="alert alert-danger" role="alert">{error}</div>}
      {mode === "list" && <div className="table-responsive"><table className="table"><thead><tr><th>Name</th><th>Actions</th></tr></thead><tbody>
        {roles.map(item => <tr key={item.id}><td><Link to={`/roles/${item.id}`}><ActionIcon action="view" />{item.name}</Link></td><td>
          <button className="btn btn-primary" type="button" onClick={() => openEdit(item)}><ActionIcon action="edit" />Edit</button>{" "}
          <button className="btn btn-danger" type="button" onClick={() => void remove(item)}><ActionIcon action="delete" />Delete</button>
        </td></tr>)}
      </tbody></table></div>}
      {mode === "view" && <><dl><dt>Name</dt><dd>{role.name}</dd><dt>Normalized Name</dt><dd>{role.normalizedName}</dd></dl>
        <button className="btn btn-primary" type="button" disabled={!role.id} onClick={() => openEdit(role)}><ActionIcon action="edit" />Edit</button></>}
    </div>
    {mode !== "list" && <div className="card-footer"><Link className="btn btn-outline-secondary" to="/roles"><ActionIcon action="back" />Back</Link></div>}
    <Modal show={showAddModal} onHide={() => setShowAddModal(false)}>
      <form onSubmit={add}>
        <Modal.Header closeButton><Modal.Title><ActionIcon action="add" />Add Role</Modal.Title></Modal.Header>
        <Modal.Body>
          {error && <div className="alert alert-danger" role="alert">{error}</div>}
          <label className="form-label" htmlFor="newRoleName">Name</label>
          <input className="form-control" id="newRoleName" value={newName} onChange={event => setNewName(event.target.value)} required autoFocus />
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" type="button" onClick={() => setShowAddModal(false)}><ActionIcon action="cancel" />Cancel</Button>
          <Button variant="primary" type="submit" disabled={saving}><ActionIcon action="save" />{saving ? "Saving..." : "Save"}</Button>
        </Modal.Footer>
      </form>
    </Modal>
    <Modal show={!!editingRole} onHide={() => setEditingRole(null)}>
      <form onSubmit={saveEdit}>
        <Modal.Header closeButton><Modal.Title><ActionIcon action="edit" />Edit Role</Modal.Title></Modal.Header>
        <Modal.Body>
          {error && <div className="alert alert-danger" role="alert">{error}</div>}
          <label className="form-label" htmlFor="editRoleName">Name</label>
          <input className="form-control" id="editRoleName" value={editName} onChange={event => setEditName(event.target.value)} required autoFocus />
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" type="button" onClick={() => setEditingRole(null)}><ActionIcon action="cancel" />Cancel</Button>
          <Button variant="primary" type="submit" disabled={saving}><ActionIcon action="save" />{saving ? "Saving..." : "Save"}</Button>
        </Modal.Footer>
      </form>
    </Modal>
  </div>;
}
