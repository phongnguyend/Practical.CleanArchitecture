export interface Role {
  id: string;
  name: string;
  normalizedName: string;
}
export const emptyRole = (): Role => ({ id: "", name: "", normalizedName: "" });
export function parseRole(value: Record<string, unknown>): Role {
  return {
    id: String(value.id ?? ""),
    name: String(value.name ?? ""),
    normalizedName: String(value.normalizedName ?? ""),
  };
}
export function validateRole(value: Role): string | undefined {
  if (!value.name.trim()) return "Name is required.";
}
export function rolesPayload(value: Role) {
  return { ...(value.id ? { id: value.id } : {}), name: value.name };
}
