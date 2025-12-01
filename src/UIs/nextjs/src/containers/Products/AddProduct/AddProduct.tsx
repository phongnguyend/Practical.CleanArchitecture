"use client";

import React, { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter, useParams } from "next/navigation";

import { checkValidity } from "../../../shared/utility";
import axios from "../axios";

interface Product {
  id?: string;
  name: string;
  description: string;
  [key: string]: any;
}

const AddProduct = () => {
  const [state, setState] = useState({
    title: "Add Product",
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
      code: {
        validation: {
          required: true,
          maxLength: 10,
        },
        error: {
          required: false,
          maxLength: false,
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
    errorMessage: null,
  });

  const [product, setProduct] = useState<Product>({
    name: "",
    description: "",
  });

  const { id } = useParams();
  const router = useRouter();

  const fetchProduct = async (id: string) => {
    try {
      const response = await axios.get(id);
      setProduct(response.data);
    } catch (error) {
      console.log(error);
    }
  };

  const saveProduct = async (product: Product) => {
    try {
      const response = product.id
        ? await axios.put(product.id, product)
        : await axios.post("", product);

      router.push("/products/" + response.data.id);
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    if (id && typeof id === "string") {
      setState({ ...state, title: "Edit Product" });
      fetchProduct(id);
    }
  }, []);

  const fieldChanged = (
    event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    checkFieldValidity(
      event.target.name as keyof typeof state.controls,
      event.target.value
    );
    setProduct({
      ...product,
      [event.target.name]: event.target.value,
    });
  };

  const checkFieldValidity = (
    name: keyof typeof state.controls,
    value: string
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

  const onSubmit = (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setState({ ...state, submitted: true });
    let isValid = true;
    for (const fieldName in state.controls) {
      isValid =
        checkFieldValidity(
          fieldName as keyof typeof state.controls,
          product[fieldName]
        ) && isValid;
    }

    if (isValid) {
      saveProduct(product);
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
                value={product?.name}
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
            <label htmlFor="code" className="col-sm-2 col-form-label">
              Code
            </label>
            <div className="col-sm-10">
              <input
                id="code"
                name="code"
                className={
                  "form-control " +
                  (state.submitted && !state.controls["code"].valid
                    ? "is-invalid"
                    : "")
                }
                value={product?.code}
                onChange={(event) => fieldChanged(event)}
              />
              <span className="invalid-feedback">
                {state.controls["code"].error.required ? (
                  <span>Enter a code</span>
                ) : null}
                {state.controls["code"].error.maxLength ? (
                  <span>The code must be less than 10 characters.</span>
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
                value={product?.description}
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
          href="/products"
          style={{ width: "80px" }}
        >
          <i className="fa fa-chevron-left"></i> Back
        </Link>
      </div>
    </div>
  );

  return form;
};

export default AddProduct;
