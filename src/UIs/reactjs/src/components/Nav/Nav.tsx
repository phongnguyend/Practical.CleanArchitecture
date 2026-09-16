import React from "react";
import { NavLink } from "react-router-dom";

import { isAuthenticated } from "../../containers/Auth/authService";
import classes from "./Nav.module.css";
import ActionIcon from "../ActionIcon/ActionIcon";

const Nav = () => {
  const pageTitle = "ClassifiedAds.React";
  return (
    <nav
      className={"navbar navbar-expand navbar-light bg-light " + classes.Nav}
      style={{ paddingLeft: "1rem", paddingRight: "1rem" }}
    >
      <a className="navbar-brand" href="/">
        <ActionIcon action="home" />
        {pageTitle + " " + React.version}
      </a>
      <ul className="nav nav-tabs flex-grow-1">
        <li>
          <NavLink className="nav-link" to="/home">
            <ActionIcon action="home" /> Home
          </NavLink>
        </li>
        <li>
          <NavLink className="nav-link" to="/settings">
            <ActionIcon action="settings" /> Settings
          </NavLink>
        </li>
        <li>
          <NavLink className="nav-link" to="/files">
            <ActionIcon action="files" /> Files
          </NavLink>
        </li>
        <li>
          <NavLink className="nav-link" to="/products">
            <ActionIcon action="products" /> Products
          </NavLink>
        </li>
        <li>
          <NavLink className="nav-link" to="/users">
            <ActionIcon action="users" /> Users
          </NavLink>
        </li>
        <li><NavLink className="nav-link" to="/roles"><ActionIcon action="roles" /> Roles</NavLink></li>
        <li>
          <NavLink className="nav-link" to="/auditlogs">
            <ActionIcon action="audit" /> Audit Logs
          </NavLink>
        </li>

        {!isAuthenticated() ? (
          <li className="ms-auto">
            <NavLink className="nav-link" to="/login">
              <ActionIcon action="login" /> Login
            </NavLink>
          </li>
        ) : null}

        {isAuthenticated() ? (
          <li className="ms-auto">
            <NavLink className="nav-link" to="/logout">
              <ActionIcon action="logout" /> Logout
            </NavLink>
          </li>
        ) : null}
      </ul>
    </nav>
  );
};

export default Nav;
