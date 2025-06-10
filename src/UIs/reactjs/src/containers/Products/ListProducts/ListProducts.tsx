import React, { useState, useEffect } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import logo from "../../../logo.svg";
import * as actions from "../actions";
import Star from "../../../components/Star/Star";
import axios from "../axios";

const ListProducts = (props: any) => {
  const [pageTitle, setPageTitle] = useState("Product List");
  const [showImage, setShowImage] = useState(false);
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [deletingProduct, setDeletingProduct] = useState({
    name: null,
  });
  const [listFilter, setListFilter] = useState("");
  const [auditLogsModalOpen, setAuditLogsModalOpen] = useState(false);
  const [importCsvModalOpen, setImportCsvModalOpen] = useState(false);
  const [importingFile, setImportingFile] = useState<File | null>(null);
  const [importCsvFormSubmitted, setImportCsvFormSubmitted] = useState(false);

  const toggleImage = () => {
    setShowImage(!showImage);
  };

  const filterChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    setListFilter(event.target.value);
  };

  const performFilter = (filterBy: string) => {
    filterBy = filterBy.toLocaleLowerCase();
    return props.products.filter(
      (product: any) => product.name.toLocaleLowerCase().indexOf(filterBy) !== -1
    );
  };

  const onRatingClicked = (event: string) => {
    const pageTitle = "Product List: " + event;
    setPageTitle(pageTitle);
  };

  const viewAuditLogs = (product: any) => {
    props.fetchAuditLogs(product);
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

  const deleteProduct = (product: any) => {
    setDeleteModalOpen(true);
    setDeletingProduct(product);
  };

  const deleteCanceled = () => {
    setDeleteModalOpen(false);
    setDeletingProduct({ name: null });
  };

  const deleteConfirmed = () => {
    props.deleteProduct(deletingProduct);
    setDeleteModalOpen(false);
    setDeletingProduct({ name: null });
  };

  const formatDateTime = (value: string) => {
    if (!value) return value;
    var date = new Date(value);
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

  const importCsvConfirmed = async (event: React.FormEvent) => {
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

    props.fetchProducts();
  };

  useEffect(() => {
    props.fetchProducts();
  }, []);

  const filteredProducts = listFilter ? performFilter(listFilter) : props.products;

  const rows = filteredProducts?.map((product: any) => (
    <tr key={product.id}>
      <td>
        {showImage ? (
          <img
            alt=""
            src={product.imageUrl || logo}
            title={product.name}
            style={{ width: "50px", margin: "2px" }}
          />
        ) : null}
      </td>
      <td>
        <NavLink to={"/products/" + product.id}>{product.name}</NavLink>
      </td>
      <td>{product.code?.toLocaleUpperCase()}</td>
      <td>{product.description}</td>
      <td>{product.price || (5).toFixed(2)}</td>
      <td>
        <Star rating={product.starRating || 4} ratingClicked={onRatingClicked}></Star>
      </td>
      <td>
        <NavLink className="btn btn-primary" to={"/products/edit/" + product.id}>
          Edit
        </NavLink>
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

  const table = filteredProducts ? (
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

  const auditLogsRows = props.auditLogs?.map((auditLog: any) => (
    <tr key={auditLog.id}>
      <td>{formatDateTime(auditLog.createdDateTime)}</td>
      <td>{auditLog.userName}</td>
      <td>{auditLog.action}</td>
      <td style={{ color: auditLog.highLight.code ? "red" : "" }}>{auditLog.data.code}</td>
      <td style={{ color: auditLog.highLight.name ? "red" : "" }}>{auditLog.data.name}</td>
      <td style={{ color: auditLog.highLight.description ? "red" : "" }}>
        {auditLog.data.description}
      </td>
    </tr>
  ));

  const auditLogsModal = (
    <Modal size="xl" show={auditLogsModalOpen} onHide={() => setAuditLogsModalOpen(false)}>
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
            <tbody>{auditLogsRows}</tbody>
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
                  "form-control " + (importCsvFormSubmitted && !importingFile ? "is-invalid" : "")
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
            <button type="button" className="btn btn-secondary" onClick={exportAsPdf}>
              Export as Pdf
            </button>
            &nbsp;
            <button type="button" className="btn btn-secondary" onClick={exportAsCsv}>
              Export as Csv
            </button>
            &nbsp;
            <NavLink className="btn btn-primary" to="/products/add">
              Add Product
            </NavLink>
            &nbsp;
            <button className="btn btn-primary" onClick={() => openImportCsvModal()}>
              Import Csv
            </button>
          </div>
        </div>
        <div className="card-body">
          <div className="row">
            <div className="col-md-2">Filter by:</div>
            <div className="col-md-4">
              <input type="text" value={listFilter} onChange={(event) => filterChanged(event)} />
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
      {props.errorMessage ? (
        <div className="alert alert-danger">Error: {props.errorMessage}</div>
      ) : null}
      {deleteModal}
      {auditLogsModal}
      {importCsvModal}
    </div>
  );
};

const mapStateToProps = (state) => {
  return {
    products: state.product.products,
    auditLogs: state.product.auditLogs,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchProducts: () => dispatch(actions.fetchProducts()),
    deleteProduct: (product) => dispatch(actions.deleteProduct(product)),
    fetchAuditLogs: (product) => dispatch(actions.fetchAuditLogs(product)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ListProducts);
