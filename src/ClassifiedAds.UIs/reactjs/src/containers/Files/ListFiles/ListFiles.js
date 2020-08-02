import React, { Component } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import * as actions from "../actions";

class ListFiles extends Component {
  state = {
    pageTitle: "Files",
    showDeleteModal: false,
    deletingFile: null,
    showAuditLogsModal: false,
  };

  downloadFile = (file) => {
    this.props.downloadFile(file);
  };

  viewAuditLogs = (file) => {
    this.props.fetchAuditLogs(file);
    this.setState({ showAuditLogsModal: true });
  };

  deleteFile = (file) => {
    this.setState({ showDeleteModal: true, deletingFile: file });
  };

  deleteCanceled = () => {
    this.setState({ showDeleteModal: false, deletingFile: null });
  };

  deleteConfirmed = () => {
    this.props.deleteFile(this.state.deletingFile);
    this.setState({ showDeleteModal: false, deletingFile: null });
  };

  formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  componentDidMount() {
    this.props.fetchFiles();
  }

  render() {
    const rows = this.props.files?.map((file) => (
      <tr key={file.id}>
        <td>
          <NavLink to={"/files/" + file.id}>
            {file.name + " (" + file.fileName + ")"}
          </NavLink>
        </td>
        <td>{file.description}</td>
        <td>{file.size}</td>
        <td>{this.formatDateTime(file.uploadedTime)}</td>
        <td>
          <button
            type="button"
            className="btn btn-primary btn-secondary"
            onClick={() => this.downloadFile(file)}
          >
            Download
          </button>
          &nbsp;
          <NavLink className="btn btn-primary" to={"/files/edit/" + file.id}>
            Edit
          </NavLink>
          &nbsp;
          <button
            type="button"
            className="btn btn-primary btn-secondary"
            onClick={() => this.viewAuditLogs(file)}
          >
            View Audit Logs
          </button>
          &nbsp;
          <button
            type="button"
            className="btn btn-primary btn-danger"
            onClick={() => this.deleteFile(file)}
          >
            Delete
          </button>
        </td>
      </tr>
    ));

    const table = this.props.files ? (
      <table className="table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Description</th>
            <th>Size</th>
            <th>Uploaded Time</th>
            <th></th>
          </tr>
        </thead>
        <tbody>{rows}</tbody>
      </table>
    ) : null;
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

    const deleteModal = (
      <Modal show={this.state.showDeleteModal} onHide={this.deleteCanceled}>
        <Modal.Header closeButton>
          <Modal.Title>Delete File</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to delete
          <strong> {this.state.deletingFile?.name}</strong>
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

    return (
      <div>
        <div className="card">
          <div className="card-header">
            {this.state.pageTitle}
            <NavLink
              className="btn btn-primary"
              style={{ float: "right" }}
              to="/files/upload"
            >
              Upload File
            </NavLink>
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
        {auditLogsModal}
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    files: state.file.files,
    auditLogs: state.file.auditLogs,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchFiles: () => dispatch(actions.fetchFiles()),
    deleteFile: (file) => dispatch(actions.deleteFile(file)),
    downloadFile: (file) => dispatch(actions.downloadFile(file)),
    fetchAuditLogs: (file) => dispatch(actions.fetchAuditLogs(file)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ListFiles);
