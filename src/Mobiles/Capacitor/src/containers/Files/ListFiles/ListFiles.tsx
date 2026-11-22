import AuditEntries from "../../../components/AuditEntries/AuditEntries";
import PageHeader from "../../../components/PageHeader/PageHeader";
import PagedTable from "../../../components/PagedTable/PagedTable";
import { downloadOrShare } from "../../../core/files/native-files";
import { useState, useEffect } from "react";
import { NavLink } from "react-router-dom";
import { Button } from "react-bootstrap";
import Modal from "../../../components/Modal/Modal";
import axios from "../axios";
import ActionIcon from "../../../components/ActionIcon/ActionIcon";

type FileType = {
  id: string;
  name: string;
  fileName: string;
};

const ListFiles = () => {
  const [pageTitle] = useState("Files");
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deletingFile, setDeletingFile] = useState<FileType>({
    id: "",
    name: "",
    fileName: "",
  });
  const [showAuditLogsModal, setShowAuditLogsModal] = useState(false);
  const [files, setFiles] = useState<any[]>([]);
  const [auditLogs, setAuditLogs] = useState<any[]>([]);
  const [errorMessage, setErrorMessage] = useState("");

  const fetchFiles = async () => {
    try {
      const response = await axios.get("");
      setFiles(response.data);
    } catch (error) {
      console.log(error);
    }
  };

  const downloadFile = async (file: FileType) => {
    try {
      const response = await axios.get(file.id + "/download", {
        responseType: "blob",
      });
      await downloadOrShare(response.data, file.fileName);
    } catch (error) {
      console.log(error);
    }
  };

  const viewAuditLogs = async (file: FileType) => {
    try {
      const response = await axios.get(file.id + "/auditLogs");
      setAuditLogs(response.data);
      setShowAuditLogsModal(true);
    } catch (error) {
      //
    }
  };

  const deleteFile = (file: FileType) => {
    setShowDeleteModal(true);
    setDeletingFile(file);
  };

  const deleteCanceled = () => {
    setShowDeleteModal(false);
    setDeletingFile({ id: "", name: "", fileName: "" });
  };

  const deleteConfirmed = async () => {
    const file = deletingFile;
    try {
      await axios.delete(file.id);
      await fetchFiles();
      setShowDeleteModal(false);
      setDeletingFile({ id: "", name: "", fileName: "" });
    } catch (error) {
      console.log(error);
    }
  };

  const formatDateTime = (value: string) => {
    if (!value) {
      return value;
    }
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  useEffect(() => {
    fetchFiles();
  }, []);

  const rows = files?.map((file: any) => (
    <tr key={file.id}>
      <td>
        <NavLink to={"/files/edit/" + file.id}>
          <ActionIcon action="view" />
          {file.name + " (" + file.fileName + ")"}
        </NavLink>
      </td>
      <td>{file.description}</td>
      <td>{file.size}</td>
      <td>{formatDateTime(file.uploadedTime)}</td>
      <td>
        <button
          type="button"
          className="btn btn-secondary"
          onClick={() => downloadFile(file)}
        >
          <ActionIcon action="download" /> Download
        </button>

        <NavLink className="btn btn-primary" to={"/files/edit/" + file.id}>
          <ActionIcon action="edit" /> Edit
        </NavLink>

        <button
          type="button"
          className="btn btn-secondary"
          onClick={() => viewAuditLogs(file)}
        >
          <ActionIcon action="history" /> History
        </button>

        <button
          type="button"
          className="btn btn-danger"
          onClick={() => deleteFile(file)}
        >
          <ActionIcon action="delete" /> Delete
        </button>
      </td>
    </tr>
  ));

  const table = files ? (
    <PagedTable searchable={true}>
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
    </PagedTable>
  ) : null;

  const auditLogsModal = (
    <Modal
      size="xl"
      show={showAuditLogsModal}
      onHide={() => setShowAuditLogsModal(false)}
    >
      <Modal.Header closeButton>
        <Modal.Title>
          <ActionIcon action="audit" />
          Audit Logs
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <AuditEntries entries={auditLogs ?? []} fields={["name", "description", "fileName", "fileLocation"]} />
      </Modal.Body>
    </Modal>
  );

  const deleteModal = (
    <Modal show={showDeleteModal} onHide={deleteCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>
          <ActionIcon action="delete" />
          Delete File
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        Are you sure you want to delete
        <strong> {deletingFile?.name}</strong>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={deleteCanceled}>
          <ActionIcon action="cancel" /> No
        </Button>
        <Button variant="primary" onClick={deleteConfirmed}>
          <ActionIcon action="confirm" /> Yes
        </Button>
      </Modal.Footer>
    </Modal>
  );

  return (
    <div>
      <div className="card">
        <PageHeader title={pageTitle} icon="files" onRefresh={fetchFiles}>
          <NavLink
            className="btn btn-primary"

            to="/files/upload"
          >
            <ActionIcon action="upload" /> Upload file
          </NavLink>
        </PageHeader>
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
