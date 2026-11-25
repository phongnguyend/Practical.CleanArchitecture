import PushInbox from "./PushInbox";
import { useNotifications, clearNotifications } from "../../core/notifications";
import ActionIcon from "../../components/ActionIcon/ActionIcon";
export default function Notifications() {
  const { status, messages } = useNotifications();
  return (
    <section className="card">
      <div className="card-header">Notifications</div>
      <div className="card-body">
        <PushInbox />
        <p role="status">{status}</p>
        <button
          className="btn btn-secondary"
          onClick={clearNotifications}
          disabled={!messages.length}
        >
          <ActionIcon action="delete" />
          Clear notifications
        </button>
        {!messages.length && <p className="mt-3">No notifications yet.</p>}
        <ol className="notification-list">
          {messages.map((item) => (
            <li key={item.id}>
              <time>{item.time}</time>
              <p>{item.text}</p>
            </li>
          ))}
        </ol>
      </div>
    </section>
  );
}
