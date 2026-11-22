import AuditEntries from "../../../components/AuditEntries/AuditEntries";
import ActionGroup from "../../../components/ActionGroup/ActionGroup";
import { useEffect, useState } from "react";
import { ChevronLeft } from "lucide-react";
import { NavLink, Navigate, useParams } from "react-router-dom";
import Modal from "../../../components/Modal/Modal";

import { checkValidity } from "../../../shared/utility";
import axios from "../axios";
import ActionIcon from "../../../components/ActionIcon/ActionIcon";

const EditFile = () => {
  const [state, setState] = useState<any>({
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

  const [file, setFile] = useState<Record<string, any>>({});
  const [auditLogs, setAuditLogs] = useState<any[]>([]);

  const { id } = useParams();

  const updateFile = (file: any) => {
    setFile(file);
  };
  const saveFile = async (file: any) => {
    try {
      const response = await axios.put(file.id, file);
      setFile(response.data);
      setState((preState: any) => {
        return { ...preState, saved: true };
      });
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    let active = true;
    if (typeof id === "string") {
      axios
        .get(id)
        .then((response) => {
          if (active) {
            setFile(response.data);
          }
        })
        .catch(() => {});
    }
    return () => {
      active = false;
    };
  }, [id]);

  const fieldChanged = (event: any) => {
    checkFieldValidity(event.target.name, event.target.value);

    updateFile({
      ...file,
      [event.target.name]: event.target.value,
    });
  };

  const checkFieldValidity = (name: any, value: any) => {
    const control = state.controls[name];
    const rules = control.validation;
    const validationRs = checkValidity(value, rules);

    setState((preState: any) => ({
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

  const onSubmit = async (event: any) => {
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

  const formatDateTime = (value: any) => {
    if (!value) {
      return value;
    }
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  const form = (
    <div className="card">
      <div className="card-header">
        <ActionIcon action="files" />
        {state.title}
      </div>
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
              <button className="btn btn-primary">
                <ActionIcon action="save" /> Save
              </button>
            </div>
          </div>
        </form>
      </div>
      <div className="card-footer">
        <ActionGroup>
          <NavLink className="btn btn-outline-secondary" to="/files">
            <ChevronLeft size={16} aria-hidden="true" /> Back
          </NavLink>

          <button
            type="button"
            className="btn btn-secondary"
            onClick={() => viewAuditLogs()}
          >
            <ActionIcon action="history" /> View Audit Logs
          </button>
        </ActionGroup>
      </div>
    </div>
  );

  const auditLogsModal = (
    <Modal
      size="xl"
      show={state.showAuditLogsModal}
      onHide={() => setState({ ...state, showAuditLogsModal: false })}
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
