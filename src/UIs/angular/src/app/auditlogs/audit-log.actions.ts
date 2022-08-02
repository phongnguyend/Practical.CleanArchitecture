import { createAction, props } from "@ngrx/store";
import { IAuditLogEntry, Paged } from "./audit-log";

export const fetchAuditLogs = createAction("FETCH_AUDIT_LOGS", props<{ page: number, pageSize: number }>());
export const fetchAuditLogsStart = createAction("FETCH_AUDIT_LOGS_START");
export const fetchAuditLogsSuccess = createAction(
  "FETCH_AUDIT_LOGS_SUCCESS",
  props<Paged<IAuditLogEntry>>()
);
export const fetchAuditLogsFail = createAction(
  "FETCH_AUDIT_LOGS_FAIL",
  props<{ error: string }>()
);
