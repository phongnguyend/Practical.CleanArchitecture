import React from "react";
import { NavLink } from "react-router-dom";

import { isAuthenticated, logout } from "../../containers/Auth/authService";
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
      <ul className="nav nav-pills">
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
        <li>
          <NavLink className="nav-link" to="/auditlogs">
            <ActionIcon action="audit" /> Audit Logs
          </NavLink>
        </li>

        {!isAuthenticated() ? (
          <li>
            <a className="nav-link" href="/login">
              <ActionIcon action="login" /> Login
            </a>
          </li>
        ) : null}

        {isAuthenticated() ? (
          <li>
            <a className="nav-link" onClick={logout} href="/logout">
              <ActionIcon action="logout" /> Logout
            </a>
          </li>
        ) : null}
      </ul>
    </nav>
  );
};

export default Nav;
