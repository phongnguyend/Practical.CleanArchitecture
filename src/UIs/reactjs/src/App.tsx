import React from "react";
import { Switch, Route, Redirect } from "react-router-dom";

import "./App.css";
import Nav from "./components/Nav/Nav";
import Notification from "./components/Notification/Notification";
import Home from "./containers/Home/Home";
import OidcLoginRedirect from "./containers/Auth/OidcLoginRedirect";
import ListConfigurationEntries from "./containers/Settings/ListConfigurationEntries/ListConfigurationEntries";
import ListProducts from "./containers/Products/ListProducts/ListProducts";
import AddProduct from "./containers/Products/AddProduct/AddProduct";
import ViewProduct from "./containers/Products/ViewProduct/ViewProduct";
import AuditLogs from "./containers/AuditLogs/AuditLogs";
import ListFiles from "./containers/Files/ListFiles/ListFiles";
import UploadFile from "./containers/Files/UploadFile/UploadFile";
import EditFile from "./containers/Files/EditFile/EditFile";
import ListUsers from "./containers/Users/ListUsers/ListUsers";
import AddUser from "./containers/Users/AddUser/AddUser";
import ViewUser from "./containers/Users/ViewUser/ViewUser";

function App() {
  return (
    <div className="container">
      <Nav />
      <Switch>
        <Route path="/home" component={Home} />
        <Route path="/oidc-login-redirect" component={OidcLoginRedirect} />
        <Route path="/settings" component={ListConfigurationEntries} />
        <Route path="/files/upload" component={UploadFile} />
        <Route path="/files/edit/:id" component={EditFile} />
        <Route path="/files" component={ListFiles} />
        <Route path="/products/add" component={AddProduct} />
        <Route path="/products/edit/:id" component={AddProduct} />
        <Route path="/products/:id" component={ViewProduct} />
        <Route path="/products" component={ListProducts} />
        <Route path="/users/add" component={AddUser} />
        <Route path="/users/edit/:id" component={AddUser} />
        <Route path="/users/:id" component={ViewUser} />
        <Route path="/users" component={ListUsers} />
        <Route path="/auditlogs" component={AuditLogs} />
        <Redirect to="/home" />
      </Switch>
      <Notification />
    </div>
  );
}

export default App;
