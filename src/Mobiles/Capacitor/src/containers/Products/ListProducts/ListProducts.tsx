import AuditEntries from "../../../components/AuditEntries/AuditEntries";
import PageHeader from "../../../components/PageHeader/PageHeader";
import PagedTable from "../../../components/PagedTable/PagedTable";
import { downloadOrShare } from "../../../core/files/native-files";
import React, { useState, useEffect } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Button } from "react-bootstrap";
import Modal from "../../../components/Modal/Modal";

import logo from "../../../logo.svg";
import * as actions from "../actions";
import ActionIcon from "../../../components/ActionIcon/ActionIcon";
import FilePicker from "../../../components/FilePicker/FilePicker";
import Star from "../../../components/Star/Star";
import axios from "../axios";

// Preserve the search when an editor route returns to this list.
let lastFilter = "";

const ListProducts = (props: any) => {
  const [pageTitle, setPageTitle] = useState("Product List");
  const [showImage, setShowImage] = useState(false);
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [deletingProduct, setDeletingProduct] = useState<any>({
    name: null,
  });
  const [listFilter, setListFilter] = useState(lastFilter);
  const [auditLogsModalOpen, setAuditLogsModalOpen] = useState(false);
  const [importCsvModalOpen, setImportCsvModalOpen] = useState(false);
  const [importingFile, setImportingFile] = useState<File | null>(null);
  const [importCsvFormSubmitted, setImportCsvFormSubmitted] = useState(false);

  const toggleImage = () => {
    setShowImage(!showImage);
  };

  const filterChanged = (event: React.ChangeEvent<HTMLInputElement>) => {
    lastFilter = event.target.value;
    setListFilter(lastFilter);
  };

  const performFilter = (filterBy: string) => {
    filterBy = filterBy.toLocaleLowerCase();
    return props.products.filter(
      (product: any) =>
        product.name.toLocaleLowerCase().indexOf(filterBy) !== -1,
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
    await downloadOrShare(rs.data, "Products.pdf");
  };

  const exportAsCsv = async () => {
    const rs = await axios.get("/ExportAsCsv", { responseType: "blob" });
    await downloadOrShare(rs.data, "Products.csv");
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
    if (!value) {
      return value;
    }
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
    return () => {
      if (!window.location.pathname.startsWith("/products")) {
        lastFilter = "";
      }
    };
  }, []);

  const filteredProducts = listFilter
    ? performFilter(listFilter)
    : props.products;

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
        <NavLink to={"/products/" + product.id}>
          <ActionIcon action="view" />
          {product.name}
        </NavLink>
      </td>
      <td>{product.code?.toLocaleUpperCase()}</td>
      <td>{product.description}</td>
      <td>{product.price || (5).toFixed(2)}</td>
      <td>
        <Star
          rating={product.starRating || 4}
          ratingClicked={onRatingClicked}
        ></Star>
      </td>
      <td>
        <NavLink
          className="btn btn-primary"
          to={"/products/edit/" + product.id}
        >
          <ActionIcon action="edit" /> Edit
        </NavLink>

        <button
          type="button"
          className="btn btn-secondary"
          onClick={() => viewAuditLogs(product)}
        >
          <ActionIcon action="history" /> History
        </button>

        <button
          type="button"
          className="btn btn-danger"
          onClick={() => deleteProduct(product)}
        >
          <ActionIcon action="delete" /> Delete
        </button>
      </td>
    </tr>
  ));

  const table = filteredProducts ? (
    <PagedTable searchable={false}>
      <thead>
        <tr>
          <th>
            <button className="btn btn-primary" onClick={toggleImage}>
              <ActionIcon action={showImage ? "hide" : "show"} />{" "}
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
    </PagedTable>
  ) : null;

  const auditLogsModal = (
    <Modal
      size="xl"
      show={auditLogsModalOpen}
      onHide={() => setAuditLogsModalOpen(false)}
    >
      <Modal.Header closeButton>
        <Modal.Title>
          <ActionIcon action="audit" />
          Audit Logs
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <AuditEntries entries={props.auditLogs ?? []} fields={["code", "name", "description"]} />
      </Modal.Body>
    </Modal>
  );

  const deleteModal = (
    <Modal show={deleteModalOpen} onHide={deleteCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>
          <ActionIcon action="delete" />
          Confirm
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        Delete {deletingProduct?.name}? This cannot be undone.
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={deleteCanceled}>
          <ActionIcon action="cancel" /> Cancel
        </Button>
        <Button variant="danger" onClick={deleteConfirmed}>
          <ActionIcon action="confirm" /> Confirm
        </Button>
      </Modal.Footer>
    </Modal>
  );

  const importCsvModal = (
    <Modal show={importCsvModalOpen} onHide={importCsvCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>
          <ActionIcon action="import" />
          Import Csv
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <form id="listproducts-modal-form-1" onSubmit={importCsvConfirmed}>
          <div className="mb-3 row">
            <div className="col-sm-12">
              <FilePicker
                id="importingFile"
                name="importingFile"
                label="CSV file"
                hint="CSV files (.csv)"
                file={importingFile}
                invalid={importCsvFormSubmitted && !importingFile}
                onChange={fileChanged}
                accept=".csv,text/csv"
              />
            </div>
          </div>
        </form>
      </Modal.Body>
      <Modal.Footer>
        <button
          type="button"
          className="btn btn-secondary"
          onClick={importCsvCanceled}
        >
          Cancel
        </button>
        <button
          type="submit"
          form="listproducts-modal-form-1"
          className="btn btn-primary"
        >
          <ActionIcon action="import" /> Import
        </button>
      </Modal.Footer>
    </Modal>
  );

  return (
    <div>
      <div className="card">
        <PageHeader
          title={pageTitle}
          icon="products"
          onRefresh={props.fetchProducts}
        >
          <button
            type="button"
            className="btn btn-secondary"
            onClick={exportAsPdf}
          >
            <ActionIcon action="export" /> Export as Pdf
          </button>

          <button
            type="button"
            className="btn btn-secondary"
            onClick={exportAsCsv}
          >
            <ActionIcon action="export" /> Export as Csv
          </button>

          <NavLink className="btn btn-primary" to="/products/add">
            <ActionIcon action="add" /> Add new
          </NavLink>

          <button
            className="btn btn-primary"
            onClick={() => openImportCsvModal()}
          >
            <ActionIcon action="import" /> Import Csv
          </button>
        </PageHeader>
        <div className="card-body">
          <div className="row">
            <div className="col-md-2">Filter by:</div>
            <div className="col-md-4">
              <input
                className="form-control"
                aria-label="Filter records"
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
      {props.errorMessage ? (
        <div className="alert alert-danger">Error: {props.errorMessage}</div>
      ) : null}
      {deleteModal}
      {auditLogsModal}
      {importCsvModal}
    </div>
  );
};

const mapStateToProps = (state: any) => {
  return {
    products: state.product.products,
    auditLogs: state.product.auditLogs,
  };
};

const mapDispatchToProps = (dispatch: any) => {
  return {
    fetchProducts: () => dispatch(actions.fetchProducts()),
    deleteProduct: (product: any) => dispatch(actions.deleteProduct(product)),
    fetchAuditLogs: (product: any) => dispatch(actions.fetchAuditLogs(product)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ListProducts);
