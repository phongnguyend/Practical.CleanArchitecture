import { useEffect, useState } from "react";
import { NavLink, Navigate, useParams } from "react-router-dom";
import { Modal } from "react-bootstrap";

import { checkValidity } from "../../../shared/utility";
import axios from "../axios";

const EditFile = () => {
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

  const [file, setFile] = useState({});
  const [auditLogs, setAuditLogs] = useState([]);

  const { id } = useParams();

  const fetchFile = async (id) => {
    try {
      const response = await axios.get(id);
      setFile(response.data);
    } catch (error) {
      //
    }
  };
  const updateFile = (file) => {
    setFile(file);
  };
  const saveFile = async (file) => {
    try {
      const response = await axios.put(file.id, file);
      setFile(response.data);
      setState((preState) => {
        return { ...preState, saved: true };
      });
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    if (id) {
      fetchFile(id);
    }
  }, []);

  const fieldChanged = (event) => {
    checkFieldValidity(event.target.name, event.target.value);

    updateFile({
      ...file,
      [event.target.name]: event.target.value,
    });
  };

  const checkFieldValidity = (name, value) => {
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

  const onSubmit = async (event) => {
    event.preventDefault();
    setState({ ...state, submitted: true });
    let isValid = true;
    for (let fieldName in state.controls) {
      isValid = checkFieldValidity(fieldName, file[fieldName]) && isValid;
    }

    if (isValid) {
      await saveFile(file);
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

  const formatDateTime = (value) => {
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
                  (state.submitted && !state.controls["name"].valid ? "is-invalid" : "")
                }
                value={file?.name}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["name"].error.required ? <span>Enter a name</span> : null}
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
                  (state.submitted && !state.controls["description"].valid ? "is-invalid" : "")
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
            <label htmlFor="description" className="col-sm-2 col-form-label"></label>
            <div className="col-sm-10">
              <button className="btn btn-primary">Save</button>
            </div>
          </div>
        </form>
      </div>
      <div className="card-footer">
        <NavLink className="btn btn-outline-secondary" to="/files" style={{ width: "80px" }}>
          <i className="fa fa-chevron-left"></i> Back
        </NavLink>
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
      <td style={{ color: auditLog.highLight.name ? "red" : "" }}>{auditLog.data.name}</td>
      <td style={{ color: auditLog.highLight.description ? "red" : "" }}>
        {auditLog.data.description}
      </td>
      <td style={{ color: auditLog.highLight.fileName ? "red" : "" }}>{auditLog.data.fileName}</td>
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

  return state.submitted && state.saved ? (
    <Navigate to={"/files"} />
  ) : (
    <div>
      {form}
      {auditLogsModal}
    </div>
  );
};

export default EditFile;
