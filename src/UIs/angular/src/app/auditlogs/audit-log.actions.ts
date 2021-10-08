import { createAction, props } from "@ngrx/store";
import { IAuditLogEntry } from "./audit-log";

export const fetchAuditLogs = createAction("FETCH_AUDIT_LOGS");
export const fetchAuditLogsStart = createAction("FETCH_AUDIT_LOGS_START");
export const fetchAuditLogsSuccess = createAction(
  "FETCH_AUDIT_LOGS_SUCCESS",
  props<{ auditLogs: IAuditLogEntry[] }>()
);
export const fetchAuditLogsFail = createAction(
  "FETCH_AUDIT_LOGS_FAIL",
  props<{ error: string }>()
);
