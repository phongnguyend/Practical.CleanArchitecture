import { updateObject } from "../../shared/utility";
import * as actionTypes from "./actionTypes";

const initialState = {
  auditLogs: [],
  totalItems: 0,
  loading: false,
  error: null,
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case actionTypes.FETCH_AUDIT_LOGS_START:
      return updateObject(state, {
        loading: true,
      });
    case actionTypes.FETCH_AUDIT_LOGS_SUCCESS:
      return updateObject(state, {
        auditLogs: action.auditLogs,
        totalItems: action.totalItems,
        loading: false,
      });
    case actionTypes.FETCH_AUDIT_LOGS_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
      });
    default:
      return state;
  }
};

export default reducer;
