export type Row = Record<string, any>;
export function auditSnapshot(row: Row): Row {
  const value = row.data ?? row.log;
  if (typeof value === "object" && value !== null) return value;
  try {
    const decoded = JSON.parse(value);
    return decoded && typeof decoded === "object" ? decoded : {};
  } catch {
    return {};
  }
}
export function apiError(status: number, text: string): string {
  if (status === 401)
    return "Session expired. Please sign out and sign in again.";
  if (status === 403) return "You do not have permission for this action.";
  try {
    const body = JSON.parse(text);
    if (body.errorMessage) return String(body.errorMessage);
    if (body.errors && typeof body.errors === "object")
      return Object.values(body.errors).flat().join("\n");
    if (body.detail || body.title) return String(body.detail || body.title);
  } catch {
    /* Non-JSON errors still retain their HTTP status. */
  }
  return `Request failed (${status}): ${text.slice(0, 1500)}`;
}
export const label = (row: Row) =>
  String(
    row.name ||
      row.userName ||
      row.key ||
      row.fileName ||
      row.action ||
      row.id ||
      "Record",
  );
