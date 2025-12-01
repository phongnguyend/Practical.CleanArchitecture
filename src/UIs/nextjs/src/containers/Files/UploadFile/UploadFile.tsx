"use client";

import React, { useState } from "react";
import { checkValidity } from "../../../shared/utility";
import axios from "../axios";
import Link from "next/link";
import { useRouter } from "next/navigation";

const UploadFile = () => {
  const router = useRouter();

  const [state, setState] = useState({
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
    formFile: null as File | null,
    hasFile: false,
    submitted: false,
    errorMessage: null as string | null,
    saved: false,
  });

  const [file, setFile] = useState({
    name: "",
    description: "",
    fileName: "",
    encrypted: false,
  });

  const fieldChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    let value: string | boolean = event.target.value;

    if (event.target.type == "checkbox") {
      value = event.target.checked;
    }

    checkFieldValidity(event.target.name as keyof typeof state.controls, value);

    setFile({
      ...file,
      [event.target.name]: value,
    });
  };

  const checkFieldValidity = (
    name: keyof typeof state.controls,
    value: string | boolean
  ) => {
    const control = state.controls[name];
    if (!control) return true;

    const rules = control.validation;
    const validationRs = checkValidity(value, rules);

    setState((preState) => {
      return {
        ...preState,
        controls: {
          ...preState.controls,
          [name]: {
            ...preState.controls[name as keyof typeof preState.controls],
            error: validationRs,
            valid: validationRs.isValid,
          },
        },
      };
    });

    return validationRs.isValid;
  };

  const handleFileInput = (event: React.ChangeEvent<HTMLInputElement>) => {
    var formFile = event.target.files?.item(0) || null;
    setState({ ...state, formFile: formFile, hasFile: !!formFile });
    setFile({
      ...file,
      [event.target.name]: formFile,
    });
  };

  const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setState({ ...state, submitted: true });
    let isValid = true;
    for (let fieldName in state.controls) {
      isValid =
        checkFieldValidity(
          fieldName as keyof typeof state.controls,
          file[fieldName as keyof typeof file]
        ) && isValid;
    }

    if (isValid && state.hasFile) {
      try {
        const formData = new FormData();
        formData.append("formFile", state.formFile as File);
        formData.append("name", file.name);
        formData.append("description", file.description);
        formData.append("encrypted", file.encrypted.toString());
        const response = await axios.post("", formData);
        setFile(response.data);
        setState({ ...state, saved: true });

        router.push("/files/edit/" + response.data.id);
      } catch (error) {
        console.log(error);
      }
    }
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
                type="file"
                name="formFile"
                className={
                  "form-control " +
                  (state.submitted && !state.hasFile ? "is-invalid" : "")
                }
                onChange={handleFileInput}
              />
              <span className="invalid-feedback">Select a file</span>
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
                checked={file?.encrypted}
                onChange={(event) => fieldChanged(event)}
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
      </div>
    </div>
  );

  return form;
};

export default UploadFile;
