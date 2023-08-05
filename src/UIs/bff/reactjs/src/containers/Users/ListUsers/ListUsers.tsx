import React, { Component } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import * as actions from "../actions";

class ListUsers extends Component<any, any> {
  state = {
    pageTitle: "Users",
    showDeleteModal: false,
    deletingUser: {
      userName: null
    },
  };

  deleteUser = (user) => {
    this.setState({ showDeleteModal: true, deletingUser: user });
  };

  deleteCanceled = () => {
    this.setState({ showDeleteModal: false, deletingUser: null });
  };

  deleteConfirmed = () => {
    this.props.deleteUser(this.state.deletingUser);
    this.setState({ showDeleteModal: false, deletingUser: null });
  };

  formatDateTime = (value) => {
    if (!value) return value;
    var date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  componentDidMount() {
    this.props.fetchUsers();
  }

  render() {
    const rows = this.props.users?.map((user) => (
      <tr key={user.id}>
        <td>
          <NavLink to={"/users/" + user.id}>{user.userName}</NavLink>
        </td>
        <td>{user.email}</td>
        <td>
          <NavLink className="btn btn-primary" to={"/users/edit/" + user.id}>
            Edit
          </NavLink>
          &nbsp;
          <button
            type="button"
            className="btn btn-primary btn-danger"
            onClick={() => this.deleteUser(user)}
          >
            Delete
          </button>
        </td>
      </tr>
    ));

    const table = this.props.users ? (
      <table className="table">
        <thead>
          <tr>
            <th>User Name</th>
            <th>Email</th>
            <th></th>
          </tr>
        </thead>
        <tbody>{rows}</tbody>
      </table>
    ) : null;

    const deleteModal = (
      <Modal show={this.state.showDeleteModal} onHide={this.deleteCanceled}>
        <Modal.Header closeButton>
          <Modal.Title>Delete User</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to delete
          <strong> {this.state.deletingUser?.userName}</strong>
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

    return (
      <div>
        <div className="card">
          <div className="card-header">
            {this.state.pageTitle}
            <NavLink
              className="btn btn-primary"
              style={{ float: "right" }}
              to="/users/add"
            >
              Add User
            </NavLink>
          </div>
          <div className="card-body">
            <div className="table-responsive">{table}</div>
          </div>
        </div>
        {this.props.errorMessage ? (
          <div className="alert alert-danger">
            Error: {this.props.errorMessage}
          </div>
        ) : null}
        {deleteModal}
      </div>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    users: state.user.users,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    fetchUsers: () => dispatch(actions.fetchUsers()),
    deleteUser: (user) => dispatch(actions.deleteUser(user)),
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(ListUsers);
