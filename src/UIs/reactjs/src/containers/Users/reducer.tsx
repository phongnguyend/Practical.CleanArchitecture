import { updateObject } from "../../shared/utility";
import * as actionTypes from "./actionTypes";

const initialState = {
  users: [],
  user: {
    userName: "",
    email: "",
    emailConfirmed: false,
    phoneNumber: "",
    phoneNumberConfirmed: false,
    twoFactorEnabled: false,
    lockoutEnabled: false,
    accessFailedCount: 0,
    lockoutEnd: "",
  },
  auditLogs: [],
  loading: false,
  saved: false,
  deleted: false,
  savedPassword: false,
  sentPasswordResetEmail: false,
  sentEmailAddressConfirmationEmail: false,
  error: null,
};

/// Users
const fetchUsersStart = (state, action) => {
  return updateObject(state, { loading: true });
};

const fetchUsersSuccess = (state, action) => {
  return updateObject(state, {
    users: action.users,
    loading: false,
  });
};

const fetchUsersFail = (state, action) => {
  return updateObject(state, { loading: false });
};

/// Users

/// User
const fetchUserStart = (state, action) => {
  return updateObject(state, { loading: true });
};

const fetchUserSuccess = (state, action) => {
  return updateObject(state, {
    user: action.user,
    loading: false,
  });
};

const fetchUserFail = (state, action) => {
  return updateObject(state, { loading: false });
};

/// User

const saveUserStart = (state, action) => {
  return updateObject(state, { loading: true, saved: false });
};

const saveUserSuccess = (state, action) => {
  return updateObject(state, {
    user: action.user,
    loading: false,
    saved: true,
  });
};

const saveUserFail = (state, action) => {
  return updateObject(state, { loading: false, saved: false });
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case actionTypes.FETCH_USERS_START:
      return fetchUsersStart(state, action);
    case actionTypes.FETCH_USERS_SUCCESS:
      return fetchUsersSuccess(state, action);
    case actionTypes.FETCH_USERS_FAIL:
    case actionTypes.FETCH_USER_START:
      return fetchUserStart(state, action);
    case actionTypes.FETCH_USER_SUCCESS:
      return fetchUserSuccess(state, action);
    case actionTypes.FETCH_USER_FAIL:
      return fetchUserFail(state, action);
    case actionTypes.UPDATE_USER:
      return updateObject(state, { user: action.user });
    case actionTypes.RESET_USER:
      return updateObject(state, initialState);
    case actionTypes.SAVE_USER_START:
      return saveUserStart(state, action);
    case actionTypes.SAVE_USER_SUCCESS:
      return saveUserSuccess(state, action);
    case actionTypes.SAVE_USER_FAIL:
      return saveUserFail(state, action);
    case actionTypes.DELETE_USER_START:
      return updateObject(state, {
        user: action.user,
        loading: true,
        deleted: false,
      });
    case actionTypes.DELETE_USER_SUCCESS:
      return updateObject(state, {
        user: action.user,
        loading: false,
        deleted: true,
      });
    case actionTypes.DELETE_USER_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
        deleted: false,
      });
    case actionTypes.FETCH_USER_AUDIT_LOGS_START:
      return updateObject(state, {
        user: action.user,
        loading: true,
      });
    case actionTypes.FETCH_USER_AUDIT_LOGS_SUCCESS:
      return updateObject(state, {
        auditLogs: action.auditLogs,
        loading: false,
        error: null,
      });
    case actionTypes.FETCH_USER_AUDIT_LOGS_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
      });
    case actionTypes.SET_PASSWORD_INIT:
      return updateObject(state, {
        loading: false,
        error: null,
        savedPassword: false,
      });
    case actionTypes.SET_PASSWORD_START:
      return updateObject(state, {
        loading: true,
        error: null,
        savedPassword: false,
      });
    case actionTypes.SET_PASSWORD_SUCCESS:
      return updateObject(state, {
        loading: false,
        error: null,
        savedPassword: true,
      });
    case actionTypes.SET_PASSWORD_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
        savedPassword: false,
      });
    case actionTypes.SEND_PASSWORD_RESET_EMAIL_INIT:
      return updateObject(state, {
        loading: false,
        error: null,
        sentPasswordResetEmail: false,
      });
    case actionTypes.SEND_PASSWORD_RESET_EMAIL_START:
      return updateObject(state, {
        loading: true,
        error: null,
        sentPasswordResetEmail: false,
      });
    case actionTypes.SEND_PASSWORD_RESET_EMAIL_SUCCESS:
      return updateObject(state, {
        loading: false,
        error: null,
        sentPasswordResetEmail: true,
      });
    case actionTypes.SEND_PASSWORD_RESET_EMAIL_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
        sentPasswordResetEmail: false,
      });
    case actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_INIT:
      return updateObject(state, {
        loading: false,
        error: null,
        sentEmailAddressConfirmationEmail: false,
      });
    case actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_START:
      return updateObject(state, {
        loading: true,
        error: null,
        sentEmailAddressConfirmationEmail: false,
      });
    case actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_SUCCESS:
      return updateObject(state, {
        loading: false,
        error: null,
        sentEmailAddressConfirmationEmail: true,
      });
    case actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
        sentEmailAddressConfirmationEmail: false,
      });
    default:
      return state;
  }
};

export default reducer;
