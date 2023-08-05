import { put, takeEvery } from "redux-saga/effects";
import axios from "./axios";

import * as actionTypes from "./actionTypes";
import * as actions from "./actions";

export function* fetchConfigurationEntriesSaga(action) {
  yield put(actions.fetchConfigurationEntriesStart());
  try {
    const response = yield axios.get("");
    const fetchedConfigurationEntries = response.data;
    yield put(
      actions.fetchConfigurationEntriesSuccess(fetchedConfigurationEntries)
    );
  } catch (error) {
    yield put(actions.fetchConfigurationEntriesFail(error));
  }
}

export function* fetchConfigurationEntrySaga(action) {
  yield put(actions.fetchConfigurationEntryStart());
  try {
    const response = yield axios.get(action.id);
    const fetchedConfigurationEntry = response.data;
    yield put(
      actions.fetchConfigurationEntrySuccess(fetchedConfigurationEntry)
    );
  } catch (error) {
    yield put(actions.fetchConfigurationEntryFail(error));
  }
}

export function* saveConfigurationEntrySaga(action) {
  yield put(actions.saveConfigurationEntryStart());
  try {
    const response = action.configurationEntry.id
      ? yield axios.put(action.configurationEntry.id, action.configurationEntry)
      : yield axios.post("", action.configurationEntry);
    const configurationEntry = response.data;
    yield put(actions.saveConfigurationEntrySuccess(configurationEntry));
    yield put(actions.fetchConfigurationEntries());
  } catch (error) {
    console.log(error);
    yield put(actions.saveConfigurationEntryFail(error));
  }
}

export function* deleteConfigurationEntrySaga(action) {
  yield put(actions.deleteConfigurationEntryStart());
  try {
    const response = yield axios.delete(
      action.configurationEntry.id,
      action.configurationEntry
    );
    yield put(
      actions.deleteConfigurationEntrySuccess(action.configurationEntry)
    );
    yield put(actions.fetchConfigurationEntries());
  } catch (error) {
    console.log(error);
    yield put(actions.deleteConfigurationEntryFail(error));
  }
}

export function* watchConfigurationEntry() {
  yield takeEvery(
    actionTypes.FETCH_CONFIGURATION_ENTRIES,
    fetchConfigurationEntriesSaga
  );
  yield takeEvery(
    actionTypes.FETCH_CONFIGURATION_ENTRY,
    fetchConfigurationEntrySaga
  );
  yield takeEvery(
    actionTypes.SAVE_CONFIGURATION_ENTRY,
    saveConfigurationEntrySaga
  );
  yield takeEvery(
    actionTypes.DELETE_CONFIGURATION_ENTRY,
    deleteConfigurationEntrySaga
  );
}
