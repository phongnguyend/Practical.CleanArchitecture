export interface IAuditLogEntry {
  userName: string;
  action: string;
  createdDateTime: Date;
  data: any;
  highLight: any;
}

export interface Paged<T> {
  totalItems: number,
  items: Array<T>
}
