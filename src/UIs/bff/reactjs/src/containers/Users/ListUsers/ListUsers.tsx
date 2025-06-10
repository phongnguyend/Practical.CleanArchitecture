import React, { useState, useEffect } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";
import { Modal, Button } from "react-bootstrap";

import * as actions from "../actions";

const ListUsers = (props: any) => {
    const [pageTitle] = useState("Users");
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [deletingUser, setDeletingUser] = useState({
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
                    onClick={() => deleteUser(user)}
                >
                    Delete
                </button>
            </td>
        </tr>
    ));

    const table = props.users ? (
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
                    <NavLink className="btn btn-primary" style={{ float: "right" }} to="/users/add">
                        Add User
                    </NavLink>
                </div>
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
