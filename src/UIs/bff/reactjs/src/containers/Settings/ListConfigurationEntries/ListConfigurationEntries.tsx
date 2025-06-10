import React, { useState, useEffect } from "react";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import * as actions from "../actions";
import { checkValidity } from "../../../shared/utility";
import axios from "../axios";

const ListConfigurationEntries = (props: any) => {
    const [deleteModalOpen, setDeleteModalOpen] = useState(false);
    const [addUpdateModalOpen, setAddUpdateModalOpen] = useState(false);
    const [addUpdateFormSubmitted, setAddUpdateFormSubmitted] = useState(false);
    const [importExcelModalOpen, setImportExcelModalOpen] = useState(false);
    const [importingFile, setImportingFile] = useState<File | null>(null);
    const [importExcelFormSubmitted, setImportExcelFormSubmitted] = useState(false);
    const [selectingEntry, setSelectingEntry] = useState({
        id: "",
        key: "",
        value: "",
        description: "",
        isSensitive: false,
    });
    const [controls, setControls] = useState({
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
    });

    const fieldChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        let value: any = event.target.value;

        if (event.target.type === "checkbox") {
            value = event.target.checked;
        }

        const updatedEntry = {
            ...selectingEntry,
            [event.target.name]: value,
        };

        checkFieldValidity(event.target.name, value);
        setSelectingEntry(updatedEntry);
    };

    const checkFieldValidity = (name: string, value: any) => {
        const control = controls[name as keyof typeof controls];

        if (!control) return true;

        const rules = control.validation;
        const validationRs = checkValidity(value, rules);

        setControls((prevState) => ({
            ...prevState,
            [name]: {
                ...prevState[name as keyof typeof prevState],
                error: validationRs,
                valid: validationRs.isValid,
            },
        }));

        return validationRs.isValid;
    };

    const openDeleteModal = (entry: any) => {
        setDeleteModalOpen(true);
        setSelectingEntry(entry);
    };

    const deleteCanceled = () => {
        setDeleteModalOpen(false);
        setSelectingEntry({
            id: "",
            key: "",
            value: "",
            description: "",
            isSensitive: false,
        });
    };

    const deleteConfirmed = () => {
        props.deleteConfigurationEntry(selectingEntry);
        setDeleteModalOpen(false);
        setSelectingEntry({
            id: "",
            key: "",
            value: "",
            description: "",
            isSensitive: false,
        });
    };

    const openAddModal = () => {
        setAddUpdateModalOpen(true);
        setSelectingEntry({
            id: "",
            key: "",
            value: "",
            description: "",
            isSensitive: false,
        });
    };

    const openUpdateModal = (entry: any) => {
        setAddUpdateModalOpen(true);
        setSelectingEntry({
            id: entry.id,
            key: entry.key,
            value: entry.isSensitive ? "" : entry.value,
            description: entry.description,
            isSensitive: entry.isSensitive,
        });
    };

    const addUpdateCanceled = () => {
        setAddUpdateModalOpen(false);
        setSelectingEntry({
            id: "",
            key: "",
            value: "",
            description: "",
            isSensitive: false,
        });
        setAddUpdateFormSubmitted(false);
    };

    const addUpdateConfirmed = (event: React.FormEvent) => {
        event.preventDefault();
        setAddUpdateFormSubmitted(true);

        let isValid = true;
        for (let fieldName in controls) {
            isValid =
                checkFieldValidity(fieldName, selectingEntry[fieldName as keyof typeof selectingEntry]) &&
                isValid;
        }

        if (isValid) {
            props.saveConfigurationEntry(selectingEntry);
            setAddUpdateModalOpen(false);
            setSelectingEntry({
                id: "",
                key: "",
                value: "",
                description: "",
                isSensitive: false,
            });
        }
    };

    const formatDateTime = (value: string) => {
        if (!value) return value;
        var date = new Date(value);
        return date.toLocaleDateString() + " " + date.toLocaleTimeString();
    };

    const exportAsExcel = async () => {
        const rs = await axios.get("/ExportAsExcel", { responseType: "blob" });
        const url = window.URL.createObjectURL(rs.data);
        const element = document.createElement("a");
        element.href = url;
        element.download = "Settings.xlsx";
        document.body.appendChild(element);
        element.click();
    };

    const openImportExcelModal = () => {
        setImportExcelModalOpen(true);
        setImportingFile(null);
        setImportExcelFormSubmitted(false);
    };

    const importExcelCanceled = () => {
        setImportExcelModalOpen(false);
        setImportingFile(null);
        setImportExcelFormSubmitted(false);
    };

    const fileChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
        setImportingFile(event.target.files?.item(0) || null);
    };

    const importExcelConfirmed = async (event: React.FormEvent) => {
        event.preventDefault();
        setImportExcelFormSubmitted(true);
        if (!importingFile) {
            return;
        }
        const formData = new FormData();
        formData.append("formFile", importingFile);
        await axios.post("ImportExcel", formData);
        setImportExcelModalOpen(false);
        setImportingFile(null);
        setImportExcelFormSubmitted(false);

        props.fetchConfigurationEntries();
    };

    useEffect(() => {
        props.fetchConfigurationEntries();
    }, []);

    const rows = props.configurationEntries?.map((entry: any) => (
        <tr key={entry.id}>
            <td>{entry.key}</td>
            <td>{entry.isSensitive ? "******" : entry.value}</td>
            <td>{entry.description}</td>
            <td>{formatDateTime(entry.updatedDateTime)}</td>
            <td>
                <button type="button" className="btn btn-primary" onClick={() => openUpdateModal(entry)}>
                    Edit
                </button>
                &nbsp;
                <button type="button" className="btn btn-danger" onClick={() => openDeleteModal(entry)}>
                    Delete
                </button>
            </td>
        </tr>
    ));

    const table = props.configurationEntries ? (
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
        <Modal show={deleteModalOpen} onHide={deleteCanceled}>
            <Modal.Header closeButton>
                <Modal.Title>Delete Entry</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                Are you sure you want to delete
                <strong> {selectingEntry?.key}</strong>
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

    const addUpdateModal = (
        <Modal show={addUpdateModalOpen && !props.saved} onHide={addUpdateCanceled}>
            <Modal.Header closeButton>
                <Modal.Title>{!selectingEntry?.id ? "Add" : "Update"}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form onSubmit={addUpdateConfirmed}>
                    <div className="mb-3 row">
                        <label htmlFor="key" className="col-sm-3 col-form-label">
                            Key
                        </label>
                        <div className="col-sm-9">
                            <input
                                id="key"
                                name="key"
                                className={
                                    "form-control " +
                                    (addUpdateFormSubmitted && !controls["key"].valid ? "is-invalid" : "")
                                }
                                value={selectingEntry?.key}
                                onChange={(event) => fieldChanged(event)}
                            />
                            <span className="invalid-feedback">
                                {controls["key"].error.required ? <span>Enter a key</span> : null}
                            </span>
                        </div>
                    </div>
                    <div className="mb-3 row">
                        <label htmlFor="value" className="col-sm-3 col-form-label">
                            Value
                        </label>
                        <div className="col-sm-9">
                            <input
                                id="value"
                                name="value"
                                className={
                                    "form-control " +
                                    (addUpdateFormSubmitted && !controls["value"].valid ? "is-invalid" : "")
                                }
                                value={selectingEntry?.value}
                                onChange={(event) => fieldChanged(event)}
                            />
                            <span className="invalid-feedback">
                                {controls["value"].error.required ? <span>Enter a value</span> : null}
                            </span>
                        </div>
                    </div>
                    <div className="mb-3 row">
                        <label htmlFor="description" className="col-sm-3 col-form-label">
                            Description
                        </label>
                        <div className="col-sm-9">
                            <input
                                id="description"
                                name="description"
                                className="form-control"
                                value={selectingEntry?.description}
                                onChange={(event) => fieldChanged(event)}
                            />
                        </div>
                    </div>
                    <div className="mb-3 row">
                        <label htmlFor="isSensitive" className="col-sm-3 col-form-label">
                            Sensitive
                        </label>
                        <div className="col-sm-9">
                            <input
                                type="checkbox"
                                id="isSensitive"
                                name="isSensitive"
                                checked={selectingEntry?.isSensitive}
                                onChange={(event) => fieldChanged(event)}
                            />
                        </div>
                    </div>
                    <div className="mb-3 row">
                        <label className="col-sm-3 col-form-label"></label>
                        <div className="col-sm-9">
                            <button className="btn btn-primary">Save</button>
                        </div>
                    </div>
                </form>
            </Modal.Body>
        </Modal>
    );

    const importExcelModal = (
        <Modal show={importExcelModalOpen} onHide={importExcelCanceled}>
            <Modal.Header closeButton>
                <Modal.Title>Import Excel</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form onSubmit={importExcelConfirmed}>
                    <div className="mb-3 row">
                        <div className="col-sm-12">
                            <input
                                id="importingFile"
                                type="file"
                                name="importingFile"
                                className={
                                    "form-control " + (importExcelFormSubmitted && !importingFile ? "is-invalid" : "")
                                }
                                onChange={fileChanged}
                            />
                            <span className="invalid-feedback"> Select a file </span>
                        </div>
                    </div>
                    <div className="mb-3 row">
                        <div className="col-sm-12" style={{ textAlign: "center" }}>
                            <button className="btn btn-primary">Import</button>
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
                    <div style={{ float: "right" }}>
                        <button type="button" className="btn btn-secondary" onClick={exportAsExcel}>
                            Export as Excel
                        </button>
                        &nbsp;
                        <button className="btn btn-primary" onClick={() => openAddModal()}>
                            Add
                        </button>
                        &nbsp;
                        <button className="btn btn-primary" onClick={() => openImportExcelModal()}>
                            Import Excel
                        </button>
                    </div>
                </div>
                <div className="card-body">
                    <div className="table-responsive">{table}</div>
                </div>
            </div>
            {props.errorMessage ? (
                <div className="alert alert-danger">Error: {props.errorMessage}</div>
            ) : null}
            {deleteModal}
            {addUpdateModal}
            {importExcelModal}
        </div>
    );
};

const mapStateToProps = (state) => {
    return {
        configurationEntries: state.configurationEntry.configurationEntries,
        saved: state.configurationEntry.saved,
    };
};

const mapDispatchToProps = (dispatch) => {
    return {
        fetchConfigurationEntries: () => dispatch(actions.fetchConfigurationEntries()),
        saveConfigurationEntry: (entry) => dispatch(actions.saveConfigurationEntry(entry)),
        deleteConfigurationEntry: (entry) => dispatch(actions.deleteConfigurationEntry(entry)),
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(ListConfigurationEntries);
