import React, { Component } from "react";
import { NavLink, Redirect } from "react-router-dom";
import { connect } from "react-redux";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";

class AddUser extends Component<any, any> {
  state = {
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
  };

  componentDidMount() {
    this.props.resetUser();
    const id = this.props.match?.params?.id;
    if (id) {
      this.setState({ title: "Edit User" });
      this.props.fetchUser(id);
    }
  }

  fieldChanged = (event) => {
    let value = event.target.value;

    if (event.target.type == "checkbox") {
      value = event.target.checked;
    }

    const user = {
      ...this.props.user,
      [event.target.name]: value,
    };

    this.checkFieldValidity(event.target.name, value);

    this.props.updateUser(user);
  };

  updateLockoutEnd = (dateTime) => {
    const user = {
      ...this.props.user,
      lockoutEnd: dateTime ? dateTime.toISOString() : "",
    };

    this.props.updateUser(user);
  };

  checkFieldValidity = (name, value) => {
    const control = this.state.controls[name];

    if (!control) return true;

    const rules = control.validation;
    const validationRs = checkValidity(value, rules);

    this.setState((preState) => {
      return {
        controls: {
          ...preState.controls,
          [name]: {
            ...preState.controls[name],
            error: validationRs,
            valid: validationRs.isValid,
          },
        },
      };
    });

    return validationRs.isValid;
  };

  onSubmit = (event) => {
    event.preventDefault();
    this.setState({ submitted: true });

    let isValid = true;
    for (let fieldName in this.state.controls) {
      isValid =
        this.checkFieldValidity(fieldName, this.props.user[fieldName]) &&
        isValid;
    }

    if (isValid) {
      const user = {
        ...this.props.user,
        accessFailedCount: this.props.user.accessFailedCount
          ? parseInt(this.props.user.accessFailedCount)
          : 0,
        lockoutEnd: this.props.user.lockoutEnd
          ? this.props.user.lockoutEnd
          : null,
      };
      this.props.saveUser(user);
    }
  };

  render() {
    const form = (
      <div className="card">
        <div className="card-header">{this.state.title}</div>
        <div className="card-body">
          {this.state.errorMessage ? (
            <div
              className="row alert alert-danger"
            >
              {this.state.errorMessage}
            </div>
          ) : null}
          <form onSubmit={this.onSubmit}>
            <div className="form-group row">
              <label htmlFor="userName" className="col-sm-3 col-form-label">
                User Name
              </label>
              <div className="col-sm-9">
                <input
                  id="userName"
                  name="userName"
                  className={
                    "form-control " +
                    (this.state.submitted &&
                    !this.state.controls["userName"].valid
                      ? "is-invalid"
                      : "")
                  }
                  value={this.props.user?.userName}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  {this.state.controls["userName"].error.required ? (
                    <span>Enter an user name</span>
                  ) : null}
                  {this.state.controls["userName"].error.minLength ? (
                    <span>The user name must be longer than 3 characters.</span>
                  ) : null}
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="email" className="col-sm-3 col-form-label">
                Email
              </label>
              <div className="col-sm-9">
                <input
                  id="email"
                  name="email"
                  className={
                    "form-control " +
                    (this.state.submitted && !this.state.controls["email"].valid
                      ? "is-invalid"
                      : "")
                  }
                  value={this.props.user?.email}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  {this.state.controls["email"].error.required ? (
                    <span>Enter an email</span>
                  ) : null}
                  {this.state.controls["email"].error.minLength ? (
                    <span>The email must be longer than 3 characters.</span>
                  ) : null}
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="emailConfirmed"
                className="col-sm-3 col-form-label"
              >
                Email Confirmed
              </label>
              <div className="col-sm-9">
                <input
                  type="checkbox"
                  id="emailConfirmed"
                  name="emailConfirmed"
                  checked={this.props.user?.emailConfirmed}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="phoneNumber" className="col-sm-3 col-form-label">
                Phone Number
              </label>
              <div className="col-sm-9">
                <input
                  id="phoneNumber"
                  name="phoneNumber"
                  className="form-control"
                  value={this.props.user?.phoneNumber}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="phoneNumberConfirmed"
                className="col-sm-3 col-form-label"
              >
                Phone Number Confirmed
              </label>
              <div className="col-sm-9">
                <input
                  type="checkbox"
                  id="phoneNumberConfirmed"
                  name="phoneNumberConfirmed"
                  checked={this.props.user?.phoneNumberConfirmed}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="twoFactorEnabled"
                className="col-sm-3 col-form-label"
              >
                Two Factor Enabled
              </label>
              <div className="col-sm-9">
                <input
                  type="checkbox"
                  id="twoFactorEnabled"
                  name="twoFactorEnabled"
                  checked={this.props.user?.twoFactorEnabled}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="lockoutEnabled"
                className="col-sm-3 col-form-label"
              >
                Lockout Enabled
              </label>
              <div className="col-sm-9">
                <input
                  type="checkbox"
                  id="lockoutEnabled"
                  name="lockoutEnabled"
                  checked={this.props.user?.lockoutEnabled}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="accessFailedCount"
                className="col-sm-3 col-form-label"
              >
                Access Failed Count
              </label>
              <div className="col-sm-9">
                <input
                  type="number"
                  id="accessFailedCount"
                  name="accessFailedCount"
                  className="form-control"
                  value={this.props.user?.accessFailedCount}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="lockoutEnd" className="col-sm-3 col-form-label">
                Lockout End
              </label>
              <div className="col-sm-9">
                <DatePicker
                  id="lockoutEnd"
                  name="lockoutEnd"
                  className="form-control"
                  autoComplete="off"
                  selected={
                    this.props.user?.lockoutEnd
                      ? new Date(this.props.user?.lockoutEnd)
                      : null
                  }
                  onChange={(date) => this.updateLockoutEnd(date)}
                  timeInputLabel="Time:"
                  dateFormat="MM/dd/yyyy h:mm aa"
                  showTimeInput
                />
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="description"
                className="col-sm-3 col-form-label"
              ></label>
              <div className="col-sm-9">
                <button className="btn btn-primary">Save</button>
              </div>
            </div>
          </form>
        </div>
        <div className="card-footer">
          <NavLink
            className="btn btn-outline-secondary"
            to="/users"
            style={{ width: "80px" }}
          >
            <i className="fa fa-chevron-left"></i> Back
          </NavLink>
        </div>
      </div>
    );

    return this.state.submitted && this.props.saved ? (
      <Redirect to={"/users/" + this.props.user.id} />
    ) : (
      form
    );
  }
}

const mapStateToProps = (state) => {
  return {
    user: state.user.user,
    saved: state.user.saved,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchUser: (id) => dispatch(actions.fetchUser(id)),
    updateUser: (user) => dispatch(actions.updateUser(user)),
    resetUser: () => dispatch(actions.resetUser()),
    saveUser: (user) => dispatch(actions.saveUser(user)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(AddUser);
