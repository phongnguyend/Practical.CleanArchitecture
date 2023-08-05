import { Routes, Route, Navigate } from "react-router-dom";

import "./App.css";
import Nav from "./components/Nav/Nav";
import Notification from "./components/Notification/Notification";
import Home from "./containers/Home/Home";
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
      <Routes>
        <Route path="/home" element={<Home />} />
        <Route path="/settings" element={<ListConfigurationEntries />} />
        <Route path="/files/upload" element={<UploadFile />} />
        <Route path="/files/edit/:id" element={<EditFile />} />
        <Route path="/files" element={<ListFiles />} />
        <Route path="/products/add" element={<AddProduct />} />
        <Route path="/products/edit/:id" element={<AddProduct />} />
        <Route path="/products/:id" element={<ViewProduct />} />
        <Route path="/products" element={<ListProducts />} />
        <Route path="/users/add" element={<AddUser />} />
        <Route path="/users/edit/:id" element={<AddUser />} />
        <Route path="/users/:id" element={<ViewUser />} />
        <Route path="/users" element={<ListUsers />} />
        <Route path="/auditlogs" element={<AuditLogs />} />
        <Route path="*" element={<Navigate to="/home" />} />
      </Routes>
      <Notification />
    </div>
  );
}

export default App;
