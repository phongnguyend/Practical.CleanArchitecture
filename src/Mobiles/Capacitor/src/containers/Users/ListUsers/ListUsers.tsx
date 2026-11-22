import PageHeader from "../../../components/PageHeader/PageHeader";
import PagedTable from "../../../components/PagedTable/PagedTable";
import React, { useState, useEffect } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Button } from "react-bootstrap";
import Modal from "../../../components/Modal/Modal";

import * as actions from "../actions";
import ActionIcon from "../../../components/ActionIcon/ActionIcon";

const ListUsers = (props: any) => {
  const [pageTitle] = useState("Users");
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deletingUser, setDeletingUser] = useState<any>({
    userName: null,
  });

  const deleteUser = (user: any) => {
    setShowDeleteModal(true);
    setDeletingUser(user);
  };

  const deleteCanceled = () => {
    setShowDeleteModal(false);
    setDeletingUser({ userName: null });
  };

  const deleteConfirmed = () => {
    props.deleteUser(deletingUser);
    setShowDeleteModal(false);
    setDeletingUser({ userName: null });
  };

  useEffect(() => {
    props.fetchUsers();
  }, []);

  const rows = props.users?.map((user: any) => (
    <tr key={user.id}>
      <td>
        <NavLink to={"/users/" + user.id}>
          <ActionIcon action="view" />
          {user.userName}
        </NavLink>
      </td>
      <td>{user.email}</td>
      <td>
        <NavLink className="btn btn-primary" to={"/users/edit/" + user.id}>
          <ActionIcon action="edit" /> Edit
        </NavLink>

        <button
          type="button"
          className="btn btn-danger"
          onClick={() => deleteUser(user)}
        >
          <ActionIcon action="delete" /> Delete
        </button>
      </td>
    </tr>
  ));

  const table = props.users ? (
    <PagedTable searchable={true}>
      <thead>
        <tr>
          <th>User Name</th>
          <th>Email</th>
          <th></th>
        </tr>
      </thead>
      <tbody>{rows}</tbody>
    </PagedTable>
  ) : null;

  const deleteModal = (
    <Modal show={showDeleteModal} onHide={deleteCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>
          <ActionIcon action="delete" />
          Delete User
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        Are you sure you want to delete
        <strong> {deletingUser?.userName}</strong>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={deleteCanceled}>
          <ActionIcon action="cancel" /> No
        </Button>
        <Button variant="primary" onClick={deleteConfirmed}>
          <ActionIcon action="confirm" /> Yes
        </Button>
      </Modal.Footer>
    </Modal>
  );

  return (
    <div>
      <div className="card">
        <PageHeader title={pageTitle} icon="users" onRefresh={props.fetchUsers}>
          <NavLink
            className="btn btn-primary"

            to="/users/add"
          >
            <ActionIcon action="add" /> Add new
          </NavLink>
        </PageHeader>
        <div className="card-body">
          <div className="table-responsive">{table}</div>
        </div>
      </div>
      {props.errorMessage ? (
        <div className="alert alert-danger">Error: {props.errorMessage}</div>
      ) : null}
      {deleteModal}
    </div>
  );
};

const mapStateToProps = (state: any) => {
  return {
    users: state.user.users,
  };
};

const mapDispatchToProps = (dispatch: any) => {
  return {
    fetchUsers: () => dispatch(actions.fetchUsers()),
    deleteUser: (user: any) => dispatch(actions.deleteUser(user)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ListUsers);
