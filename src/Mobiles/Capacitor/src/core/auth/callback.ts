export const loginRedirect = "classifiedads.capacitor:/oauthredirect";
export const logoutRedirect = "classifiedads.capacitor:/logout";
export function isAuthCallback(
  value: string,
  expected = loginRedirect,
): boolean {
  try {
    const actual = new URL(value);
    const target = new URL(expected);
    return (
      actual.protocol === target.protocol &&
      actual.host === target.host &&
      actual.pathname === target.pathname
    );
  } catch {
    return false;
  }
}
export function safeReturnPath(value: string | null): string {
  return value?.startsWith("/") &&
    !value.startsWith("//") &&
    !value.includes("\\")
    ? value
    : "/home";
}
