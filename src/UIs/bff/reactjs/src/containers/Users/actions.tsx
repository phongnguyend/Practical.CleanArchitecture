import * as actionTypes from "./actionTypes";

/// USERS
export const fetchUsersSuccess = (users) => {
  return {
    type: actionTypes.FETCH_USERS_SUCCESS,
    users: users,
  };
};

export const fetchUsersFail = (error) => {
  return {
    type: actionTypes.FETCH_USERS_FAIL,
    error: error,
  };
};

export const fetchUsersStart = () => {
  return {
    type: actionTypes.FETCH_USERS_START,
  };
};

export const fetchUsers = () => {
  return {
    type: actionTypes.FETCH_USERS,
  };
};
/// USERS

/// USER
export const fetchUserSuccess = (user) => {
  return {
    type: actionTypes.FETCH_USER_SUCCESS,
    user: user,
  };
};

export const fetchUserFail = (error) => {
  return {
    type: actionTypes.FETCH_USER_FAIL,
    error: error,
  };
};

export const fetchUserStart = () => {
  return {
    type: actionTypes.FETCH_USER_START,
  };
};

export const fetchUser = (id) => {
  return {
    type: actionTypes.FETCH_USER,
    id: id,
  };
};
/// USER

/// UPDATE USER
export const updateUser = (user) => {
  return {
    type: actionTypes.UPDATE_USER,
    user: user,
  };
};

export const resetUser = () => {
  return {
    type: actionTypes.RESET_USER,
  };
};
/// UPDATE USER

/// SAVE USER
export const saveUserSuccess = (user) => {
  return {
    type: actionTypes.SAVE_USER_SUCCESS,
    user: user,
  };
};

export const saveUserFail = (error) => {
  return {
    type: actionTypes.SAVE_USER_FAIL,
    error: error,
  };
};

export const saveUserStart = () => {
  return {
    type: actionTypes.SAVE_USER_START,
  };
};

export const saveUser = (user) => {
  return {
    type: actionTypes.SAVE_USER,
    user: user,
  };
};
/// SAVE USER

/// DELETE USER
export const deleteUserSuccess = (user) => {
  return {
    type: actionTypes.DELETE_USER_SUCCESS,
  };
};

export const deleteUserFail = (error) => {
  return {
    type: actionTypes.DELETE_USER_FAIL,
    error: error,
  };
};

export const deleteUserStart = () => {
  return {
    type: actionTypes.DELETE_USER_START,
  };
};

export const deleteUser = (user) => {
  return {
    type: actionTypes.DELETE_USER,
    user: user,
  };
};
/// DELETE USER

/// VIEW AUDIT LOGS
export const fetchAuditLogsSuccess = (auditLogs) => {
  return {
    type: actionTypes.FETCH_USER_AUDIT_LOGS_SUCCESS,
    auditLogs: auditLogs,
  };
};

export const fetchAuditLogsFail = (error) => {
  return {
    type: actionTypes.FETCH_USER_AUDIT_LOGS_FAIL,
    error: error,
  };
};

export const fetchAuditLogsStart = () => {
  return {
    type: actionTypes.FETCH_USER_AUDIT_LOGS_START,
  };
};

export const fetchAuditLogs = (user) => {
  return {
    type: actionTypes.FETCH_USER_AUDIT_LOGS,
    user: user,
  };
};
/// VIEW AUDIT LOGS

/// SET PASSWORD
export const setPasswordSuccess = () => {
  return {
    type: actionTypes.SET_PASSWORD_SUCCESS,
  };
};

export const setPasswordFail = (error) => {
  return {
    type: actionTypes.SET_PASSWORD_FAIL,
    error: error,
  };
};

export const setPasswordStart = () => {
  return {
    type: actionTypes.SET_PASSWORD_START,
  };
};

export const setPasswordInit = () => {
  return {
    type: actionTypes.SET_PASSWORD_INIT,
  };
};

export const setPassword = (password) => {
  return {
    type: actionTypes.SET_PASSWORD,
    password: password,
  };
};
/// SET PASSWORD

/// SEND PASSWORD RESET EMAIL

export const sendPasswordResetEmail = (id) => {
  return {
    type: actionTypes.SEND_PASSWORD_RESET_EMAIL,
    id: id,
  };
};

export const sendPasswordResetEmailInit = () => {
  return {
    type: actionTypes.SEND_PASSWORD_RESET_EMAIL_INIT,
  };
};

export const sendPasswordResetEmailStart = () => {
  return {
    type: actionTypes.SEND_PASSWORD_RESET_EMAIL_START,
  };
};

export const sendPasswordResetEmailSuccess = () => {
  return {
    type: actionTypes.SEND_PASSWORD_RESET_EMAIL_SUCCESS,
  };
};

export const sendPasswordResetEmailFail = (error) => {
  return {
    type: actionTypes.SEND_PASSWORD_RESET_EMAIL_FAIL,
    error: error,
  };
};

/// SEND PASSWORD RESET EMAIL

/// SEND EMAIL ADDRESS CONFIRMATION EMAIL

export const sendEmailAddressConfirmationEmail = (id) => {
  return {
    type: actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL,
    id: id,
  };
};

export const sendEmailAddressConfirmationEmailInit = () => {
  return {
    type: actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_INIT,
  };
};

export const sendEmailAddressConfirmationEmailStart = () => {
  return {
    type: actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_START,
  };
};

export const sendEmailAddressConfirmationEmailSuccess = () => {
  return {
    type: actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_SUCCESS,
  };
};

export const sendEmailAddressConfirmationEmailFail = (error) => {
  return {
    type: actionTypes.SEND_EMAIL_ADDRESS_CONFIRMATION_EMAIL_FAIL,
    error: error,
  };
};

/// SEND EMAIL ADDRESS CONFIRMATION EMAIL
