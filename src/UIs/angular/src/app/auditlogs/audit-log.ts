export interface IAuditLogEntry {
  UserName: string;
  Action: string;
  CreatedDateTime: Date;
  Data: any;
  HighLight: any;
}
