export interface User {
  id: string;
  userName: string;
  email: string;
  emailConfirmed: boolean;
  phoneNumber: string;
  phoneNumberConfirmed: boolean;
  twoFactorEnabled: boolean;
  lockoutEnabled: boolean;
  lockoutEnd: string;
  accessFailedCount: number;
}
export const emptyUser = (): User => ({
  id: "",
  userName: "",
  email: "",
  emailConfirmed: false,
  phoneNumber: "",
  phoneNumberConfirmed: false,
  twoFactorEnabled: false,
  lockoutEnabled: false,
  lockoutEnd: "",
  accessFailedCount: 0,
});
export function parseUser(value: Record<string, unknown>): User {
  return {
    id: String(value.id ?? ""),
    userName: String(value.userName ?? ""),
    email: String(value.email ?? ""),
    emailConfirmed: Boolean(value.emailConfirmed ?? false),
    phoneNumber: String(value.phoneNumber ?? ""),
    phoneNumberConfirmed: Boolean(value.phoneNumberConfirmed ?? false),
    twoFactorEnabled: Boolean(value.twoFactorEnabled ?? false),
    lockoutEnabled: Boolean(value.lockoutEnabled ?? false),
    lockoutEnd: String(value.lockoutEnd ?? ""),
    accessFailedCount: Number(value.accessFailedCount ?? 0),
  };
}
export function validateUser(value: User): string | undefined {
  if (!value.userName.trim()) return "Username is required.";
  if (value.userName.trim().length < 3)
    return "Username needs at least 3 characters.";
  if (!value.email.trim()) return "Email is required.";
  if (value.email.trim().length < 3)
    return "Email needs at least 3 characters.";
  if (value.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value.email.trim()))
    return "Enter a valid email.";
  if (value.lockoutEnd && Number.isNaN(Date.parse(value.lockoutEnd)))
    return "Enter a valid date and time.";
  if (!Number.isInteger(value.accessFailedCount) || value.accessFailedCount < 0)
    return "Failed attempts must be a nonnegative integer.";
}
export function usersPayload(value: User) {
  return {
    ...(value.id ? { id: value.id } : {}),
    userName: value.userName,
    email: value.email.trim(),
    emailConfirmed: value.emailConfirmed,
    phoneNumber: value.phoneNumber,
    phoneNumberConfirmed: value.phoneNumberConfirmed,
    twoFactorEnabled: value.twoFactorEnabled,
    lockoutEnabled: value.lockoutEnabled,
    lockoutEnd: value.lockoutEnd
      ? new Date(value.lockoutEnd).toISOString()
      : null,
    accessFailedCount: value.accessFailedCount,
  };
}
