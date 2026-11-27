import styles from "./AuditAction.module.css";

export default function AuditAction({ action }: { action?: string | null }) {
  const label = action?.trim() || "Event";
  const kind = label.toUpperCase().split(/[_\s-]/)[0];
  return <span className={styles.badge} data-action={kind}>{label}</span>;
}
