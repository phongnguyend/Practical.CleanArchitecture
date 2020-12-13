import * as actionTypes from "./actionTypes";

/// UPDATE FILE
export const updateFile = (file) => {
  return {
    type: actionTypes.UPDATE_FILE,
    file: file,
  };
};

export const resetFile = () => {
  return {
    type: actionTypes.RESET_FILE,
  };
};
/// UPDATE FILE

/// FETCH FILES
export const fetchFilesSuccess = (files) => {
  return {
    type: actionTypes.FETCH_FILES_SUCCESS,
    files: files,
  };
};

export const fetchFilesFail = (error) => {
  return {
    type: actionTypes.FETCH_FILES_FAIL,
    error: error,
  };
};

export const fetchFilesStart = () => {
  return {
    type: actionTypes.FETCH_FILES_START,
  };
};

export const fetchFiles = () => {
  return {
    type: actionTypes.FETCH_FILES,
  };
};
/// FETCH FILES

/// FETCH FILE
export const fetchFileSuccess = (file) => {
  return {
    type: actionTypes.FETCH_FILE_SUCCESS,
    file: file,
  };
};

export const fetchFileFail = (error) => {
  return {
    type: actionTypes.FETCH_FILE_FAIL,
    error: error,
  };
};

export const fetchFileStart = () => {
  return {
    type: actionTypes.FETCH_FILE_START,
  };
};

export const fetchFile = (id) => {
  return {
    type: actionTypes.FETCH_FILE,
    id: id,
  };
};
/// FETCH FILE

/// SAVE FILE
export const saveFileSuccess = (file) => {
  return {
    type: actionTypes.SAVE_FILE_SUCCESS,
    file: file,
  };
};

export const saveFileFail = (error) => {
  return {
    type: actionTypes.SAVE_FILE_FAIL,
    error: error,
  };
};

export const saveFileStart = () => {
  return {
    type: actionTypes.SAVE_FILE_START,
  };
};

export const saveFile = (file) => {
  return {
    type: actionTypes.SAVE_FILE,
    file: file,
  };
};
/// SAVE FILE

/// DELETE FILE
export const deleteFileSuccess = (file) => {
  return {
    type: actionTypes.DELETE_FILE_SUCCESS,
  };
};

export const deleteFileFail = (error) => {
  return {
    type: actionTypes.DELETE_FILE_FAIL,
    error: error,
  };
};

export const deleteFileStart = () => {
  return {
    type: actionTypes.DELETE_FILE_START,
  };
};

export const deleteFile = (file) => {
  return {
    type: actionTypes.DELETE_FILE,
    file: file,
  };
};
/// DELETE FILE

/// DOWNLOAD FILE
export const downloadFileSuccess = (file, data) => {
  return {
    type: actionTypes.DOWNLOAD_FILE_SUCCESS,
    file: file,
    data: data,
  };
};

export const downloadFileFail = (error) => {
  return {
    type: actionTypes.DOWNLOAD_FILE_FAIL,
    error: error,
  };
};

export const downloadFileStart = () => {
  return {
    type: actionTypes.DOWNLOAD_FILE_START,
  };
};

export const downloadFile = (file) => {
  return {
    type: actionTypes.DOWNLOAD_FILE,
    file: file,
  };
};
/// DOWNLOAD FILE

/// FETCH AUDIT LOGS
export const fetchAuditLogsSuccess = (auditLogs) => {
  return {
    type: actionTypes.FETCH_FILE_AUDIT_LOGS_SUCCESS,
    auditLogs: auditLogs,
  };
};

export const fetchAuditLogsFail = (error) => {
  return {
    type: actionTypes.FETCH_FILE_AUDIT_LOGS_FAIL,
    error: error,
  };
};

export const fetchAuditLogsStart = () => {
  return {
    type: actionTypes.FETCH_FILE_AUDIT_LOGS_START,
  };
};

export const fetchAuditLogs = (file) => {
  return {
    type: actionTypes.FETCH_FILE_AUDIT_LOGS,
    file: file,
  };
};
/// FETCH AUDIT LOGS
