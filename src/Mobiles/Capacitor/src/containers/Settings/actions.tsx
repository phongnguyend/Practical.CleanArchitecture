import * as actionTypes from "./actionTypes";

/// CONFIGURATION_ENTRIES
export const fetchConfigurationEntriesSuccess = (configurationEntries: any) => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRIES_SUCCESS,
    configurationEntries: configurationEntries,
  };
};

export const fetchConfigurationEntriesFail = (error: any) => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRIES_FAIL,
    error: error,
  };
};

export const fetchConfigurationEntriesStart = () => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRIES_START,
  };
};

export const fetchConfigurationEntries = () => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRIES,
  };
};
/// CONFIGURATION_ENTRIES

/// CONFIGURATION_ENTRY
export const fetchConfigurationEntrySuccess = (configurationEntry: any) => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRY_SUCCESS,
    configurationEntry: configurationEntry,
  };
};

export const fetchConfigurationEntryFail = (error: any) => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRY_FAIL,
    error: error,
  };
};

export const fetchConfigurationEntryStart = () => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRY_START,
  };
};

export const fetchConfigurationEntry = (id: any) => {
  return {
    type: actionTypes.FETCH_CONFIGURATION_ENTRY,
    id: id,
  };
};
/// CONFIGURATION_ENTRY

/// UPDATE CONFIGURATION_ENTRY
export const updateConfigurationEntry = (configurationEntry: any) => {
  return {
    type: actionTypes.UPDATE_CONFIGURATION_ENTRY,
    configurationEntry: configurationEntry,
  };
};

export const resetConfigurationEntry = () => {
  return {
    type: actionTypes.RESET_CONFIGURATION_ENTRY,
  };
};
/// UPDATE CONFIGURATION_ENTRY

/// SAVE CONFIGURATION_ENTRY
export const saveConfigurationEntrySuccess = (configurationEntry: any) => {
  return {
    type: actionTypes.SAVE_CONFIGURATION_ENTRY_SUCCESS,
    configurationEntry: configurationEntry,
  };
};

export const saveConfigurationEntryFail = (error: any) => {
  return {
    type: actionTypes.SAVE_CONFIGURATION_ENTRY_FAIL,
    error: error,
  };
};

export const saveConfigurationEntryStart = () => {
  return {
    type: actionTypes.SAVE_CONFIGURATION_ENTRY_START,
  };
};

export const saveConfigurationEntry = (configurationEntry: any) => {
  return {
    type: actionTypes.SAVE_CONFIGURATION_ENTRY,
    configurationEntry: configurationEntry,
  };
};
/// SAVE CONFIGURATION_ENTRY

/// DELETE CONFIGURATION_ENTRY
export const deleteConfigurationEntrySuccess = (configurationEntry: any) => {
  return {
    type: actionTypes.DELETE_CONFIGURATION_ENTRY_SUCCESS,
  };
};

export const deleteConfigurationEntryFail = (error: any) => {
  return {
    type: actionTypes.DELETE_CONFIGURATION_ENTRY_FAIL,
    error: error,
  };
};

export const deleteConfigurationEntryStart = () => {
  return {
    type: actionTypes.DELETE_CONFIGURATION_ENTRY_START,
  };
};

export const deleteConfigurationEntry = (configurationEntry: any) => {
  return {
    type: actionTypes.DELETE_CONFIGURATION_ENTRY,
    configurationEntry: configurationEntry,
  };
};
/// DELETE CONFIGURATION_ENTRY
