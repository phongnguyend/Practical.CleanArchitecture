import { updateObject } from "../../shared/utility";
import * as actionTypes from "./actionTypes";

const initialState = {
  configurationEntries: [],
  configurationEntry: {
    key: "",
    value: "",
    description: "",
  },
  loading: false,
  saved: false,
  deleted: false,
  error: null,
};

/// ConfigurationEntries
const fetchConfigurationEntriesStart = (state, action) => {
  return updateObject(state, { loading: true });
};

const fetchConfigurationEntriesSuccess = (state, action) => {
  return updateObject(state, {
    configurationEntries: action.configurationEntries,
    loading: false,
    saved: false,
  });
};

const fetchConfigurationEntriesFail = (state, action) => {
  return updateObject(state, { loading: false });
};

/// ConfigurationEntries

/// ConfigurationEntry
const fetchConfigurationEntryStart = (state, action) => {
  return updateObject(state, { loading: true });
};

const fetchConfigurationEntrySuccess = (state, action) => {
  return updateObject(state, {
    configurationEntry: action.configurationEntry,
    loading: false,
    saved: false,
  });
};

const fetchConfigurationEntryFail = (state, action) => {
  return updateObject(state, { loading: false });
};

/// ConfigurationEntry

const saveConfigurationEntryStart = (state, action) => {
  return updateObject(state, { loading: true, saved: false });
};

const saveConfigurationEntrySuccess = (state, action) => {
  return updateObject(state, {
    configurationEntry: action.configurationEntry,
    loading: false,
    saved: true,
  });
};

const saveConfigurationEntryFail = (state, action) => {
  return updateObject(state, { loading: false, saved: false });
};

const reducer = (state = initialState, action) => {
  switch (action.type) {
    case actionTypes.FETCH_CONFIGURATION_ENTRIES_START:
      return fetchConfigurationEntriesStart(state, action);
    case actionTypes.FETCH_CONFIGURATION_ENTRIES_SUCCESS:
      return fetchConfigurationEntriesSuccess(state, action);
    case actionTypes.FETCH_CONFIGURATION_ENTRIES_FAIL:
    case actionTypes.FETCH_CONFIGURATION_ENTRY_START:
      return fetchConfigurationEntryStart(state, action);
    case actionTypes.FETCH_CONFIGURATION_ENTRY_SUCCESS:
      return fetchConfigurationEntrySuccess(state, action);
    case actionTypes.FETCH_CONFIGURATION_ENTRY_FAIL:
      return fetchConfigurationEntryFail(state, action);
    case actionTypes.UPDATE_CONFIGURATION_ENTRY:
      return updateObject(state, {
        configurationEntry: action.configurationEntry,
      });
    case actionTypes.RESET_CONFIGURATION_ENTRY:
      return updateObject(state, initialState);
    case actionTypes.SAVE_CONFIGURATION_ENTRY_START:
      return saveConfigurationEntryStart(state, action);
    case actionTypes.SAVE_CONFIGURATION_ENTRY_SUCCESS:
      return saveConfigurationEntrySuccess(state, action);
    case actionTypes.SAVE_CONFIGURATION_ENTRY_FAIL:
      return saveConfigurationEntryFail(state, action);
    case actionTypes.DELETE_CONFIGURATION_ENTRY_START:
      return updateObject(state, {
        configurationEntry: action.configurationEntry,
        loading: true,
        deleted: false,
      });
    case actionTypes.DELETE_CONFIGURATION_ENTRY_SUCCESS:
      return updateObject(state, {
        configurationEntry: initialState.configurationEntry,
        loading: false,
        deleted: true,
      });
    case actionTypes.DELETE_CONFIGURATION_ENTRY_FAIL:
      return updateObject(state, {
        error: action.error,
        loading: false,
        deleted: false,
      });
    default:
      return state;
  }
};

export default reducer;
