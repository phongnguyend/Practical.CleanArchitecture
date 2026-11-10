import * as SecureStore from "expo-secure-store";

export type Appearance = "system" | "light" | "dark";
export async function readTheme(): Promise<Appearance> {
  try {
    const value = await SecureStore.getItemAsync("classifiedads.theme");
    return value === "light" || value === "dark" ? value : "system";
  } catch {
    return "system";
  }
}
export async function saveTheme(value: Appearance) {
  try {
    await SecureStore.setItemAsync("classifiedads.theme", value);
  } catch {
    /* Apply for this session when storage is unavailable. */
  }
}
