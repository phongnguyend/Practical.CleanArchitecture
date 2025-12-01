"use client";

import { useState, useEffect } from "react";
import { Modal, Button } from "react-bootstrap";
import axios from "../axios";
import Link from "next/link";

interface File {
  id: string;
  name: string;
  fileName: string;
  description: string;
  size: number;
  uploadedTime: string;
}

interface AuditLog {
  id: string;
  createdDateTime: string;
  userName: string;
  action: string;
  data: {
    name: string;
    description: string;
    fileName: string;
    fileLocation: string;
  };
  highLight: {
    name: boolean;
    description: boolean;
    fileName: boolean;
    fileLocation: boolean;
  };
}

interface DeletingFile {
  id?: string;
  name?: string;
}

const ListFiles = () => {
  const [pageTitle] = useState("Files");
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deletingFile, setDeletingFile] = useState<DeletingFile>({
    name: undefined,
  });
  const [showAuditLogsModal, setShowAuditLogsModal] = useState(false);
  const [files, setFiles] = useState<File[]>([]);
  const [auditLogs, setAuditLogs] = useState<AuditLog[]>([]);
  const [errorMessage, setErrorMessage] = useState("");

  const fetchFiles = async () => {
    try {
      const response = await axios.get("");
      setFiles(response.data);
    } catch (error) {
      console.log(error);
    }
  };

  const downloadFile = async (file: File) => {
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

  const viewAuditLogs = async (file: File) => {
    try {
      const response = await axios.get(file.id + "/auditLogs");
      setAuditLogs(response.data);
      setShowAuditLogsModal(true);
    } catch (error) {
      //
    }
  };

  const deleteFile = (file: File) => {
    setShowDeleteModal(true);
    setDeletingFile(file);
  };

  const deleteCanceled = () => {
    setShowDeleteModal(false);
    setDeletingFile({});
  };

  const deleteConfirmed = async () => {
    try {
      await axios.delete(deletingFile.id!);
      await fetchFiles();
      setShowDeleteModal(false);
      setDeletingFile({});
    } catch (error) {
      console.log(error);
    }
  };

  const formatDateTime = (value: string) => {
    if (!value) return value;
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  useEffect(() => {
    fetchFiles();
  }, []);

  const rows = files?.map((file) => (
    <tr key={file.id}>
      <td>
        <Link href={"/files/" + file.id}>
          {file.name + " (" + file.fileName + ")"}
        </Link>
      </td>
      <td>{file.description}</td>
      <td>{file.size}</td>
      <td>{formatDateTime(file.uploadedTime)}</td>
      <td>
        <button
          type="button"
          className="btn btn-primary btn-secondary"
          onClick={() => downloadFile(file)}
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
          onClick={() => viewAuditLogs(file)}
        >
          View Audit Logs
        </button>
        &nbsp;
        <button
          type="button"
          className="btn btn-primary btn-danger"
          onClick={() => deleteFile(file)}
        >
          Delete
        </button>
      </td>
    </tr>
  ));

  const table = files ? (
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

  const auditLogRows = auditLogs?.map((auditLog) => (
    <tr key={auditLog.id}>
      <td>{formatDateTime(auditLog.createdDateTime)}</td>
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
      show={showAuditLogsModal}
      onHide={() => setShowAuditLogsModal(false)}
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
    <Modal show={showDeleteModal} onHide={deleteCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>Delete File</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        Are you sure you want to delete
        <strong> {deletingFile?.name}</strong>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={deleteCanceled}>
          No
        </Button>
        <Button variant="primary" onClick={deleteConfirmed}>
          Yes
        </Button>
      </Modal.Footer>
    </Modal>
  );

  return (
    <div>
      <div className="card">
        <div className="card-header">
          {pageTitle}
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
      {errorMessage ? (
        <div className="alert alert-danger">Error: {errorMessage}</div>
      ) : null}
      {deleteModal}
      {auditLogsModal}
    </div>
  );
};

export default ListFiles;
