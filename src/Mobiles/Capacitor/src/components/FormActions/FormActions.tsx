import { useNavigate } from "react-router-dom";
import { useLayoutEffect, useRef, useState } from "react";
import { confirmModal } from "../Modal/modalRuntime";
import ActionIcon from "../ActionIcon/ActionIcon";
export default function FormActions({
  dirty,
  to,
}: {
  dirty: boolean;
  to: string;
}) {
  const navigate = useNavigate();
  const footer = useRef<HTMLDivElement>(null);
  const [footerHeight, setFooterHeight] = useState(80);
  useLayoutEffect(() => {
    if (!footer.current) {
      return;
    }
    const observer = new ResizeObserver(([entry]) => {
      setFooterHeight(entry.target.getBoundingClientRect().height);
    });
    observer.observe(footer.current);
    return () => observer.disconnect();
  }, []);
  async function cancel() {
    if (
      dirty &&
      !(await confirmModal("Discard unsaved changes?", {
        title: "Confirm",
        confirmText: "Confirm",
      }))
    ) {
      return;
    }
    navigate(to);
  }
  return (
    <>
      <div aria-hidden="true" style={{ height: footerHeight }} />
      <div className="form-actions" ref={footer}>
        <button
          type="button"
          className="btn btn-secondary"
          onClick={() => void cancel()}
        >
          <ActionIcon action="cancel" />
          Cancel
        </button>
        <button type="submit" className="btn btn-primary">
          <ActionIcon action="save" />
          Save
        </button>
      </div>
    </>
  );
}
