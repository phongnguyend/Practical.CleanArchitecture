"use client";

import { useEffect, useState } from "react";
import { Modal } from "react-bootstrap";

import { checkValidity } from "../../../shared/utility";
import axios from "../axios";
import Link from "next/link";
import { useRouter, useParams } from "next/navigation";

interface File {
  id: string;
  name: string;
  fileName: string;
  description: string;
  size: number;
  uploadedTime: string;
  encrypted?: boolean;
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

const EditFile = ({}) => {
  const router = useRouter();

  const [state, setState] = useState({
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
    saved: false,
  });

  const [file, setFile] = useState<Partial<File>>({});
  const [auditLogs, setAuditLogs] = useState<AuditLog[]>([]);

  const { id } = useParams();

  const fetchFile = async (id: string) => {
    try {
      const response = await axios.get(id);
      setFile(response.data);
    } catch (error) {
      //
    }
  };
  const updateFile = (file: File) => {
    setFile(file);
  };
  const saveFile = async (file: File) => {
    try {
      const response = await axios.put(file.id, file);
      setFile(response.data);
      setState((preState) => {
        return { ...preState, saved: true };
      });

      router.push("/files");
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    if (id && typeof id === "string") {
      fetchFile(id);
    }
  }, []);

  const fieldChanged = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const fieldName = event.target.name as keyof typeof state.controls;
    checkFieldValidity(fieldName, event.target.value);

    updateFile({
      ...file,
      [event.target.name]: event.target.value,
    } as File);
  };

  const checkFieldValidity = (
    name: keyof typeof state.controls,
    value: any
  ) => {
    const control = state.controls[name];
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

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setState({ ...state, submitted: true });
    let isValid = true;
    for (let fieldName in state.controls) {
      const key = fieldName as keyof typeof state.controls;
      const value = (file as any)[fieldName];
      isValid = checkFieldValidity(key, value) && isValid;
    }

    if (isValid) {
      await saveFile(file as File);
    }
  };

  const viewAuditLogs = async () => {
    try {
      const response = await axios.get(file.id + "/auditLogs");
      setAuditLogs(response.data);
      setState({ ...state, showAuditLogsModal: true });
    } catch (error) {
      //
    }
  };

  const formatDateTime = (value: string) => {
    if (!value) return value;
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
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
            <label htmlFor="name" className="col-sm-2 col-form-label">
              Name
            </label>
            <div className="col-sm-10">
              <input
                id="name"
                name="name"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["name"].valid
                    ? "is-invalid"
                    : "")
                }
                value={file?.name}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["name"].error.required ? (
                  <span>Enter a name</span>
                ) : null}
                {state.controls["name"].error.minLength ? (
                  <span>The name must be longer than 3 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="description" className="col-sm-2 col-form-label">
              Description
            </label>
            <div className="col-sm-10">
              <input
                id="description"
                name="description"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["description"].valid
                    ? "is-invalid"
                    : "")
                }
                value={file?.description}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["description"].error.required ? (
                  <span>Enter a description</span>
                ) : null}
                {state.controls["description"].error.maxLength ? (
                  <span>The code must be less than 100 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="formFile" className="col-sm-2 col-form-label">
              File
            </label>
            <div className="col-sm-10">
              <input
                id="formFile"
                name="formFile"
                className="form-control"
                disabled
                value={file?.fileName}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="encrypted" className="col-sm-2 col-form-label">
              Encrypted
            </label>
            <div className="col-sm-10">
              <input
                type="checkbox"
                id="encrypted"
                name="encrypted"
                disabled
                checked={file?.encrypted}
              />
            </div>
          </div>
          <div className="mb-3 row">
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
        <Link
          className="btn btn-outline-secondary"
          href="/files"
          style={{ width: "80px" }}
        >
          <i className="fa fa-chevron-left"></i> Back
        </Link>
        &nbsp;
        <button
          type="button"
          className="btn btn-primary btn-secondary"
          onClick={() => viewAuditLogs()}
        >
          View Audit Logs
        </button>
      </div>
    </div>
  );

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
      show={state.showAuditLogsModal}
      onHide={() => setState({ ...state, showAuditLogsModal: false })}
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

  return (
    <div>
      {form}
      {auditLogsModal}
    </div>
  );
};

export default EditFile;
