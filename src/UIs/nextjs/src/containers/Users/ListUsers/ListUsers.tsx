"use client";

import Link from "next/link";
import React, { useState, useEffect, useCallback } from "react";
import { Modal, Button } from "react-bootstrap";
import axios from "../axios";

interface User {
  id: string;
  userName: string;
  email: string;
}

const ListUsers: React.FC = () => {
  const [pageTitle] = useState("Users");
  const [users, setUsers] = useState<User[]>([]);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [deletingUser, setDeletingUser] = useState<User | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | undefined>();

  const deleteUser = (user: User) => {
    setShowDeleteModal(true);
    setDeletingUser(user);
  };

  const deleteCanceled = () => {
    setShowDeleteModal(false);
    setDeletingUser(null);
  };

  const deleteConfirmed = async () => {
    try {
      if (deletingUser) {
        await axios.delete(deletingUser.id);
        setShowDeleteModal(false);
        setDeletingUser(null);
        fetchUsers();
      }
    } catch (error) {
      console.log(error);
    }
  };

  const formatDateTime = (value: string) => {
    if (!value) return value;
    const date = new Date(value);
    return date.toLocaleDateString() + " " + date.toLocaleTimeString();
  };

  const fetchUsers = useCallback(async () => {
    try {
      const response = await axios.get("");
      const fetchedUsers: User[] = response.data;
      setUsers(fetchedUsers);
    } catch (error) {
      console.log(error);
    }
  }, []);

  useEffect(() => {
    fetchUsers();
  }, [fetchUsers]);

  const rows = users?.map((user: User) => (
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
          onClick={() => deleteUser(user)}
        >
          Delete
        </button>
      </td>
    </tr>
  ));

  const table = users ? (
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
    <Modal show={showDeleteModal} onHide={deleteCanceled}>
      <Modal.Header closeButton>
        <Modal.Title>Delete User</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        Are you sure you want to delete
        <strong> {deletingUser?.userName}</strong>
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

  return (
    <div>
      <div className="card">
        <div className="card-header">
          {pageTitle}
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
      {errorMessage ? (
        <div className="alert alert-danger">Error: {errorMessage}</div>
      ) : null}
      {deleteModal}
    </div>
  );
};

export default ListUsers;
