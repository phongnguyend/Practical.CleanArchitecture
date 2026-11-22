import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { isAuthenticated, login, logout } from "../Auth/authService";
import { destinations } from "../../components/Nav/Nav";
import ActionIcon from "../../components/ActionIcon/ActionIcon";
import Modal from "../../components/Modal/Modal";
import { openExternal } from "../../core/files/native-files";
export default function Home() {
  const [now, setNow] = useState(new Date());
  const [busy, setBusy] = useState(false);
  const [confirm, setConfirm] = useState(false);
  const [error, setError] = useState("");
  useEffect(() => {
    const timer = setInterval(() => setNow(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);
  const authenticate = async (out = false) => {
    setBusy(true);
    setError("");
    try {
      await (out ? logout() : login());
      setConfirm(false);
    } catch {
      setError("Authentication did not complete. Please try again.");
    } finally {
      setBusy(false);
    }
  };
  if (!isAuthenticated()) {
    return (
      <section className="signed-out">
        <h1>Your workspace, anywhere.</h1>
        <button
          className="btn btn-primary"
          disabled={busy}
          onClick={() => void authenticate()}
        >
          <ActionIcon action="login" />
          Sign in
        </button>
        {error && <p role="alert">{error}</p>}
      </section>
    );
  }
  return (
    <div className="home-actions">
      <section className="card">
        <div className="card-body">
          <h1>Welcome to Classified Ads</h1>
          <p>Capacitor &middot; {now.toLocaleString("en-US")}</p>
          <p>Developed by Phong Nguyen &middot; @phongnguyend</p>
          <button
            className="btn btn-secondary w-100"
            onClick={() =>
              void openExternal(
                "https://github.com/phongnguyend/Practical.CleanArchitecture",
              )
            }
          >
            <ActionIcon action="external" />
            Practical.CleanArchitecture
          </button>
        </div>
      </section>
      {destinations
        .filter(
          ([path]) => !["/home", "/auditlogs", "/notifications"].includes(path),
        )
        .map(([path, title, icon]) => (
          <Link className="btn btn-secondary" key={path} to={path}>
            <ActionIcon action={icon} />
            {title}
          </Link>
        ))}
      <button className="btn btn-secondary" onClick={() => setConfirm(true)}>
        <ActionIcon action="logout" />
        Sign out
      </button>
      {error && <p role="alert">{error}</p>}
      <Modal
        show={confirm}
        onHide={() => {
          if (!busy) {
            setConfirm(false);
          }
        }}
      >
        <Modal.Header>
          <Modal.Title>Confirm</Modal.Title>
        </Modal.Header>
        <Modal.Body>Sign out of this device?</Modal.Body>
        <Modal.Footer>
          <button
            className="btn btn-secondary"
            disabled={busy}
            onClick={() => setConfirm(false)}
          >
            <ActionIcon action="cancel" />
            Cancel
          </button>
          <button
            className="btn btn-primary"
            disabled={busy}
            onClick={() => void authenticate(true)}
          >
            <ActionIcon action="logout" />
            Sign out
          </button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}
