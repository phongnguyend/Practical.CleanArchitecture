import React, { Component } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import logo from "../../../logo.svg";
import * as actions from "../actions";
import Star from "../../../components/Star/Star";
import axios from "../axios";

class ListProducts extends Component<any, any> {
  state = {
    pageTitle: "Product List",
    showImage: false,
    deleteModalOpen: false,
    deletingProduct: {
      name: null,
    },
    listFilter: "",
    auditLogsModalOpen: false,
    importCsvModalOpen: false,
    importingFile: null as File | null,
    importCsvFormSubmitted: false,
  };

  toggleImage = () => {
    this.setState({ showImage: !this.state.showImage });
  };

  filterChanged = (event) => {
    this.setState({ listFilter: event.target.value });
  };

  performFilter(filterBy) {
    filterBy = filterBy.toLocaleLowerCase();
    return this.props.products.filter(
      (product) => product.name.toLocaleLowerCase().indexOf(filterBy) !== -1
    );
  }

  onRatingClicked = (event) => {
    const pageTitle = "Product List: " + event;
    this.setState({ pageTitle: pageTitle });
  };

  viewAuditLogs = (product) => {
    this.props.fetchAuditLogs(product);
    this.setState({ auditLogsModalOpen: true });
  };

  exportAsPdf = async () => {
    const rs = await axios.get("/ExportAsPdf", { responseType: "blob" });
    const url = window.URL.createObjectURL(rs.data);
    const element = document.createElement("a");
    element.href = url;
    element.download = "Products.pdf";
    document.body.appendChild(element);
    element.click();
  };

  exportAsCsv = async () => {
    const rs = await axios.get("/ExportAsCsv", { responseType: "blob" });
    const url = window.URL.createObjectURL(rs.data);
    const element = document.createElement("a");
    element.href = url;
    element.download = "Products.csv";
    document.body.appendChild(element);
    element.click();
  };

  deleteProduct = (product) => {
    this.setState({ deleteModalOpen: true, deletingProduct: product });
  };

  deleteCanceled = () => {
    this.setState({ deleteModalOpen: false, deletingProduct: null });
  };

  deleteConfirmed = () => {
    this.props.deleteProduct(this.state.deletingProduct);
    this.setState({ deleteModalOpen: false, deletingProduct: null });
  };

  formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  openImportCsvModal = () => {
    this.setState({
      importCsvModalOpen: true,
      importingFile: null,
      importCsvFormSubmitted: false,
    });
  };

  importCsvCanceled = () => {
    this.setState({
      importCsvModalOpen: false,
      importingFile: null,
      importCsvFormSubmitted: false,
    });
  };

  fileChanged = (event) => {
    this.setState({
      importingFile: event.target.files.item(0),
    });
  };

  importCsvConfirmed = async (event) => {
    event.preventDefault();
    this.setState({
      importCsvFormSubmitted: true,
    });
    if (!this.state.importingFile) {
      return;
    }
    const formData = new FormData();
    formData.append("formFile", this.state.importingFile);
    await axios.post("ImportCsv", formData);
    this.setState({
      importCsvModalOpen: false,
      importingFile: null,
      importCsvFormSubmitted: false,
    });

    this.props.fetchProducts();
  };

  componentDidMount() {
    this.props.fetchProducts();
  }

  render() {
    const filteredProducts = this.state.listFilter
      ? this.performFilter(this.state.listFilter)
      : this.props.products;

    const rows = filteredProducts?.map((product) => (
      <tr key={product.id}>
        <td>
          {this.state.showImage ? (
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
          <Star
            rating={product.starRating || 4}
            ratingClicked={(event) => this.onRatingClicked(event)}
          ></Star>
        </td>
        <td>
          <NavLink className="btn btn-primary" to={"/products/edit/" + product.id}>
            Edit
          </NavLink>
          &nbsp;
          <button
            type="button"
            className="btn btn-primary btn-secondary"
            onClick={() => this.viewAuditLogs(product)}
          >
            View Audit Logs
          </button>
          &nbsp;
          <button
            type="button"
            className="btn btn-primary btn-danger"
            onClick={() => this.deleteProduct(product)}
          >
            Delete
          </button>
        </td>
      </tr>
    ));

    const table = this.props.products ? (
      <table className="table">
        <thead>
          <tr>
            <th>
              <button className="btn btn-primary" onClick={this.toggleImage}>
                {this.state.showImage ? "Hide" : "Show"} Image
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
    const auditLogRows = this.props.auditLogs?.map((auditLog) => (
      <tr key={auditLog.id}>
        <td>{this.formatDateTime(auditLog.createdDateTime)}</td>
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
      <Modal
        size="xl"
        show={this.state.auditLogsModalOpen}
        onHide={() => this.setState({ auditLogsModalOpen: false })}
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
      <Modal show={this.state.deleteModalOpen} onHide={this.deleteCanceled}>
        <Modal.Header closeButton>
          <Modal.Title>Delete Product</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to delete
          <strong> {this.state.deletingProduct?.name}</strong>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={this.deleteCanceled}>
            No
          </Button>
          <Button variant="primary" onClick={this.deleteConfirmed}>
            Yes
          </Button>
        </Modal.Footer>
      </Modal>
    );

    const importCsvModal = (
      <Modal show={this.state.importCsvModalOpen} onHide={this.importCsvCanceled}>
        <Modal.Header closeButton>
          <Modal.Title>Import Csv</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <form onSubmit={this.importCsvConfirmed}>
            <div className="mb-3 row">
              <div className="col-sm-12">
                <input
                  id="importingFile"
                  type="file"
                  name="importingFile"
                  className={
                    "form-control " +
                    (this.state.importCsvFormSubmitted && !this.state.importingFile
                      ? "is-invalid"
                      : "")
                  }
                  onChange={this.fileChanged}
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
            {this.state.pageTitle}
            <div style={{ float: "right" }}>
              <button type="button" className="btn btn-secondary" onClick={this.exportAsPdf}>
                Export as Pdf
              </button>
              &nbsp;
              <button type="button" className="btn btn-secondary" onClick={this.exportAsCsv}>
                Export as Csv
              </button>
              &nbsp;
              <NavLink className="btn btn-primary" to="/products/add">
                Add Product
              </NavLink>
              &nbsp;
              <button className="btn btn-primary" onClick={() => this.openImportCsvModal()}>
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
                  value={this.state.listFilter}
                  onChange={(event) => this.filterChanged(event)}
                />
              </div>
            </div>
            {this.state.listFilter ? (
              <div className="row">
                <div className="col-md-6">
                  <h4>Filtered by: {this.state.listFilter}</h4>
                </div>
              </div>
            ) : null}
            <div className="table-responsive">{table}</div>
          </div>
        </div>
        {this.props.errorMessage ? (
          <div className="alert alert-danger">Error: {this.props.errorMessage}</div>
        ) : null}
        {deleteModal}
        {auditLogsModal}
        {importCsvModal}
      </div>
    );
  }
}

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
