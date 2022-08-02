import * as actionTypes from "./actionTypes";

/// FETCH AUDIT LOGS
export const fetchAuditLogsSuccess = (auditLogs) => {
  return {
    type: actionTypes.FETCH_AUDIT_LOGS_SUCCESS,
    auditLogs: auditLogs.items,
    totalItems: auditLogs.totalItems
  };
};

export const fetchAuditLogsFail = (error) => {
  return {
    type: actionTypes.FETCH_AUDIT_LOGS_FAIL,
    error: error,
  };
};

export const fetchAuditLogsStart = () => {
  return {
    type: actionTypes.FETCH_AUDIT_LOGS_START,
  };
};

export const fetchAuditLogs = (page: number, pageSize: number) => {
  return {
    type: actionTypes.FETCH_AUDIT_LOGS,
    page,
    pageSize
  };
};
/// FETCH AUDIT LOGS
