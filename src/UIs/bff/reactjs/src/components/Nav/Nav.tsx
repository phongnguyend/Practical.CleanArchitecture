import React from "react";
import { NavLink } from "react-router-dom";

import { isAuthenticated, logout } from "../../containers/Auth/authService";
import classes from "./Nav.module.css";
import ActionIcon from "../ActionIcon/ActionIcon";
import ThemeSwitcher from "../ThemeSwitcher/ThemeSwitcher";

const Nav = () => {
  const pageTitle = "ClassifiedAds.React";
  return (
    <nav
      className={"navbar navbar-expand bg-body " + classes.Nav}
      style={{ paddingLeft: "1rem", paddingRight: "1rem" }}
    >
      <a className="navbar-brand" href="/">
        <ActionIcon action="home" />
        {pageTitle + " " + React.version}
      </a>
      <ul className={"nav nav-tabs flex-grow-1 flex-nowrap " + classes.Tabs}>
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

      </ul>
      <div className="d-flex align-items-center gap-2 ms-auto flex-shrink-0">
        {!isAuthenticated() ? <a className="nav-link" href="/login"><ActionIcon action="login" /> Login</a> : null}
        {isAuthenticated() ? <a className="nav-link" onClick={logout} href="/logout"><ActionIcon action="logout" /> Logout</a> : null}
        <ThemeSwitcher />
      </div>
    </nav>
  );
};

export default Nav;
