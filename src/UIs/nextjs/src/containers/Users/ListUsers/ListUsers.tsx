"use client";

import Link from "next/link";
import React, { Component } from "react";
import { Modal, Button } from "react-bootstrap";
import axios from "../axios";

class ListUsers extends Component<any, any> {
  state = {
    pageTitle: "Users",
    users: [],
    showDeleteModal: false,
    deletingUser: {
      userName: null,
    },
  };

  deleteUser = (user: any) => {
    this.setState({ showDeleteModal: true, deletingUser: user });
  };

  deleteCanceled = () => {
    this.setState({ showDeleteModal: false, deletingUser: null });
  };

  deleteConfirmed = async () => {
    try {
      const user = this.state.deletingUser;
      await axios.delete(user.id, user);
      this.setState({ showDeleteModal: false, deletingUser: null });
      this.fetchUsers();
    } catch (error) {
      console.log(error);
    }
  };

  formatDateTime = (value: string) => {
    if (!value) return value;
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  componentDidMount() {
    this.fetchUsers();
  }

  async fetchUsers() {
    try {
      const response = await axios.get("");
      const fetchedUsers = response.data;
      this.setState({ users: fetchedUsers });
    } catch (error) {
      console.log(error);
    }
  }

  render() {
    const rows = this.state.users?.map((user) => (
      <tr key={user.id}>
        <td>
          <Link href={"/users/" + user.id}>{user.userName}</Link>
        </td>
        <td>{user.email}</td>
        <td>
          <Link className="btn btn-primary" href={"/users/edit/" + user.id}>
            Edit
          </Link>
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

    const table = this.state.users ? (
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
            <Link
              className="btn btn-primary"
              style={{ float: "right" }}
              href="/users/add"
            >
              Add User
            </Link>
          </div>
          <div className="card-body">
            <div className="table-responsive">{table}</div>
          </div>
        </div>
        {this.state.errorMessage ? (
          <div className="alert alert-danger">
            Error: {this.state.errorMessage}
          </div>
        ) : null}
        {deleteModal}
      </div>
    );
  }
}

export default ListUsers;
