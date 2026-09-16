import type { ChangeEventHandler } from "react";
import { FileUp } from "lucide-react";
import "./FilePicker.css";

type Props = {
  id: string;
  name: string;
  label: string;
  hint: string;
  file: File | null;
  invalid?: boolean;
  accept?: string;
  onChange: ChangeEventHandler<HTMLInputElement>;
};

const FilePicker = ({ id, name, label, hint, file, invalid = false, accept, onChange }: Props) => (
  <div>
    <label className={`file-picker${invalid ? " file-picker-invalid" : ""}`}>
      <input id={id} name={name} type="file" accept={accept} title={file?.name} aria-label={label} aria-describedby={`${id}-hint`} aria-invalid={invalid} onChange={onChange} />
      <span className="file-picker-icon"><FileUp size={22} aria-hidden="true" /></span>
      <span className="file-picker-copy">
        <strong className="file-picker-title">{file ? file.name : "Choose a file"}</strong>
        <span id={`${id}-hint`} className="file-picker-hint">{hint}</span>
      </span>
      <span className="file-picker-button">Browse</span>
    </label>
    {invalid && <div className="text-danger small mt-1">Select a file</div>}
  </div>
);

export default FilePicker;
