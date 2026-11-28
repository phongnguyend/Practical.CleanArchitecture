import { Membership } from "../roles/Membership";
import { Detail } from "../../shared/ui/fields";
import { Role } from "./model";
export function RoleSummary({
  value,
  changed = {},
}: {
  value: Role;
  changed?: Record<string, boolean>;
}) {
  return <></>;
}
export function RoleDetails({
  value,
  changed = {},
}: {
  value: Role;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail label="Name" value={value.name} changed={!!changed.name} />
      <Detail
        label="Normalized name"
        value={value.normalizedName}
        changed={!!changed.normalizedName}
      />
      <Membership key={value.id} roleId={value.id} />
    </>
  );
}
export function RoleHistory({
  value,
  changed = {},
}: {
  value: Role;
  changed?: Record<string, boolean>;
}) {
  return <></>;
}
