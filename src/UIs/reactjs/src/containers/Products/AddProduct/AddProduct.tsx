import { useEffect, useState } from "react";
import { NavLink, Navigate, useParams } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";

const AddProduct = () => {
  const [state, setState] = useState({
    title: "Add Product",
    controls: {
      name: {
        validation: {
          required: true,
          minLength: 3,
        },
        error: {
          required: false,
          minLength: false,
        },
        valid: false,
        touched: false,
      },
      code: {
        validation: {
          required: true,
          maxLength: 10,
        },
        error: {
          required: false,
          maxLength: false,
        },
        valid: false,
        touched: false,
      },
      description: {
        validation: {
          required: true,
          maxLength: 100,
        },
        error: {
          required: false,
          maxLength: false,
        },
        valid: false,
        touched: false,
      },
    },
    valid: false,
    submitted: false,
    errorMessage: null,
  });

  const { id } = useParams();
  const { product, saved } = useSelector((state: any) => state.product);
  const dispatch = useDispatch();
  const fetchProduct = (id) => dispatch(actions.fetchProduct(id));
  const updateProduct = (product) => dispatch(actions.updateProduct(product));
  const resetProduct = () => dispatch(actions.resetProduct());
  const saveProduct = (product) => dispatch(actions.saveProduct(product));

  useEffect(() => {
    resetProduct();
    if (id) {
      setState({ ...state, title: "Edit Product" });
      fetchProduct(id);
    }
  }, []);

  const fieldChanged = (event) => {
    checkFieldValidity(event.target.name, event.target.value);

    updateProduct({
      ...product,
      [event.target.name]: event.target.value,
    });
  };

  const checkFieldValidity = (name, value) => {
    const control = state.controls[name];
    const rules = control.validation;
    const validationRs = checkValidity(value, rules);
    setState((preState) => ({
      ...preState,
      controls: {
        ...preState.controls,
        [name]: {
          ...preState.controls[name],
          error: validationRs,
          valid: validationRs.isValid,
        },
      },
    }));

    return validationRs.isValid;
  };

  const onSubmit = (event) => {
    event.preventDefault();
    setState({ ...state, submitted: true });
    let isValid = true;
    for (let fieldName in state.controls) {
      isValid = checkFieldValidity(fieldName, product[fieldName]) && isValid;
    }

    if (isValid) {
      saveProduct(product);
    }
  };

  const form = (
    <div className="card">
      <div className="card-header">{state.title}</div>
      <div className="card-body">
        {state.errorMessage ? (
          <div className="row alert alert-danger">{state.errorMessage}</div>
        ) : null}
        <form onSubmit={onSubmit}>
          <div className="mb-3 row">
            <label htmlFor="name" className="col-sm-2 col-form-label">
              Name
            </label>
            <div className="col-sm-10">
              <input
                id="name"
                name="name"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["name"].valid ? "is-invalid" : "")
                }
                value={product?.name}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["name"].error.required ? <span>Enter a name</span> : null}
                {state.controls["name"].error.minLength ? (
                  <span>The name must be longer than 3 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="code" className="col-sm-2 col-form-label">
              Code
            </label>
            <div className="col-sm-10">
              <input
                id="code"
                name="code"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["code"].valid ? "is-invalid" : "")
                }
                value={product?.code}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["code"].error.required ? <span>Enter a code</span> : null}
                {state.controls["code"].error.maxLength ? (
                  <span>The code must be less than 10 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="description" className="col-sm-2 col-form-label">
              Description
            </label>
            <div className="col-sm-10">
              <input
                id="description"
                name="description"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["description"].valid ? "is-invalid" : "")
                }
                value={product?.description}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["description"].error.required ? (
                  <span>Enter a description</span>
                ) : null}
                {state.controls["description"].error.maxLength ? (
                  <span>The code must be less than 100 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="description" className="col-sm-2 col-form-label"></label>
            <div className="col-sm-10">
              <button className="btn btn-primary">Save</button>
            </div>
          </div>
        </form>
      </div>
      <div className="card-footer">
        <NavLink className="btn btn-outline-secondary" to="/products" style={{ width: "80px" }}>
          <i className="fa fa-chevron-left"></i> Back
        </NavLink>
      </div>
    </div>
  );

  return state.submitted && saved ? <Navigate to={"/products/" + product.id} /> : form;
};

export default AddProduct;
