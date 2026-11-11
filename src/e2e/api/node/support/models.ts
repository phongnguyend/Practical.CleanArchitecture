export interface RecordModel { id: string; name?: string; key?: string; userName?: string }
export interface Product extends RecordModel { name: string; code: string; description: string }
export interface ConfigurationEntry extends RecordModel { key: string; value: string; description: string; isSensitive: boolean }
export interface FileEntry extends RecordModel { name: string; description: string; fileName: string; encrypted: boolean; size: number }
export interface User extends RecordModel { userName: string; email: string; phoneNumber?: string; lockoutEnd?: string | null }
export interface AuditEntry { action: string; data?: string }
