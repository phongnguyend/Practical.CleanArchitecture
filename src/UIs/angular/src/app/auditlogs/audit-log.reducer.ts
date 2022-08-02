import { createReducer, on } from "@ngrx/store";
import { IAuditLogEntry } from "./audit-log";
import * as actions from "./audit-log.actions";

export interface AuditLogState {
  auditLogs: IAuditLogEntry[];
  totalItems: number,
  loading: boolean;
  error: string;
}

const initialState: AuditLogState = {
  auditLogs: [],
  totalItems: 0,
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
      auditLogs: action.items,
      totalItems: action.totalItems,
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
