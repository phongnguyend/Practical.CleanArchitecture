import { createReducer, on } from "@ngrx/store";
import { IAuditLogEntry } from "./audit-log";
import * as actions from "./audit-log.actions";

export interface AuditLogState {
  auditLogs: IAuditLogEntry[];
  loading: boolean;
  error: string;
}

const initialState: AuditLogState = {
  auditLogs: [],
  loading: false,
  error: null,
};

export const auditLogReducer = createReducer<AuditLogState>(
  initialState,
  on(actions.fetchAuditLogsStart, (state): AuditLogState => {
    return {
      ...state,
      auditLogs: [],
      loading: true,
    };
  }),
  on(actions.fetchAuditLogsSuccess, (state, action): AuditLogState => {
    return {
      ...state,
      auditLogs: action.auditLogs,
      loading: false,
    };
  }),
  on(actions.fetchAuditLogsFail, (state, action): AuditLogState => {
    return {
      ...state,
      error: action.error,
      loading: false,
    };
  })
);
