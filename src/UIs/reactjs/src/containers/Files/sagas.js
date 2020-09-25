import { put, takeEvery } from "redux-saga/effects";
import axios from "./axios";

import * as actionTypes from "./actionTypes";
import * as actions from "./actions";

export function* fetchFilesSaga(action) {
  yield put(actions.fetchFilesStart());
  try {
    const response = yield axios.get("");
    const fetchedFiles = response.data;
    yield put(actions.fetchFilesSuccess(fetchedFiles));
  } catch (error) {
    yield put(actions.fetchFilesFail(error));
  }
}

export function* fetchFileSaga(action) {
  yield put(actions.fetchFileStart());
  try {
    const response = yield axios.get(action.id);
    const fetchedFile = response.data;
    yield put(actions.fetchFileSuccess(fetchedFile));
  } catch (error) {
    yield put(actions.fetchFileFail(error));
  }
}

export function* saveFileSaga(action) {
  yield put(actions.saveFileStart());
  try {
    const formData = action.file.id ? null : new FormData();
    if (formData) {
      formData.append("formFile", action.file.formFile);
      formData.append("name", action.file.name);
      formData.append("description", action.file.description);
    }
    const response = action.file.id
      ? yield axios.put(action.file.id, action.file)
      : yield axios.post("", formData);
    const file = response.data;
    yield put(actions.saveFileSuccess(file));
  } catch (error) {
    console.log(error);
    yield put(actions.saveFileFail(error));
  }
}

export function* deleteFileSaga(action) {
  yield put(actions.deleteFileStart());
  try {
    const response = yield axios.delete(action.file.id, action.file);
    yield put(actions.deleteFileSuccess(action.file));
    yield put(actions.fetchFiles());
  } catch (error) {
    console.log(error);
    yield put(actions.deleteFileFail(error));
  }
}

export function* downloadFileSaga(action) {
  yield put(actions.downloadFileStart());
  try {
    const response = yield axios.get(action.file.id + "/download", {
      responseType: "blob",
    });
    yield put(actions.downloadFileSuccess(action.file, response.data));
  } catch (error) {
    console.log(error);
    yield put(actions.deleteFileFail(error));
  }
}

export function* fetchAuditLogsSaga(action) {
  yield put(actions.fetchAuditLogsStart());
  try {
    const response = yield axios.get(action.file.id + "/auditLogs");
    const fetchedAuditLogs = response.data;
    yield put(actions.fetchAuditLogsSuccess(fetchedAuditLogs));
  } catch (error) {
    yield put(actions.fetchAuditLogsFail(error));
  }
}

export function* watchFile() {
  yield takeEvery(actionTypes.FETCH_FILES, fetchFilesSaga);
  yield takeEvery(actionTypes.FETCH_FILE, fetchFileSaga);
  yield takeEvery(actionTypes.SAVE_FILE, saveFileSaga);
  yield takeEvery(actionTypes.DELETE_FILE, deleteFileSaga);
  yield takeEvery(actionTypes.DOWNLOAD_FILE, downloadFileSaga);
  yield takeEvery(actionTypes.FETCH_FILE_AUDIT_LOGS, fetchAuditLogsSaga);
}
