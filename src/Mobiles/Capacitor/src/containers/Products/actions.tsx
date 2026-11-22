import * as actionTypes from "./actionTypes";

/// PRODUCTS
export const fetchProductsSuccess = (products: any) => {
  return {
    type: actionTypes.FETCH_PRODUCTS_SUCCESS,
    products: products,
  };
};

export const fetchProductsFail = (error: any) => {
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
export const fetchProductSuccess = (product: any) => {
  return {
    type: actionTypes.FETCH_PRODUCT_SUCCESS,
    product: product,
  };
};

export const fetchProductFail = (error: any) => {
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

export const fetchProduct = (id: any) => {
  return {
    type: actionTypes.FETCH_PRODUCT,
    id: id,
  };
};
/// PRODUCT

/// UPDATE PRODUCT
export const updateProduct = (product: any) => {
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
export const saveProductSuccess = (product: any) => {
  return {
    type: actionTypes.SAVE_PRODUCT_SUCCESS,
    product: product,
  };
};

export const saveProductFail = (error: any) => {
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

export const saveProduct = (product: any) => {
  return {
    type: actionTypes.SAVE_PRODUCT,
    product: product,
  };
};
/// SAVE PRODUCT

/// DELETE PRODUCT
export const deleteProductSuccess = (product: any) => {
  return {
    type: actionTypes.DELETE_PRODUCT_SUCCESS,
  };
};

export const deleteProductFail = (error: any) => {
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

export const deleteProduct = (product: any) => {
  return {
    type: actionTypes.DELETE_PRODUCT,
    product: product,
  };
};
/// DELETE PRODUCT

/// VIEW AUDIT LOGS
export const fetchAuditLogsSuccess = (auditLogs: any) => {
  return {
    type: actionTypes.FETCH_PRODUCT_AUDIT_LOGS_SUCCESS,
    auditLogs: auditLogs,
  };
};

export const fetchAuditLogsFail = (error: any) => {
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

export const fetchAuditLogs = (product: any) => {
  return {
    type: actionTypes.FETCH_PRODUCT_AUDIT_LOGS,
    product: product,
  };
};
/// VIEW AUDIT LOGS
