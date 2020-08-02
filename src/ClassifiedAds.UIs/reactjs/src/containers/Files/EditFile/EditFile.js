import React, { Component } from "react";
import { NavLink, Redirect } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";

class EditFile extends Component {
  state = {
    title: "Edit File",
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
    showAuditLogsModal: false,
    errorMessage: null,
  };

  componentDidMount() {
    this.props.resetFile();
    const id = this.props.match?.params?.id;
    if (id) {
      this.props.fetchFile(id);
    }
  }

  fieldChanged = (event) => {
    const file = {
      ...this.props.file,
      [event.target.name]: event.target.value,
    };

    this.checkFieldValidity(event.target.name, event.target.value);

    this.props.updateFile(file);
  };

  checkFieldValidity = (name, value) => {
    const control = this.state.controls[name];
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

  handleFileInput = (event) => {
    var formFile = event.target.files.item(0);
    this.setState({ formFile: formFile, hasFile: !!formFile });
    const file = {
      ...this.props.file,
      formFile: formFile,
    };
    this.props.updateFile(file);
  };

  onSubmit = (event) => {
    event.preventDefault();
    this.setState({ submitted: true });
    let isValid = true;
    for (let fieldName in this.state.controls) {
      isValid =
        this.checkFieldValidity(fieldName, this.props.file[fieldName]) &&
        isValid;
    }

    if (isValid) {
      this.props.saveFile(this.props.file);
    }
  };

  viewAuditLogs = () => {
    this.props.fetchAuditLogs(this.props.file);
    this.setState({ showAuditLogsModal: true });
  };

  formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  render() {
    const form = (
      <div className="card">
        <div className="card-header">{this.state.title}</div>
        <div className="card-body">
          {this.state.errorMessage ? (
            <div
              className="row"
              hidden="!postError"
              className="alert alert-danger"
            >
              {this.state.errorMessage}
            </div>
          ) : null}
          <form onSubmit={this.onSubmit}>
            <div className="form-group row">
              <label htmlFor="name" className="col-sm-2 col-form-label">
                Name
              </label>
              <div className="col-sm-10">
                <input
                  id="name"
                  name="name"
                  className={
                    "form-control " +
                    (this.state.submitted && !this.state.controls["name"].valid
                      ? "is-invalid"
                      : "")
                  }
                  value={this.props.file?.name}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  {this.state.controls["name"].error.required ? (
                    <span>Enter a name</span>
                  ) : null}
                  {this.state.controls["name"].error.minLength ? (
                    <span>The name must be longer than 3 characters.</span>
                  ) : null}
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="description" className="col-sm-2 col-form-label">
                Description
              </label>
              <div className="col-sm-10">
                <input
                  id="description"
                  name="description"
                  className={
                    "form-control " +
                    (this.state.submitted &&
                    !this.state.controls["description"].valid
                      ? "is-invalid"
                      : "")
                  }
                  value={this.props.file?.description}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  {this.state.controls["description"].error.required ? (
                    <span>Enter a description</span>
                  ) : null}
                  {this.state.controls["description"].error.maxLength ? (
                    <span>The code must be less than 100 characters.</span>
                  ) : null}
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="formFile" className="col-sm-2 col-form-label">
                File
              </label>
              <div className="col-sm-10">
                <input
                  id="formFile"
                  name="formFile"
                  className="form-control"
                  disabled
                  value={this.props.file?.fileName}
                />
              </div>
            </div>
            <div className="form-group row">
              <label
                htmlFor="description"
                className="col-sm-2 col-form-label"
              ></label>
              <div className="col-sm-10">
                <button className="btn btn-primary">Save</button>
              </div>
            </div>
          </form>
        </div>
        <div className="card-footer">
          <NavLink
            className="btn btn-outline-secondary"
            to="/files"
            style={{ width: "80px" }}
          >
            <i className="fa fa-chevron-left"></i> Back
          </NavLink>
          &nbsp;
          <button
            type="button"
            className="btn btn-primary btn-secondary"
            onClick={() => this.viewAuditLogs()}
          >
            View Audit Logs
          </button>
        </div>
      </div>
    );

    const auditLogRows = this.props.auditLogs?.map((auditLog) => (
      <tr key={auditLog.id}>
        <td>{this.formatDateTime(auditLog.createdDateTime)}</td>
        <td>{auditLog.userName}</td>
        <td>{auditLog.action}</td>
        <td style={{ color: auditLog.highLight.name ? "red" : "" }}>
          {auditLog.data.name}
        </td>
        <td style={{ color: auditLog.highLight.description ? "red" : "" }}>
          {auditLog.data.description}
        </td>
        <td style={{ color: auditLog.highLight.fileName ? "red" : "" }}>
          {auditLog.data.fileName}
        </td>
        <td style={{ color: auditLog.highLight.fileLocation ? "red" : "" }}>
          {auditLog.data.fileLocation}
        </td>
      </tr>
    ));
    const auditLogsModal = (
      <Modal
        size="xl"
        show={this.state.showAuditLogsModal}
        onHide={() => this.setState({ showAuditLogsModal: false })}
      >
        <Modal.Body>
          <div className="table-responsive">
            <table className="table">
              <thead>
                <tr>
                  <th>Date Time</th>
                  <th>User Name</th>
                  <th>Action</th>
                  <th>Name</th>
                  <th>Description</th>
                  <th>File Name</th>
                  <th>File Location</th>
                </tr>
              </thead>
              <tbody>{auditLogRows}</tbody>
            </table>
          </div>
        </Modal.Body>
      </Modal>
    );

    return this.state.submitted && this.props.saved ? (
      <Redirect to={"/files"} />
    ) : (
      <div>
        {form}
        {auditLogsModal}
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    file: state.file.file,
    saved: state.file.saved,
    auditLogs: state.file.auditLogs,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFile: (id) => dispatch(actions.fetchFile(id)),
    updateFile: (file) => dispatch(actions.updateFile(file)),
    resetFile: () => dispatch(actions.resetFile()),
    saveFile: (file) => dispatch(actions.saveFile(file)),
    fetchAuditLogs: (file) => dispatch(actions.fetchAuditLogs(file)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(EditFile);
