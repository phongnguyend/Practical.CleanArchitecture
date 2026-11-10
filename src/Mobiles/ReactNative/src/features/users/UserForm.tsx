import { DateInput, Input, Toggle } from "../../shared/ui/fields";
import { EditorProps } from "../../shared/ui/RecordPage";
import { User } from "./model";
export function UserForm({ value, change, busy }: EditorProps<User>) {
  return (
    <>
      <Input
        label="Username"
        value={String(value.userName)}
        onChange={(text) => change({ ...value, userName: text })}
        disabled={busy}
      />
      <Input
        label="Email"
        value={String(value.email)}
        onChange={(text) => change({ ...value, email: text })}
        disabled={busy}
        email
      />
      <Toggle
        label="Email confirmed"
        value={value.emailConfirmed}
        onChange={(text) => change({ ...value, emailConfirmed: text })}
        disabled={busy}
      />
      <Input
        label="Phone number"
        value={String(value.phoneNumber)}
        onChange={(text) => change({ ...value, phoneNumber: text })}
        disabled={busy}
      />
      <Toggle
        label="Phone confirmed"
        value={value.phoneNumberConfirmed}
        onChange={(text) => change({ ...value, phoneNumberConfirmed: text })}
        disabled={busy}
      />
      <Toggle
        label="Two-factor enabled"
        value={value.twoFactorEnabled}
        onChange={(text) => change({ ...value, twoFactorEnabled: text })}
        disabled={busy}
      />
      <Toggle
        label="Lockout enabled"
        value={value.lockoutEnabled}
        onChange={(text) => change({ ...value, lockoutEnabled: text })}
        disabled={busy}
      />
      <DateInput
        label="Lockout end (ISO 8601)"
        value={String(value.lockoutEnd)}
        onChange={(text) => change({ ...value, lockoutEnd: text })}
        disabled={busy}
      />
      <Input
        label="Failed attempts"
        value={String(value.accessFailedCount)}
        onChange={(text) =>
          change({ ...value, accessFailedCount: Number(text) })
        }
        disabled={busy}
        numeric
      />
    </>
  );
}
