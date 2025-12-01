"use client";

import { useState, useEffect } from "react";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

import { checkValidity } from "../../../shared/utility";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import axios from "../axios";

interface User {
  id?: string;
  userName: string;
  email: string;
  emailConfirmed: boolean;
  phoneNumber: string;
  phoneNumberConfirmed: boolean;
  twoFactorEnabled: boolean;
  lockoutEnabled: boolean;
  accessFailedCount: number;
  lockoutEnd: string;
}

interface ValidationError {
  required: boolean;
  minLength: boolean;
  isValid?: boolean;
}

interface ControlState {
  validation: {
    required: boolean;
    minLength: number;
  };
  error: ValidationError;
  valid: boolean;
  touched: boolean;
}

interface AppState {
  title: string;
  controls: {
    [key: string]: ControlState;
  };
  valid: boolean;
  submitted: boolean;
  errorMessage: string | null;
}

const AddUser = () => {
  const [state, setState] = useState<AppState>({
    title: "Add User",
    controls: {
      userName: {
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
      email: {
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
    },
    valid: false,
    submitted: false,
    errorMessage: null,
  });

  const { id } = useParams();
  const router = useRouter();

  const defaulUser = {
    userName: "",
    email: "",
    emailConfirmed: false,
    phoneNumber: "",
    phoneNumberConfirmed: false,
    twoFactorEnabled: false,
    lockoutEnabled: false,
    accessFailedCount: 0,
    lockoutEnd: "",
  };

  const [user, setUser] = useState(defaulUser);

  const fetchUser = async (id: string) => {
    try {
      const response = await axios.get(id);
      setUser(response.data);
    } catch (error) {
      console.log(error);
    }
  };

  const saveUser = async (user: User) => {
    try {
      const response = user.id
        ? await axios.put(user.id, user)
        : await axios.post("", user);
      router.push("/users/" + response.data.id);
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    if (id && typeof id === "string") {
      fetchUser(id);
      setState({ ...state, title: "Edit User" });
    }
  }, []);

  const fieldChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    let value: string | boolean | number = event.target.value;

    if (event.target.type === "checkbox") {
      value = event.target.checked;
    }

    checkFieldValidity(event.target.name, value);

    setUser({
      ...user,
      [event.target.name]: value,
    });
  };

  const updateLockoutEnd = (dateTime: Date | null) => {
    setUser({
      ...user,
      lockoutEnd: dateTime ? dateTime.toISOString() : "",
    });
  };

  const checkFieldValidity = (
    name: string,
    value: string | boolean | number
  ) => {
    const control = state.controls[name];

    if (!control) return true;

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

  const onSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setState({ ...state, submitted: true });

    let isValid = true;
    for (const fieldName in state.controls) {
      const fieldValue = user[fieldName as keyof typeof user];
      isValid = checkFieldValidity(fieldName, fieldValue) && isValid;
    }

    if (isValid) {
      saveUser({
        ...user,
        accessFailedCount: user.accessFailedCount
          ? parseInt(user.accessFailedCount.toString())
          : 0,
        lockoutEnd: user.lockoutEnd ? user.lockoutEnd : "",
      });
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
            <label htmlFor="userName" className="col-sm-3 col-form-label">
              User Name
            </label>
            <div className="col-sm-9">
              <input
                id="userName"
                name="userName"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["userName"].valid
                    ? "is-invalid"
                    : "")
                }
                value={user?.userName}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["userName"].error.required ? (
                  <span>Enter an user name</span>
                ) : null}
                {state.controls["userName"].error.minLength ? (
                  <span>The user name must be longer than 3 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="email" className="col-sm-3 col-form-label">
              Email
            </label>
            <div className="col-sm-9">
              <input
                id="email"
                name="email"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["email"].valid
                    ? "is-invalid"
                    : "")
                }
                value={user?.email}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["email"].error.required ? (
                  <span>Enter an email</span>
                ) : null}
                {state.controls["email"].error.minLength ? (
                  <span>The email must be longer than 3 characters.</span>
                ) : null}
              </span>
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="emailConfirmed" className="col-sm-3 col-form-label">
              Email Confirmed
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="emailConfirmed"
                name="emailConfirmed"
                checked={user?.emailConfirmed}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="phoneNumber" className="col-sm-3 col-form-label">
              Phone Number
            </label>
            <div className="col-sm-9">
              <input
                id="phoneNumber"
                name="phoneNumber"
                className="form-control"
                value={user?.phoneNumber}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label
              htmlFor="phoneNumberConfirmed"
              className="col-sm-3 col-form-label"
            >
              Phone Number Confirmed
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="phoneNumberConfirmed"
                name="phoneNumberConfirmed"
                checked={user?.phoneNumberConfirmed}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label
              htmlFor="twoFactorEnabled"
              className="col-sm-3 col-form-label"
            >
              Two Factor Enabled
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="twoFactorEnabled"
                name="twoFactorEnabled"
                checked={user?.twoFactorEnabled}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="lockoutEnabled" className="col-sm-3 col-form-label">
              Lockout Enabled
            </label>
            <div className="col-sm-9">
              <input
                type="checkbox"
                id="lockoutEnabled"
                name="lockoutEnabled"
                checked={user?.lockoutEnabled}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label
              htmlFor="accessFailedCount"
              className="col-sm-3 col-form-label"
            >
              Access Failed Count
            </label>
            <div className="col-sm-9">
              <input
                type="number"
                id="accessFailedCount"
                name="accessFailedCount"
                className="form-control"
                value={user?.accessFailedCount}
                onChange={(event) => fieldChanged(event)}
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label htmlFor="lockoutEnd" className="col-sm-3 col-form-label">
              Lockout End
            </label>
            <div className="col-sm-9">
              <DatePicker
                id="lockoutEnd"
                name="lockoutEnd"
                className="form-control"
                autoComplete="off"
                selected={user?.lockoutEnd ? new Date(user?.lockoutEnd) : null}
                onChange={(date) => updateLockoutEnd(date)}
                timeInputLabel="Time:"
                dateFormat="MM/dd/yyyy h:mm aa"
                showTimeInput
              />
            </div>
          </div>
          <div className="mb-3 row">
            <label
              htmlFor="description"
              className="col-sm-3 col-form-label"
            ></label>
            <div className="col-sm-9">
              <button className="btn btn-primary">Save</button>
            </div>
          </div>
        </form>
      </div>
      <div className="card-footer">
        <Link
          className="btn btn-outline-secondary"
          href="/users"
          style={{ width: "80px" }}
        >
          <i className="fa fa-chevron-left"></i> Back
        </Link>
      </div>
    </div>
  );

  return form;
};

export default AddUser;
