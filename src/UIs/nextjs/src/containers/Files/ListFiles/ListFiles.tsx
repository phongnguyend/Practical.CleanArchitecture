"use client";

import { Component } from "react";
import { Modal, Button } from "react-bootstrap";
import axios from "../axios";
import Link from "next/link";

type State = {
  pageTitle: string;
  showDeleteModal: boolean;
  deletingFile: {};
  showAuditLogsModal: boolean;
  files: [];
  auditLogs: [];
  errorMessage: string;
};

class ListFiles extends Component<State> {
  state = {
    pageTitle: "Files",
    showDeleteModal: false,
    deletingFile: {
      name: null,
    },
    showAuditLogsModal: false,
  };

  fetchFiles = async () => {
    try {
      const response = await axios.get("");
      this.setState({ files: response.data });
    } catch (error) {
      console.log(error);
    }
  };

  downloadFile = async (file) => {
    try {
      const response = await axios.get(file.id + "/download", {
        responseType: "blob",
      });
      const url = window.URL.createObjectURL(response.data);
      const element = document.createElement("a");
      element.href = url;
      element.download = file.fileName;
      document.body.appendChild(element);
      element.click();
    } catch (error) {
      console.log(error);
    }
  };

  viewAuditLogs = async (file) => {
    try {
      const response = await axios.get(file.id + "/auditLogs");
      this.setState({ auditLogs: response.data });
      this.setState({ showAuditLogsModal: true });
    } catch (error) {
      //
    }
  };

  deleteFile = (file) => {
    this.setState({ showDeleteModal: true, deletingFile: file });
  };

  deleteCanceled = () => {
    this.setState({ showDeleteModal: false, deletingFile: {} });
  };

  deleteConfirmed = async () => {
    const file = this.state.deletingFile;
    try {
      await axios.delete(file.id, file);
      await this.fetchFiles();
      this.setState({ showDeleteModal: false, deletingFile: {} });
    } catch (error) {
      console.log(error);
    }
  };

  formatDateTime = (value: string) => {
    if (!value) return value;
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  componentDidMount() {
    this.fetchFiles();
  }

  render() {
    const rows = this.state.files?.map((file) => (
      <tr key={file.id}>
        <td>
          <Link href={"/files/" + file.id}>
            {file.name + " (" + file.fileName + ")"}
          </Link>
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
          <Link className="btn btn-primary" href={"/files/edit/" + file.id}>
            Edit
          </Link>
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

    const table = this.state.files ? (
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
    const auditLogRows = this.state.auditLogs?.map((auditLog) => (
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
            <Link
              className="btn btn-primary"
              style={{ float: "right" }}
              href="/files/upload"
            >
              Upload File
            </Link>
          </div>
          <div className="card-body">
            <div className="table-responsive">{table}</div>
          </div>
        </div>
        {this.state.errorMessage ? (
          <div className="alert alert-danger">
            Error: {this.state.errorMessage}
          </div>
        ) : null}
        {deleteModal}
        {auditLogsModal}
      </div>
    );
  }
}

export default ListFiles;
