import { useState, useEffect } from "react";
import { Navigate, NavLink, useParams } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";

const AddUser = () => {
  const [state, setState] = useState({
    title: "Add User",
    controls: {
      userName: {
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
      email: {
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
    },
    valid: false,
    submitted: false,
    errorMessage: null,
  });

  const { id } = useParams();
  const { user, saved } = useSelector((state: any) => state.user);
  const dispatch = useDispatch();
  const fetchUser = (id) => dispatch(actions.fetchUser(id));
  const updateUser = (user) => dispatch(actions.updateUser(user));
  const resetUser = () => dispatch(actions.resetUser());
  const saveUser = (user) => dispatch(actions.saveUser(user));

  useEffect(() => {
    resetUser();
    if (id) {
      fetchUser(id);
      setState({ ...state, title: "Edit User" });
    }
  }, []);

  const fieldChanged = (event) => {
    let value = event.target.value;

    if (event.target.type === "checkbox") {
      value = event.target.checked;
    }

    checkFieldValidity(event.target.name, value);

    updateUser({
      ...user,
      [event.target.name]: value,
    });
  };

  const updateLockoutEnd = (dateTime) => {
    updateUser({
      ...user,
      lockoutEnd: dateTime ? dateTime.toISOString() : "",
    });
  };

  const checkFieldValidity = (name, value) => {
    const control = state.controls[name];

    if (!control) return true;

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
      isValid = checkFieldValidity(fieldName, user[fieldName]) && isValid;
    }

    if (isValid) {
      saveUser({
        ...user,
        accessFailedCount: user.accessFailedCount ? parseInt(user.accessFailedCount) : 0,
        lockoutEnd: user.lockoutEnd ? user.lockoutEnd : null,
      });
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
            <label htmlFor="userName" className="col-sm-3 col-form-label">
              User Name
            </label>
            <div className="col-sm-9">
              <input
                id="userName"
                name="userName"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["userName"].valid ? "is-invalid" : "")
                }
                value={user?.userName}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["userName"].error.required ? <span>Enter an user name</span> : null}
                {state.controls["userName"].error.minLength ? (
                  <span>The user name must be longer than 3 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="email" className="col-sm-3 col-form-label">
              Email
            </label>
            <div className="col-sm-9">
              <input
                id="email"
                name="email"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["email"].valid ? "is-invalid" : "")
                }
                value={user?.email}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["email"].error.required ? <span>Enter an email</span> : null}
                {state.controls["email"].error.minLength ? (
                  <span>The email must be longer than 3 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="emailConfirmed" className="col-sm-3 col-form-label">
              Email Confirmed
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="emailConfirmed"
                name="emailConfirmed"
                checked={user?.emailConfirmed}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="phoneNumber" className="col-sm-3 col-form-label">
              Phone Number
            </label>
            <div className="col-sm-9">
              <input
                id="phoneNumber"
                name="phoneNumber"
                className="form-control"
                value={user?.phoneNumber}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="phoneNumberConfirmed" className="col-sm-3 col-form-label">
              Phone Number Confirmed
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="phoneNumberConfirmed"
                name="phoneNumberConfirmed"
                checked={user?.phoneNumberConfirmed}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="twoFactorEnabled" className="col-sm-3 col-form-label">
              Two Factor Enabled
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="twoFactorEnabled"
                name="twoFactorEnabled"
                checked={user?.twoFactorEnabled}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="lockoutEnabled" className="col-sm-3 col-form-label">
              Lockout Enabled
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="lockoutEnabled"
                name="lockoutEnabled"
                checked={user?.lockoutEnabled}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="accessFailedCount" className="col-sm-3 col-form-label">
              Access Failed Count
            </label>
            <div className="col-sm-9">
              <input
                type="number"
                id="accessFailedCount"
                name="accessFailedCount"
                className="form-control"
                value={user?.accessFailedCount}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="lockoutEnd" className="col-sm-3 col-form-label">
              Lockout End
            </label>
            <div className="col-sm-9">
              <DatePicker
                id="lockoutEnd"
                name="lockoutEnd"
                className="form-control"
                autoComplete="off"
                selected={user?.lockoutEnd ? new Date(user?.lockoutEnd) : null}
                onChange={(date) => updateLockoutEnd(date)}
                timeInputLabel="Time:"
                dateFormat="MM/dd/yyyy h:mm aa"
                showTimeInput
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="description" className="col-sm-3 col-form-label"></label>
            <div className="col-sm-9">
              <button className="btn btn-primary">Save</button>
            </div>
          </div>
        </form>
      </div>
      <div className="card-footer">
        <NavLink className="btn btn-outline-secondary" to="/users" style={{ width: "80px" }}>
          <i className="fa fa-chevron-left"></i> Back
        </NavLink>
      </div>
    </div>
  );

  return state.submitted && saved ? <Navigate to={"/users/" + user.id} /> : form;
};

export default AddUser;
