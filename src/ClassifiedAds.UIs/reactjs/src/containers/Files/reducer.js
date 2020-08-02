import { updateObject } from "../../shared/utility";
import * as actionTypes from "./actionTypes";

const initialState = {
  files: [],
  file: {
    name: "",
    description: "",
  },
  auditLogs: [],
  loading: false,
  saved: false,
  deleted: false,
  error: null,
};

/// Files
const fetchFilesStart = (state, action) => {
  return updateObject(state, { loading: true });
};

const fetchFilesSuccess = (state, action) => {
  return updateObject(state, {
    files: action.files,
    loading: false,
  });
};

const fetchFilesFail = (state, action) => {
  return updateObject(state, { loading: false });
};

/// Files

/// File
const fetchFileStart = (state, action) => {
  return updateObject(state, { loading: true });
};

const fetchFileSuccess = (state, action) => {
  return updateObject(state, {
    file: action.file,
    loading: false,
  });
};

const fetchFileFail = (state, action) => {
  return updateObject(state, { loading: false });
};

/// File

const saveFileStart = (state, action) => {
  return updateObject(state, { loading: true, saved: false });
};

const saveFileSuccess = (state, action) => {
  return updateObject(state, {
    file: action.file,
    loading: false,
    saved: true,
  });
};

const saveFileFail = (state, action) => {
  return updateObject(state, { loading: false, saved: false });
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case actionTypes.UPDATE_FILE:
      return updateObject(state, { file: action.file });
    case actionTypes.RESET_FILE:
      return updateObject(state, initialState);
    case actionTypes.FETCH_FILES_START:
      return fetchFilesStart(state, action);
    case actionTypes.FETCH_FILES_SUCCESS:
      return fetchFilesSuccess(state, action);
    case actionTypes.FETCH_FILES_FAIL:
    case actionTypes.FETCH_FILE_START:
      return fetchFileStart(state, action);
    case actionTypes.FETCH_FILE_SUCCESS:
      return fetchFileSuccess(state, action);
    case actionTypes.FETCH_FILE_FAIL:
      return fetchFileFail(state, action);
    case actionTypes.SAVE_FILE_START:
      return saveFileStart(state, action);
    case actionTypes.SAVE_FILE_SUCCESS:
      return saveFileSuccess(state, action);
    case actionTypes.SAVE_FILE_FAIL:
      return saveFileFail(state, action);
    case actionTypes.DELETE_FILE_START:
      return updateObject(state, {
        file: action.file,
        loading: true,
        deleted: false,
      });
    case actionTypes.DELETE_FILE_SUCCESS:
      return updateObject(state, {
        file: action.file,
        loading: false,
        deleted: true,
      });
    case actionTypes.DELETE_FILE_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
        deleted: false,
      });
    case actionTypes.DOWNLOAD_FILE_SUCCESS:
      const url = window.URL.createObjectURL(action.data);
      const element = document.createElement("a");
      element.href = url;
      element.download = action.file.fileName;
      document.body.appendChild(element);
      element.click();
      return state;
    case actionTypes.FETCH_FILE_AUDIT_LOGS_START:
      return updateObject(state, {
        loading: true,
      });
    case actionTypes.FETCH_FILE_AUDIT_LOGS_SUCCESS:
      return updateObject(state, {
        auditLogs: action.auditLogs,
        loading: false,
      });
    case actionTypes.FETCH_FILE_AUDIT_LOGS_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
      });
    default:
      return state;
  }
};

export default reducer;
