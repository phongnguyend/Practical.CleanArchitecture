import React, { Component } from "react";
import { NavLink } from "react-router-dom";
import { connect } from "react-redux";

import classes from "./Nav.module.css";

class Nav extends Component {
  render() {
    const pageTitle = "ClassifiedAds.React";
    return (
      <nav
        className={"navbar navbar-expand navbar-light bg-light " + classes.Nav}
      >
        <a className="navbar-brand" href="/">
          {pageTitle + " " + React.version}
        </a>
        <ul className="nav nav-pills">
          <li>
            <NavLink className="nav-link" to="/home">
              Home
            </NavLink>
          </li>
          <li>
            <NavLink className="nav-link" to="/files">
              File
            </NavLink>
          </li>
          <li>
            <NavLink className="nav-link" to="/products">
              Product
            </NavLink>
          </li>

          <li>
            <NavLink className="nav-link" to="/auditlogs">
              Audit Logs
            </NavLink>
          </li>

          {!this.props.authService.isAuthenticated() ? (
            <li>
              <NavLink
                className="nav-link"
                to="/login"
                onClick={this.props.authService.login}
                href="javascript:void(0)"
              >
                Login
              </NavLink>
            </li>
          ) : null}

          {this.props.authService.isAuthenticated() ? (
            <li>
              <NavLink
                className="nav-link"
                to="/logout"
                onClick={this.props.authService.logout}
                href="javascript:void(0)"
              >
                Logout
              </NavLink>
            </li>
          ) : null}
        </ul>
      </nav>
    );
  }
}
const mapStateToProps = (state) => {
  return {
    user: state.auth.user,
    authService: state.auth.authService,
  };
};

const mapDispatchToProps = (dispatch) => {
  return {};
};

export default connect(mapStateToProps, mapDispatchToProps)(Nav);
