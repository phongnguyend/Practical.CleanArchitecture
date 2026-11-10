import { Detail, dateText } from "../../shared/ui/fields";
import { User } from "./model";
export function UserSummary({
  value,
  changed = {},
}: {
  value: User;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail label="Email" value={value.email} changed={!!changed.email} />
    </>
  );
}
export function UserDetails({
  value,
  changed = {},
}: {
  value: User;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail
        label="Username"
        value={value.userName}
        changed={!!changed.userName}
      />
      <Detail label="Email" value={value.email} changed={!!changed.email} />
      <Detail
        label="Email confirmed"
        value={value.emailConfirmed ? "Yes" : "No"}
        changed={!!changed.emailConfirmed}
      />
      <Detail
        label="Phone number"
        value={value.phoneNumber}
        changed={!!changed.phoneNumber}
      />
      <Detail
        label="Phone confirmed"
        value={value.phoneNumberConfirmed ? "Yes" : "No"}
        changed={!!changed.phoneNumberConfirmed}
      />
      <Detail
        label="Two-factor enabled"
        value={value.twoFactorEnabled ? "Yes" : "No"}
        changed={!!changed.twoFactorEnabled}
      />
      <Detail
        label="Lockout enabled"
        value={value.lockoutEnabled ? "Yes" : "No"}
        changed={!!changed.lockoutEnabled}
      />
      <Detail
        label="Lockout end (ISO 8601)"
        value={dateText(value.lockoutEnd)}
        changed={!!changed.lockoutEnd}
      />
      <Detail
        label="Failed attempts"
        value={value.accessFailedCount}
        changed={!!changed.accessFailedCount}
      />
    </>
  );
}
export function UserHistory({
  value,
  changed = {},
}: {
  value: User;
  changed?: Record<string, boolean>;
}) {
  return (
    <>
      <Detail
        label="Username"
        value={value.userName}
        changed={!!changed.userName}
      />
      <Detail label="Email" value={value.email} changed={!!changed.email} />
      <Detail
        label="Email confirmed"
        value={value.emailConfirmed ? "Yes" : "No"}
        changed={!!changed.emailConfirmed}
      />
      <Detail
        label="Phone number"
        value={value.phoneNumber}
        changed={!!changed.phoneNumber}
      />
      <Detail
        label="Phone confirmed"
        value={value.phoneNumberConfirmed ? "Yes" : "No"}
        changed={!!changed.phoneNumberConfirmed}
      />
      <Detail
        label="Two-factor enabled"
        value={value.twoFactorEnabled ? "Yes" : "No"}
        changed={!!changed.twoFactorEnabled}
      />
      <Detail
        label="Lockout enabled"
        value={value.lockoutEnabled ? "Yes" : "No"}
        changed={!!changed.lockoutEnabled}
      />
      <Detail
        label="Lockout end (ISO 8601)"
        value={dateText(value.lockoutEnd)}
        changed={!!changed.lockoutEnd}
      />
      <Detail
        label="Failed attempts"
        value={value.accessFailedCount}
        changed={!!changed.accessFailedCount}
      />
    </>
  );
}
