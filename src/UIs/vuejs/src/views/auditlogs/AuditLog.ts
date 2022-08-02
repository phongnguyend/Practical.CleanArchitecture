export interface IAuditLogEntry {
  id: string;
  userName: string;
  action: string;
  createdDateTime: string;
  data: any;
  highLight: any;
  log: string;
}
