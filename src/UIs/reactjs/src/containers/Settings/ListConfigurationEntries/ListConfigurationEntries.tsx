import React, { Component } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";

class ListConfigurationEntries extends Component<any, any> {
  state = {
    showDeleteModal: false,
    showAddUpdateModel: false,
    selectingEntry: {
      id: "",
      key: "",
      value: "",
      description: "",
      isSensitive: false,
    },
    controls: {
      key: {
        validation: {
          required: true,
        },
        error: {
          required: false,
        },
        valid: false,
        touched: false,
      },
      value: {
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

  fieldChanged = (event) => {
    let value = event.target.value;

    if (event.target.type == "checkbox") {
      value = event.target.checked;
    }

    const selectingEntry = {
      ...this.state.selectingEntry,
      [event.target.name]: value,
    };

    this.checkFieldValidity(event.target.name, value);

    this.setState({ selectingEntry: selectingEntry });
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

  deleteEntry = (entry) => {
    this.setState({ showDeleteModal: true, selectingEntry: entry });
  };

  deleteCanceled = () => {
    this.setState({ showDeleteModal: false, selectingEntry: null });
  };

  deleteConfirmed = () => {
    this.props.deleteConfigurationEntry(this.state.selectingEntry);
    this.setState({ showDeleteModal: false, selectingEntry: null });
  };

  addEntry = () => {
    this.setState({ showAddUpdateModel: true, selectingEntry: {} });
  };

  updateEntry = (entry) => {
    this.setState({
      showAddUpdateModel: true,
      selectingEntry: {
        id: entry.id,
        key: entry.key,
        value: entry.isSensitive ? "" : entry.value,
        description: entry.description,
        isSensitive: entry.isSensitive,
      },
    });
  };

  addUpdateCanceled = () => {
    this.setState({
      showAddUpdateModel: false,
      selectingEntry: null,
      submitted: false,
    });
  };

  addUpdateConfirmed = (event) => {
    event.preventDefault();
    this.setState({ submitted: true });

    let isValid = true;
    for (let fieldName in this.state.controls) {
      isValid =
        this.checkFieldValidity(
          fieldName,
          this.state.selectingEntry[fieldName]
        ) && isValid;
    }

    if (isValid) {
      this.props.saveConfigurationEntry(this.state.selectingEntry);
      this.setState({ showAddUpdateModel: false, selectingEntry: null });
    }
  };

  formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  componentDidMount() {
    this.props.fetchConfigurationEntries();
  }

  render() {
    const rows = this.props.configurationEntries?.map((entry) => (
      <tr key={entry.id}>
        <td>{entry.key}</td>
        <td>{entry.isSensitive ? "******" : entry.value}</td>
        <td>{entry.description}</td>
        <td>{this.formatDateTime(entry.updatedDateTime)}</td>
        <td>
          <button
            className="btn btn-primary"
            onClick={() => this.updateEntry(entry)}
          >
            Edit
          </button>
          &nbsp;
          <button
            type="button"
            className="btn btn-primary btn-danger"
            onClick={() => this.deleteEntry(entry)}
          >
            Delete
          </button>
        </td>
      </tr>
    ));

    const table = this.props.configurationEntries ? (
      <table className="table">
        <thead>
          <tr>
            <th>Key</th>
            <th>Value</th>
            <th>Description</th>
            <th>Updated Time</th>
            <th></th>
          </tr>
        </thead>
        <tbody>{rows}</tbody>
      </table>
    ) : null;

    const deleteModal = (
      <Modal show={this.state.showDeleteModal} onHide={this.deleteCanceled}>
        <Modal.Header closeButton>
          <Modal.Title>Delete Entry</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to delete
          <strong> {this.state.selectingEntry?.key}</strong>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={this.deleteCanceled}>
            No
          </Button>
          <Button variant="primary" onClick={this.deleteConfirmed}>
            Yes
          </Button>
        </Modal.Footer>
      </Modal>
    );

    const addUpdateModal = (
      <Modal
        show={this.state.showAddUpdateModel && !this.props.saved}
        onHide={this.addUpdateCanceled}
      >
        <Modal.Header closeButton>
          <Modal.Title>
            {!this.state.selectingEntry?.id ? "Add" : "Update"}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <form onSubmit={this.addUpdateConfirmed}>
            <div className="form-group row">
              <label htmlFor="key" className="col-sm-3 col-form-label">
                Key
              </label>
              <div className="col-sm-9">
                <input
                  id="key"
                  name="key"
                  className={
                    "form-control " +
                    (this.state.submitted && !this.state.controls["key"].valid
                      ? "is-invalid"
                      : "")
                  }
                  value={this.state.selectingEntry?.key}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  {this.state.controls["key"].error.required ? (
                    <span>Enter a key</span>
                  ) : null}
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="value" className="col-sm-3 col-form-label">
                Value
              </label>
              <div className="col-sm-9">
                <input
                  id="value"
                  name="value"
                  className={
                    "form-control " +
                    (this.state.submitted && !this.state.controls["value"].valid
                      ? "is-invalid"
                      : "")
                  }
                  value={this.state.selectingEntry?.value}
                  onChange={(event) => this.fieldChanged(event)}
                />
                <span className="invalid-feedback">
                  {this.state.controls["value"].error.required ? (
                    <span>Enter a value</span>
                  ) : null}
                </span>
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="description" className="col-sm-3 col-form-label">
                Description
              </label>
              <div className="col-sm-9">
                <input
                  id="description"
                  name="description"
                  className="form-control"
                  value={this.state.selectingEntry?.description}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label htmlFor="isSensitive" className="col-sm-3 col-form-label">
                Sensitive
              </label>
              <div className="col-sm-9">
                <input
                  type="checkbox"
                  id="isSensitive"
                  name="isSensitive"
                  checked={this.state.selectingEntry?.isSensitive}
                  onChange={(event) => this.fieldChanged(event)}
                />
              </div>
            </div>
            <div className="form-group row">
              <label className="col-sm-3 col-form-label"></label>
              <div className="col-sm-9">
                <button className="btn btn-primary">Save</button>
              </div>
            </div>
          </form>
        </Modal.Body>
      </Modal>
    );

    return (
      <div>
        <div className="card">
          <div className="card-header">
            Settings
            <button
              className="btn btn-primary"
              style={{ float: "right" }}
              onClick={() => this.addEntry()}
            >
              Add
            </button>
          </div>
          <div className="card-body">
            <div className="table-responsive">{table}</div>
          </div>
        </div>
        {this.props.errorMessage ? (
          <div className="alert alert-danger">
            Error: {this.props.errorMessage}
          </div>
        ) : null}
        {deleteModal}
        {addUpdateModal}
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    configurationEntries: state.configurationEntry.configurationEntries,
    saved: state.configurationEntry.saved,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchConfigurationEntries: () =>
      dispatch(actions.fetchConfigurationEntries()),
    //updateConfigurationEntry: (entry) =>
    //dispatch(actions.updateConfigurationEntry(entry)),
    //resetConfigurationEntry: () => dispatch(actions.resetConfigurationEntry()),
    saveConfigurationEntry: (entry) =>
      dispatch(actions.saveConfigurationEntry(entry)),
    deleteConfigurationEntry: (entry) =>
      dispatch(actions.deleteConfigurationEntry(entry)),
  };
};

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ListConfigurationEntries);
