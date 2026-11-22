import { useNavigate } from "react-router-dom";
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
    <div className="form-actions">
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
  );
}
