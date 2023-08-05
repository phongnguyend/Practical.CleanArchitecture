import { put, takeEvery } from "redux-saga/effects";
import axios from "./axios";

import * as actionTypes from "./actionTypes";
import * as actions from "./actions";

export function* fetchProductsSaga(action) {
  yield put(actions.fetchProductsStart());
  try {
    const response = yield axios.get("");
    const fetchedProducts = response.data;
    yield put(actions.fetchProductsSuccess(fetchedProducts));
  } catch (error) {
    yield put(actions.fetchProductsFail(error));
  }
}

export function* fetchProductSaga(action) {
  yield put(actions.fetchProductStart());
  try {
    const response = yield axios.get(action.id);
    const fetchedProduct = response.data;
    yield put(actions.fetchProductSuccess(fetchedProduct));
  } catch (error) {
    yield put(actions.fetchProductFail(error));
  }
}

export function* saveProductSaga(action) {
  yield put(actions.saveProductStart());
  try {
    const response = action.product.id
      ? yield axios.put(action.product.id, action.product)
      : yield axios.post("", action.product);
    const product = response.data;
    yield put(actions.saveProductSuccess(product));
  } catch (error) {
    console.log(error);
    yield put(actions.saveProductFail(error));
  }
}

export function* deleteProductSaga(action) {
  yield put(actions.deleteProductStart());
  try {
    const response = yield axios.delete(action.product.id, action.product);
    yield put(actions.deleteProductSuccess(action.product));
    yield put(actions.fetchProducts());
  } catch (error) {
    console.log(error);
    yield put(actions.deleteProductFail(error));
  }
}

export function* fetchAuditLogsSaga(action) {
  yield put(actions.fetchAuditLogsStart());
  try {
    const response = yield axios.get(action.product.id + "/auditLogs");
    const fetchedAuditLogs = response.data;
    yield put(actions.fetchAuditLogsSuccess(fetchedAuditLogs));
  } catch (error) {
    yield put(actions.fetchAuditLogsFail(error));
  }
}

export function* watchProduct() {
  yield takeEvery(actionTypes.FETCH_PRODUCTS, fetchProductsSaga);
  yield takeEvery(actionTypes.FETCH_PRODUCT, fetchProductSaga);
  yield takeEvery(actionTypes.SAVE_PRODUCT, saveProductSaga);
  yield takeEvery(actionTypes.DELETE_PRODUCT, deleteProductSaga);
  yield takeEvery(actionTypes.FETCH_PRODUCT_AUDIT_LOGS, fetchAuditLogsSaga);
}
