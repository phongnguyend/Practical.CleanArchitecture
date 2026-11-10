import { Detail, dateText } from "../../shared/ui/fields";
import { StoredFile } from "./model";
export function StoredFileSummary({
  value,
  changed = {},
}: {
  value: StoredFile;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail
        label="File name"
        value={value.fileName}
        changed={!!changed.fileName}
      />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
      <Detail
        label="Size"
        value={`${value.size} bytes`}
        changed={!!changed.size}
      />
      <Detail
        label="Uploaded time"
        value={dateText(value.uploadedTime)}
        changed={!!changed.uploadedTime}
      />
    </>
  );
}
export function StoredFileDetails({
  value,
  changed = {},
}: {
  value: StoredFile;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail label="Name" value={value.name} changed={!!changed.name} />
      <Detail
        label="File name"
        value={value.fileName}
        changed={!!changed.fileName}
      />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
      <Detail
        label="Size"
        value={`${value.size} bytes`}
        changed={!!changed.size}
      />
      <Detail
        label="Uploaded time"
        value={dateText(value.uploadedTime)}
        changed={!!changed.uploadedTime}
      />
      <Detail
        label="Encrypted"
        value={value.encrypted ? "Yes" : "No"}
        changed={!!changed.encrypted}
      />
      <Detail
        label="File location"
        value={value.fileLocation}
        changed={!!changed.fileLocation}
      />
    </>
  );
}
export function StoredFileHistory({
  value,
  changed = {},
}: {
  value: StoredFile;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail label="Name" value={value.name} changed={!!changed.name} />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
      <Detail
        label="File name"
        value={value.fileName}
        changed={!!changed.fileName}
      />
      <Detail
        label="File location"
        value={value.fileLocation}
        changed={!!changed.fileLocation}
      />
    </>
  );
}
