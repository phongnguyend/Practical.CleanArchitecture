import React, { Component } from "react";
import { NavLink, Redirect } from "react-router-dom";
import { connect } from "react-redux";

import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";

class UploadFile extends Component {
  state = {
    title: "Upload File",
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
    formFile: null,
    hasFile: false,
    submitted: false,
    errorMessage: null,
  };

  componentDidMount() {
    this.props.resetFile();
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

    if (isValid && this.state.hasFile) {
      this.props.saveFile(this.props.file);
    }
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
                  type="file"
                  name="formFile"
                  className={
                    "form-control " +
                    (this.state.submitted && !this.state.hasFile
                      ? "is-invalid"
                      : "")
                  }
                  onChange={this.handleFileInput}
                />
                <span className="invalid-feedback">Select a file</span>
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
          {this.props.file?.id ? (
            <button
              type="button"
              className="btn btn-primary btn-secondary"
              onClick={() => this.viewAuditLogs()}
            >
              View Audit Logs
            </button>
          ) : null}
        </div>
      </div>
    );

    return this.state.submitted && this.props.saved ? (
      <Redirect to={"/files/edit/" + this.props.file.id} />
    ) : (
      form
    );
  }
}

const mapStateToProps = (state) => {
  return {
    file: state.file.file,
    saved: state.file.saved,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    updateFile: (file) => dispatch(actions.updateFile(file)),
    resetFile: () => dispatch(actions.resetFile()),
    saveFile: (file) => dispatch(actions.saveFile(file)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(UploadFile);
