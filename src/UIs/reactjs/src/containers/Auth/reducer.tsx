import { updateObject } from "../../shared/utility";
import * as actionTypes from "./actionTypes";

const initialState = {
  user: null,
  authService: null
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case actionTypes.SET_AUTH_SERVICE:
      return updateObject(state, {
        authService: action.authService
      });
    case actionTypes.LOGIN:
      return updateObject(state, {
        user: action.user
      });
    case actionTypes.LOGOUT:
      return updateObject(state, { user: initialState.user });
    default:
      return state;
  }
};

export default reducer;
