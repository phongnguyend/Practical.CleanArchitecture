import { put, takeEvery, takeLatest } from "redux-saga/effects";
import axios from "./axios";

import * as actionTypes from "./actionTypes";
import * as actions from "./actions";

export function* fetchUsersSaga(action: any): Generator<any, any, any> {
  yield put(actions.fetchUsersStart());
  try {
    const response = yield axios.get("");
    const fetchedUsers = response.data;
    yield put(actions.fetchUsersSuccess(fetchedUsers));
  } catch (error) {
    yield put(actions.fetchUsersFail(error));
  }
}

export function* fetchUserSaga(action: any): Generator<any, any, any> {
  yield put(actions.fetchUserStart());
  try {
    const response = yield axios.get(action.id);
    const fetchedUser = response.data;
    yield put(actions.fetchUserSuccess(fetchedUser));
  } catch (error) {
    yield put(actions.fetchUserFail(error));
  }
}

export function* saveUserSaga(action: any): Generator<any, any, any> {
  yield put(actions.saveUserStart());
  try {
    const response = action.user.id
      ? yield axios.put(action.user.id, action.user)
      : yield axios.post("", action.user);
    const user = response.data;
    yield put(actions.saveUserSuccess(user));
  } catch (error) {
    yield put(actions.saveUserFail(error));
  }
}

export function* deleteUserSaga(action: any): Generator<any, any, any> {
  yield put(actions.deleteUserStart());
  try {
    const response = yield axios.delete(action.user.id, action.user);
    yield put(actions.deleteUserSuccess(action.user));
    yield put(actions.fetchUsers());
  } catch (error) {
    yield put(actions.deleteUserFail(error));
  }
}

export function* fetchAuditLogsSaga(action: any): Generator<any, any, any> {
  yield put(actions.fetchAuditLogsStart());
  try {
    const response = yield axios.get(action.user.id + "/auditLogs");
    const fetchedAuditLogs = response.data;
    yield put(actions.fetchAuditLogsSuccess(fetchedAuditLogs));
  } catch (error) {
    yield put(actions.fetchAuditLogsFail(error));
  }
}

export function* setPasswordSaga(action: any): Generator<any, any, any> {
  yield put(actions.setPasswordStart());
  try {
    const response = yield axios.put(
      action.password.id + "/password",
      action.password,
    );
    yield put(actions.setPasswordSuccess());
  } catch (error) {
    yield put(actions.setPasswordFail(error));
  }
}

export function* sendPasswordResetEmailSaga(
  action: any,
): Generator<any, any, any> {
  yield put(actions.sendPasswordResetEmailStart());
  try {
    const response = yield axios.post(action.id + "/passwordresetemail", {
      id: action.id,
    });
    yield put(actions.sendPasswordResetEmailSuccess());
  } catch (error) {
    yield put(actions.sendPasswordResetEmailFail(error));
  }
}

export function* sendEmailAddressConfirmationEmailSaga(
  action: any,
): Generator<any, any, any> {
  yield put(actions.sendEmailAddressConfirmationEmailStart());
  try {
    const response = yield axios.post(action.id + "/emailaddressconfirmation", {
      id: action.id,
    });
    yield put(actions.sendEmailAddressConfirmationEmailSuccess());
  } catch (error) {
    yield put(actions.sendEmailAddressConfirmationEmailFail(error));
  }
}

export function* watchUser(): Generator<any, any, any> {
  yield takeEvery(actionTypes.FETCH_USERS, fetchUsersSaga);
  yield takeLatest(actionTypes.FETCH_USER, fetchUserSaga);
  yield takeEvery(actionTypes.SAVE_USER, saveUserSaga);
  yield takeEvery(actionTypes.DELETE_USER, deleteUserSaga);
  yield takeEvery(actionTypes.FETCH_USER_AUDIT_LOGS, fetchAuditLogsSaga);
  yield takeEvery(actionTypes.SET_PASSWORD, setPasswordSaga);
  yield takeEvery(
    actionTypes.SEND_PASSWORD_RESET_EMAIL,
    sendPasswordResetEmailSaga,
  );
  yield takeEvery(
    actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL,
    sendEmailAddressConfirmationEmailSaga,
  );
}
