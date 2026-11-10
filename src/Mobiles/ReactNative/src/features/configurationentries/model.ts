export interface ConfigurationEntry {
  id: string;
  key: string;
  value: string;
  description: string;
  isSensitive: boolean;
  updatedDateTime: string;
}
export const emptyConfigurationEntry = (): ConfigurationEntry => ({
  id: "",
  key: "",
  value: "",
  description: "",
  isSensitive: false,
  updatedDateTime: "",
});
export function parseConfigurationEntry(
  value: Record<string, unknown>,
): ConfigurationEntry {
  return {
    id: String(value.id ?? ""),
    key: String(value.key ?? ""),
    value: String(value.value ?? ""),
    description: String(value.description ?? ""),
    isSensitive: Boolean(value.isSensitive ?? false),
    updatedDateTime: String(value.updatedDateTime ?? ""),
  };
}
export function validateConfigurationEntry(
  value: ConfigurationEntry,
): string | undefined {
  if (!value.key.trim()) return "Key is required.";
  if (!value.value.trim()) return "Value is required.";
}
export function configurationentriesPayload(value: ConfigurationEntry) {
  return {
    ...(value.id ? { id: value.id } : {}),
    key: value.key,
    value: value.value,
    description: value.description,
    isSensitive: value.isSensitive,
  };
}
