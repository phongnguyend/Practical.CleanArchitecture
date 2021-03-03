import React, { Component } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import logo from "../../../logo.svg";
import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";

class ViewUser extends Component<any, any> {
  state = {
    user: {
      userName: "",
      email: "",
      emailConfirmed: null,
      phoneNumber: null,
      phoneNumberConfirmed: null,
      twoFactorEnabled: null,
      lockoutEnabled: null,
      accessFailedCount: null,
      lockoutEnd: null,
    },
    showSetPasswordModal: false,
    showSendPasswordResetEmailModal: false,
    showSendEmailAddressConfirmationEmailModal: false,
    setPasswordModel: {
      id: "",
      password: "",
      confirmPassword: "",
    },
    controls: {
      password: {
        validation: {
          required: true,
        },
        error: {
          required: false,
        },
        valid: false,
        touched: false,
      },
    },
    submitted: false,
    errorMessage: null,
  };

  componentDidMount() {
    const id = this.props.match.params.id;

    this.props.fetchUser(id);

    const setPasswordModel = {
      ...this.state.setPasswordModel,
      id: id,
    };

    this.setState({ setPasswordModel: setPasswordModel });
  }

  back = () => {
    this.props.history.push("/users");
  };

  showSetPasswordModal = () => {
    this.setState({
      showSetPasswordModal: true,
      setPasswordModel: {
        ...this.state.setPasswordModel,
        password: "",
        confirmPassword: "",
      },
      submitted: false,
    });
    this.props.setPasswordInit();
  };

  cancelSetPassword = () => {
    this.setState({ showSetPasswordModal: false });
    this.props.setPasswordInit();
  };

  showSendPasswordResetEmailModal = () => {
    this.setState({
      showSendPasswordResetEmailModal: true,
    });
    this.props.sendPasswordResetEmailInit();
  };

  cancelSendPasswordResetEmail = () => {
    this.setState({ showSendPasswordResetEmailModal: false });
    this.props.sendPasswordResetEmailInit();
  };

  showSendEmailAddressConfirmationEmailModal = () => {
    this.setState({
      showSendEmailAddressConfirmationEmailModal: true,
    });
    this.props.sendEmailAddressConfirmationEmailInit();
  };

  cancelSendEmailAddressConfirmationEmail = () => {
    this.setState({ showSendEmailAddressConfirmationEmailModal: false });
    this.props.sendEmailAddressConfirmationEmailInit();
  };

  fieldChanged = (event) => {
    let value = event.target.value;
    const setPasswordModel = {
      ...this.state.setPasswordModel,
      [event.target.name]: value,
    };

    this.checkFieldValidity(event.target.name, value);

    this.setState({ setPasswordModel: setPasswordModel });
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

  confirmSetPassword = (event) => {
    event.preventDefault();
    this.setState({ submitted: true });

    let isValid = true;
    for (let fieldName in this.state.controls) {
      isValid =
        this.checkFieldValidity(
          fieldName,
          this.state.setPasswordModel[fieldName]
        ) && isValid;
    }

    isValid =
      isValid &&
      this.state.setPasswordModel.password ==
        this.state.setPasswordModel.confirmPassword;

    if (isValid) {
      this.props.setPassword(this.state.setPasswordModel);
    }
  };

  render() {
    const passwordErrors = this.props.postError?.response?.data
      ? this.props.postError?.response?.data?.map((error) => (
          <li key={error.code}>{error.description}</li>
        ))
      : null;

    const setPasswordModal = (
      <Modal
        show={this.state.showSetPasswordModal && !this.props.savedPassword}
        onHide={this.cancelSetPassword}
      >
        <Modal.Header closeButton>
          <Modal.Title>Set Password</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {passwordErrors ? (
            <div className="row alert alert-danger">
              <ul>{passwordErrors}</ul>
            </div>
          ) : null}
          {this.props.postError && !passwordErrors ? (
            <div className="row alert alert-danger">
              {this.props.postError?.response?.status}
            </div>
          ) : null}
          <form onSubmit={this.confirmSetPassword}>
            <div className="form-group row">
              <label htmlFor="userName" className="col-sm-4 col-form-label">
                User Name
              </label>
              <div className="col-sm-8">{this.props.user?.userName}</div>
            </div>
            <div className="form-group row">
              <label htmlFor="password" className="col-sm-4 col-form-label">
                Password
              </label>
              <div className="col-sm-8">
                <input
                  type="password"
                  id="password"
                  name="password"
                  className={
                    "form-control " +
                    (this.state.submitted &&
                    !this.state.controls["password"].valid
                      ? "is-invalid"
                      : "")
                  }
                  value={this.state.setPasswordModel?.password}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  {this.state.controls["password"].error.required ? (
                    <span>Enter a password</span>
                  ) : null}
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="confirmPassword"
                className="col-sm-4 col-form-label"
              >
                Confirm Password
              </label>
              <div className="col-sm-8">
                <input
                  type="password"
                  id="confirmPassword"
                  name="confirmPassword"
                  className={
                    "form-control " +
                    (this.state.submitted &&
                    this.state.setPasswordModel?.password !=
                      this.state.setPasswordModel?.confirmPassword
                      ? "is-invalid"
                      : "")
                  }
                  value={this.state.setPasswordModel?.confirmPassword}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  Confirm Password does not match
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label className="col-sm-4 col-form-label"></label>
              <div className="col-sm-8">
                <button className="btn btn-primary">Save</button>
              </div>
            </div>
          </form>
        </Modal.Body>
      </Modal>
    );

    const sendPasswordResetEmailModal = (
      <Modal
        show={
          this.state.showSendPasswordResetEmailModal &&
          !this.props.sentPasswordResetEmail
        }
        onHide={this.cancelSendPasswordResetEmail}
      >
        <Modal.Header closeButton>
          <Modal.Title>Send Password Reset Email</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to send reset password email
          <strong> {this.props.user?.userName}</strong>
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            onClick={this.cancelSendPasswordResetEmail}
          >
            No
          </Button>
          <Button
            variant="primary"
            onClick={() =>
              this.props.sendPasswordResetEmail(this.props.user.id)
            }
          >
            Yes
          </Button>
        </Modal.Footer>
      </Modal>
    );

    const sendEmailAddressConfirmationEmailModal = (
      <Modal
        show={
          this.state.showSendEmailAddressConfirmationEmailModal &&
          !this.props.sentEmailAddressConfirmationEmail
        }
        onHide={this.cancelSendEmailAddressConfirmationEmail}
      >
        <Modal.Header closeButton>
          <Modal.Title>Send Email Address Confirmation Email</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to send email address confirmation email
          <strong> {this.props.user?.userName}</strong>
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            onClick={this.cancelSendEmailAddressConfirmationEmail}
          >
            No
          </Button>
          <Button
            variant="primary"
            onClick={() =>
              this.props.sendEmailAddressConfirmationEmail(this.props.user.id)
            }
          >
            Yes
          </Button>
        </Modal.Footer>
      </Modal>
    );

    const page = this.props.user ? (
      <div className="card">
        <div className="card-header">
          {"User Detail: " + this.props.user.userName}
        </div>

        <div className="card-body">
          <div className="row">
            <div className="col-md-8">
              <div className="row">
                <div className="col-md-4">User Name:</div>
                <div className="col-md-8">{this.props.user.userName}</div>
              </div>
              <div className="row">
                <div className="col-md-4">Email:</div>
                <div className="col-md-8">{this.props.user.email}</div>
              </div>
              <div className="row">
                <div className="col-md-4">Email Confirmed:</div>
                <div className="col-md-8">
                  {this.props.user.emailConfirmed ? "true" : "false"}
                </div>
              </div>
              <div className="row">
                <div className="col-md-4">Phone Number:</div>
                <div className="col-md-8">{this.props.user.phoneNumber}</div>
              </div>
              <div className="row">
                <div className="col-md-4">Phone Number Confirmed:</div>
                <div className="col-md-8">
                  {this.props.user.phoneNumberConfirmed ? "true" : "false"}
                </div>
              </div>
              <div className="row">
                <div className="col-md-4">Two Factor Enabled:</div>
                <div className="col-md-8">
                  {this.props.user.twoFactorEnabled ? "true" : "false"}
                </div>
              </div>
              <div className="row">
                <div className="col-md-4">Lockout Enabled:</div>
                <div className="col-md-8">
                  {this.props.user.lockoutEnabled ? "true" : "false"}
                </div>
              </div>
              <div className="row">
                <div className="col-md-4">Access Failed Count:</div>
                <div className="col-md-8">
                  {this.props.user.accessFailedCount}
                </div>
              </div>
              <div className="row">
                <div className="col-md-4">Lockout End:</div>
                <div className="col-md-8">{this.props.user.lockoutEnd}</div>
              </div>
            </div>

            <div className="col-md-4">
              <img
                className="center-block img-responsive"
                style={{ width: "200px", margin: "2px" }}
                src={logo}
                title={this.props.user.userName}
              />
            </div>
          </div>
        </div>

        <div className="card-footer">
          <button
            className="btn btn-outline-secondary"
            onClick={this.back}
            style={{ width: "80px" }}
          >
            <i className="fa fa-chevron-left"></i> Back
          </button>
          &nbsp;
          <NavLink
            className="btn btn-primary"
            to={"/users/edit/" + this.props.user.id}
          >
            Edit
          </NavLink>
          &nbsp;
          <button
            type="button"
            className="btn btn-secondary"
            onClick={() => this.showSetPasswordModal()}
          >
            Set Password
          </button>
          &nbsp;
          <button
            type="button"
            className="btn btn-secondary"
            onClick={() => this.showSendPasswordResetEmailModal()}
          >
            Send Password Reset Email
          </button>
          &nbsp;
          <button
            type="button"
            className="btn btn-secondary"
            onClick={() => this.showSendEmailAddressConfirmationEmailModal()}
          >
            Send Email Address Confirmation Email
          </button>
        </div>

        {setPasswordModal}
        {sendPasswordResetEmailModal}
        {sendEmailAddressConfirmationEmailModal}
      </div>
    ) : null;
    return page;
  }
}

const mapStateToProps = (state) => {
  return {
    user: state.user.user,
    postError: state.user.error,
    savedPassword: state.user.savedPassword,
    sentPasswordResetEmail: state.user.sentPasswordResetEmail,
    sentEmailAddressConfirmationEmail:
      state.user.sentEmailAddressConfirmationEmail,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchUser: (id) => dispatch(actions.fetchUser(id)),
    setPasswordInit: () => dispatch(actions.setPasswordInit()),
    setPassword: (password) => dispatch(actions.setPassword(password)),
    sendPasswordResetEmailInit: () =>
      dispatch(actions.sendPasswordResetEmailInit()),
    sendPasswordResetEmail: (id) =>
      dispatch(actions.sendPasswordResetEmail(id)),
    sendEmailAddressConfirmationEmailInit: () =>
      dispatch(actions.sendEmailAddressConfirmationEmailInit()),
    sendEmailAddressConfirmationEmail: (id) =>
      dispatch(actions.sendEmailAddressConfirmationEmail(id)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ViewUser);
