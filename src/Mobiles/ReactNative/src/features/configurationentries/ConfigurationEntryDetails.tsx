import { Detail, dateText } from "../../shared/ui/fields";
import { ConfigurationEntry } from "./model";
export function ConfigurationEntrySummary({
  value,
  changed = {},
}: {
  value: ConfigurationEntry;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail
        label="Value"
        value={value.isSensitive ? "******" : value.value}
        changed={!!changed.value}
      />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
      <Detail
        label="Updated time"
        value={dateText(value.updatedDateTime)}
        changed={!!changed.updatedDateTime}
      />
    </>
  );
}
export function ConfigurationEntryDetails({
  value,
  changed = {},
}: {
  value: ConfigurationEntry;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail label="Key" value={value.key} changed={!!changed.key} />
      <Detail
        label="Value"
        value={value.isSensitive ? "******" : value.value}
        changed={!!changed.value}
      />
      <Detail
        label="Description"
        value={value.description}
        changed={!!changed.description}
      />
      <Detail
        label="Updated time"
        value={dateText(value.updatedDateTime)}
        changed={!!changed.updatedDateTime}
      />
      <Detail
        label="Sensitive"
        value={value.isSensitive ? "Yes" : "No"}
        changed={!!changed.isSensitive}
      />
    </>
  );
}
export function ConfigurationEntryHistory({
  value,
  changed = {},
}: {
  value: ConfigurationEntry;
  changed?: Record<string, boolean>;
}) {
  return <></>;
}
