import React from "react";
import { Switch, Route, Redirect } from "react-router-dom";

import "./App.css";
import Nav from "./components/Nav/Nav";
import Home from "./containers/Home/Home";
import OidcLoginRedirect from "./containers/Auth/OidcLoginRedirect";
import Products from "./containers/Products/Products";
import AddProduct from "./containers/Products/AddProduct/AddProduct";
import ViewProduct from "./containers/Products/ViewProduct/ViewProduct";
import AuditLogs from "./containers/AuditLogs/AuditLogs";
import ListFiles from "./containers/Files/ListFiles/ListFiles";
import UploadFile from "./containers/Files/UploadFile/UploadFile";
import EditFile from "./containers/Files/EditFile/EditFile";

function App() {
  return (
    <div className="container">
      <Nav />
      <Switch>
        <Route path="/home" component={Home} />
        <Route path="/oidc-login-redirect" component={OidcLoginRedirect} />
        <Route path="/files/upload" component={UploadFile} />
        <Route path="/files/edit/:id" component={EditFile} />
        <Route path="/files" component={ListFiles} />
        <Route path="/products/add" component={AddProduct} />
        <Route path="/products/edit/:id" component={AddProduct} />
        <Route path="/products/:id" component={ViewProduct} />
        <Route path="/products" component={Products} />
        <Route path="/auditlogs" component={AuditLogs} />
        <Redirect to="/home" />
      </Switch>
    </div>
  );
}

export default App;
