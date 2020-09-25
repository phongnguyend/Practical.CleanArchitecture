import * as actionTypes from "./actionTypes";

/// PRODUCTS
export const fetchProductsSuccess = (products) => {
  return {
    type: actionTypes.FETCH_PRODUCTS_SUCCESS,
    products: products,
  };
};

export const fetchProductsFail = (error) => {
  return {
    type: actionTypes.FETCH_PRODUCTS_FAIL,
    error: error,
  };
};

export const fetchProductsStart = () => {
  return {
    type: actionTypes.FETCH_PRODUCTS_START,
  };
};

export const fetchProducts = () => {
  return {
    type: actionTypes.FETCH_PRODUCTS,
  };
};
/// PRODUCTS

/// PRODUCT
export const fetchProductSuccess = (product) => {
  return {
    type: actionTypes.FETCH_PRODUCT_SUCCESS,
    product: product,
  };
};

export const fetchProductFail = (error) => {
  return {
    type: actionTypes.FETCH_PRODUCT_FAIL,
    error: error,
  };
};

export const fetchProductStart = () => {
  return {
    type: actionTypes.FETCH_PRODUCT_START,
  };
};

export const fetchProduct = (id) => {
  return {
    type: actionTypes.FETCH_PRODUCT,
    id: id,
  };
};
/// PRODUCT

/// UPDATE PRODUCT
export const updateProduct = (product) => {
  return {
    type: actionTypes.UPDATE_PRODUCT,
    product: product,
  };
};

export const resetProduct = () => {
  return {
    type: actionTypes.RESET_PRODUCT,
  };
};
/// UPDATE PRODUCT

/// SAVE PRODUCT
export const saveProductSuccess = (product) => {
  return {
    type: actionTypes.SAVE_PRODUCT_SUCCESS,
    product: product,
  };
};

export const saveProductFail = (error) => {
  return {
    type: actionTypes.SAVE_PRODUCT_FAIL,
    error: error,
  };
};

export const saveProductStart = () => {
  return {
    type: actionTypes.SAVE_PRODUCT_START,
  };
};

export const saveProduct = (product) => {
  return {
    type: actionTypes.SAVE_PRODUCT,
    product: product,
  };
};
/// SAVE PRODUCT

/// DELETE PRODUCT
export const deleteProductSuccess = (product) => {
  return {
    type: actionTypes.DELETE_PRODUCT_SUCCESS,
  };
};

export const deleteProductFail = (error) => {
  return {
    type: actionTypes.DELETE_PRODUCT_FAIL,
    error: error,
  };
};

export const deleteProductStart = () => {
  return {
    type: actionTypes.DELETE_PRODUCT_START,
  };
};

export const deleteProduct = (product) => {
  return {
    type: actionTypes.DELETE_PRODUCT,
    product: product,
  };
};
/// DELETE PRODUCT

/// VIEW AUDIT LOGS
export const fetchAuditLogsSuccess = (auditLogs) => {
  return {
    type: actionTypes.FETCH_PRODUCT_AUDIT_LOGS_SUCCESS,
    auditLogs: auditLogs,
  };
};

export const fetchAuditLogsFail = (error) => {
  return {
    type: actionTypes.FETCH_PRODUCT_AUDIT_LOGS_FAIL,
    error: error,
  };
};

export const fetchAuditLogsStart = () => {
  return {
    type: actionTypes.FETCH_PRODUCT_AUDIT_LOGS_START,
  };
};

export const fetchAuditLogs = (product) => {
  return {
    type: actionTypes.FETCH_PRODUCT_AUDIT_LOGS,
    product: product,
  };
};
/// VIEW AUDIT LOGS
