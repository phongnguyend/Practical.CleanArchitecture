import { updateObject } from "../../shared/utility";
import * as actionTypes from "./actionTypes";

const initialState = {
  user: null,
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case actionTypes.LOGIN:
      return updateObject(state, {
        user: action.user,
      });
    case actionTypes.LOGOUT:
      return updateObject(state, { user: initialState.user });
    default:
      return state;
  }
};

export default reducer;
