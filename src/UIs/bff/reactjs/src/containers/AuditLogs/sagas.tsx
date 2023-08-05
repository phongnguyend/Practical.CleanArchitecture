import { put, takeEvery } from "redux-saga/effects";
import axios from "./axios";

import * as actionTypes from "./actionTypes";
import * as actions from "./actions";

export function* fetchAuditLogsSaga(action) {
  yield put(actions.fetchAuditLogsStart());
  try {
    const response = yield axios.get("paged?page=" + action.page + "&pageSize=" + action.pageSize);
    const fetchedAuditLogs = response.data;
    yield put(actions.fetchAuditLogsSuccess(fetchedAuditLogs));
  } catch (error) {
    yield put(actions.fetchAuditLogsFail(error));
  }
}

export function* watchAuditLog() {
  yield takeEvery(actionTypes.FETCH_AUDIT_LOGS, fetchAuditLogsSaga);
}
