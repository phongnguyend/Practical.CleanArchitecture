export interface StoredFile {
  id: string;
  name: string;
  description: string;
  encrypted: boolean;
  fileName: string;
  size: number;
  uploadedTime: string;
  fileLocation: string;
}
export const emptyStoredFile = (): StoredFile => ({
  id: "",
  name: "",
  description: "",
  encrypted: false,
  fileName: "",
  size: 0,
  uploadedTime: "",
  fileLocation: "",
});
export function parseStoredFile(value: Record<string, unknown>): StoredFile {
  return {
    id: String(value.id ?? ""),
    name: String(value.name ?? ""),
    description: String(value.description ?? ""),
    encrypted: Boolean(value.encrypted ?? false),
    fileName: String(value.fileName ?? ""),
    size: Number(value.size ?? 0),
    uploadedTime: String(value.uploadedTime ?? ""),
    fileLocation: String(value.fileLocation ?? ""),
  };
}
export function validateStoredFile(value: StoredFile): string | undefined {
  if (!value.name.trim()) return "Name is required.";
  if (value.name.trim().length < 3) return "Name needs at least 3 characters.";
  if (!value.description.trim()) return "Description is required.";
  if (value.description.length > 100)
    return "Description must be at most 100 characters.";
}
export function filesPayload(value: StoredFile) {
  return {
    ...(value.id ? { id: value.id } : {}),
    name: value.name,
    description: value.description,
    encrypted: value.encrypted,
    ...(value.id ? { fileName: value.fileName } : {}),
  };
}
