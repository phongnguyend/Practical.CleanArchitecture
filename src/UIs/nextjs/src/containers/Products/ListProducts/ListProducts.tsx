"use client";

import React, { useState, useEffect } from "react";
import Image from "next/image";
import Link from "next/link";
import { Modal, Button } from "react-bootstrap";

import Star from "../../../components/Star/Star";
import axios from "../axios";

interface Product {
  id: string;
  name: string;
  code: string;
  description: string;
  [key: string]: any;
}

interface AuditLog {
  id: string;
  action: string;
  objectId: string;
  log: string;
  createdDateTime: string;
  createdBy: string;
  userName: string;
  highLight: {
    code: boolean;
    name: boolean;
    description: boolean;
  };
  data: {
    code: string;
    name: string;
    description: string;
  };
}

const ListProducts = () => {
  const [pageTitle, setPageTitle] = useState<string>("Product List");
  const [products, setProducts] = useState<Product[]>([]);
  const [auditLogs, setAuditLogs] = useState<AuditLog[]>([]);
  const [showImage, setShowImage] = useState<boolean>(false);
  const [listFilter, setListFilter] = useState<string>("");
  const [auditLogsModalOpen, setAuditLogsModalOpen] = useState<boolean>(false);
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);
  const [deletingProduct, setDeletingProduct] = useState<Product | null>(null);
  const [importCsvModalOpen, setImportCsvModalOpen] = useState<boolean>(false);
  const [importingFile, setImportingFile] = useState<File | null>(null);
  const [importCsvFormSubmitted, setImportCsvFormSubmitted] =
    useState<boolean>(false);

  const toggleImage = () => {
    setShowImage(!showImage);
  };

  const filterChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    setListFilter(event.target.value);
  };

  const performFilter = (filterBy: string) => {
    filterBy = filterBy.toLocaleLowerCase();
    return products.filter(
      (product: Product) =>
        product.name.toLocaleLowerCase().indexOf(filterBy) !== -1
    );
  };

  const onRatingClicked = (event: string) => {
    const newPageTitle = "Product List: " + event;
    setPageTitle(newPageTitle);
  };

  const viewAuditLogs = async (product: Product) => {
    try {
      const response = await axios.get(product.id + "/auditLogs");
      const fetchedAuditLogs = response.data;
      setAuditLogs(fetchedAuditLogs);
    } catch (error) {
      console.log(error);
    }
    setAuditLogsModalOpen(true);
  };

  const exportAsPdf = async () => {
    const rs = await axios.get("/ExportAsPdf", { responseType: "blob" });
    const url = window.URL.createObjectURL(rs.data);
    const element = document.createElement("a");
    element.href = url;
    element.download = "Products.pdf";
    document.body.appendChild(element);
    element.click();
  };

  const exportAsCsv = async () => {
    const rs = await axios.get("/ExportAsCsv", { responseType: "blob" });
    const url = window.URL.createObjectURL(rs.data);
    const element = document.createElement("a");
    element.href = url;
    element.download = "Products.csv";
    document.body.appendChild(element);
    element.click();
  };

  const deleteProduct = (product: Product) => {
    setDeleteModalOpen(true);
    setDeletingProduct(product);
  };

  const deleteCanceled = () => {
    setDeleteModalOpen(false);
    setDeletingProduct(null);
  };

  const deleteConfirmed = async () => {
    const product = deletingProduct;
    if (product) {
      try {
        await axios.delete(product.id);
        fetchProducts();
      } catch (error) {
        console.log(error);
      }
    }
    setDeleteModalOpen(false);
    setDeletingProduct(null);
  };

  const formatDateTime = (value: string | null | undefined) => {
    if (!value) return value;
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  const openImportCsvModal = () => {
    setImportCsvModalOpen(true);
    setImportingFile(null);
    setImportCsvFormSubmitted(false);
  };

  const importCsvCanceled = () => {
    setImportCsvModalOpen(false);
    setImportingFile(null);
    setImportCsvFormSubmitted(false);
  };

  const fileChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    setImportingFile(event.target.files?.item(0) || null);
  };

  const importCsvConfirmed = async (
    event: React.FormEvent<HTMLFormElement>
  ) => {
    event.preventDefault();
    setImportCsvFormSubmitted(true);
    if (!importingFile) {
      return;
    }
    const formData = new FormData();
    formData.append("formFile", importingFile);
    await axios.post("ImportCsv", formData);
    setImportCsvModalOpen(false);
    setImportingFile(null);
    setImportCsvFormSubmitted(false);

    fetchProducts();
  };

  const fetchProducts = async () => {
    try {
      const response = await axios.get("");
      const fetchedProducts = response.data;
      setProducts(fetchedProducts);
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    fetchProducts();
  }, []);

  const filteredProducts = listFilter ? performFilter(listFilter) : products;

  const rows = filteredProducts?.map((product: Product) => (
    <tr key={product.id}>
      <td>
        {showImage ? (
          <Image
            src={product.imageUrl || "next.svg"}
            alt=""
            className="img-responsive center-block"
            width={50}
            height={50}
            title={product.name}
            style={{ width: "50px", margin: "2px" }}
          />
        ) : null}
      </td>
      <td>
        <Link href={"/products/" + product.id}>{product.name}</Link>
      </td>
      <td>{product.code?.toLocaleUpperCase()}</td>
      <td>{product.description}</td>
      <td>{product.price || (5).toFixed(2)}</td>
      <td>
        <Star
          rating={product.starRating || 4}
          ratingClicked={(event) => onRatingClicked(event)}
        ></Star>
      </td>
      <td>
        <Link className="btn btn-primary" href={"/products/edit/" + product.id}>
          Edit
        </Link>
        &nbsp;
        <button
          type="button"
          className="btn btn-primary btn-secondary"
          onClick={() => viewAuditLogs(product)}
        >
          View Audit Logs
        </button>
        &nbsp;
        <button
          type="button"
          className="btn btn-primary btn-danger"
          onClick={() => deleteProduct(product)}
        >
          Delete
        </button>
      </td>
    </tr>
  ));

  const table = products ? (
    <table className="table">
      <thead>
        <tr>
          <th>
            <button className="btn btn-primary" onClick={toggleImage}>
              {showImage ? "Hide" : "Show"} Image
            </button>
          </th>
          <th>Product</th>
          <th>Code</th>
          <th>Description</th>
          <th>Price</th>
          <th>5 Star Rating</th>
          <th></th>
        </tr>
      </thead>
      <tbody>{rows}</tbody>
    </table>
  ) : null;
  const auditLogRows = auditLogs?.map((auditLog: AuditLog) => (
    <tr key={auditLog.id}>
      <td>{formatDateTime(auditLog.createdDateTime)}</td>
      <td>{auditLog.userName}</td>
      <td>{auditLog.action}</td>
      <td style={{ color: auditLog.highLight.code ? "red" : "" }}>
        {auditLog.data.code}
      </td>
      <td style={{ color: auditLog.highLight.name ? "red" : "" }}>
        {auditLog.data.name}
      </td>
      <td style={{ color: auditLog.highLight.description ? "red" : "" }}>
        {auditLog.data.description}
      </td>
    </tr>
  ));
  const auditLogsModal = (
    <Modal
      size="xl"
      show={auditLogsModalOpen}
      onHide={() => setAuditLogsModalOpen(false)}
    >
      <Modal.Body>
        <div className="table-responsive">
          <table className="table">
            <thead>
              <tr>
                <th>Date Time</th>
                <th>User Name</th>
                <th>Action</th>
                <th>Code</th>
                <th>Name</th>
                <th>Description</th>
              </tr>
            </thead>
            <tbody>{auditLogRows}</tbody>
          </table>
        </div>
      </Modal.Body>
    </Modal>
  );

  const deleteModal = (
    <Modal show={deleteModalOpen} onHide={deleteCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>Delete Product</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        Are you sure you want to delete
        <strong> {deletingProduct?.name}</strong>
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

  const importCsvModal = (
    <Modal show={importCsvModalOpen} onHide={importCsvCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>Import Csv</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <form onSubmit={importCsvConfirmed}>
          <div className="mb-3 row">
            <div className="col-sm-12">
              <input
                id="importingFile"
                type="file"
                name="importingFile"
                className={
                  "form-control " +
                  (importCsvFormSubmitted && !importingFile ? "is-invalid" : "")
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
          {pageTitle}
          <div style={{ float: "right" }}>
            <button
              type="button"
              className="btn btn-secondary"
              onClick={exportAsPdf}
            >
              Export as Pdf
            </button>
            &nbsp;
            <button
              type="button"
              className="btn btn-secondary"
              onClick={exportAsCsv}
            >
              Export as Csv
            </button>
            &nbsp;
            <Link className="btn btn-primary" href="/products/add">
              Add Product
            </Link>
            &nbsp;
            <button
              className="btn btn-primary"
              onClick={() => openImportCsvModal()}
            >
              Import Csv
            </button>
          </div>
        </div>
        <div className="card-body">
          <div className="row">
            <div className="col-md-2">Filter by:</div>
            <div className="col-md-4">
              <input
                type="text"
                value={listFilter}
                onChange={(event) => filterChanged(event)}
              />
            </div>
          </div>
          {listFilter ? (
            <div className="row">
              <div className="col-md-6">
                <h4>Filtered by: {listFilter}</h4>
              </div>
            </div>
          ) : null}
          <div className="table-responsive">{table}</div>
        </div>
      </div>
      {deleteModal}
      {auditLogsModal}
      {importCsvModal}
    </div>
  );
};

export default ListProducts;
