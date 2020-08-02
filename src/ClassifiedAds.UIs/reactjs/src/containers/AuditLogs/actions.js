import * as actionTypes from "./actionTypes";

/// FETCH AUDIT LOGS
export const fetchAuditLogsSuccess = (auditLogs) => {
  return {
    type: actionTypes.FETCH_AUDIT_LOGS_SUCCESS,
    auditLogs: auditLogs,
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

export const fetchAuditLogs = () => {
  return {
    type: actionTypes.FETCH_AUDIT_LOGS,
  };
};
/// FETCH AUDIT LOGS
