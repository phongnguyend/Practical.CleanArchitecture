import Admin from "./containers/Admin/Admin";
import { initializePush, syncPush, consumePush } from "./core/push";
import { useEffect, useState } from "react";
import { App as NativeApp } from "@capacitor/app";
import { Capacitor } from "@capacitor/core";
import { useLocation, useNavigate } from "react-router-dom";
import { isAuthenticated } from "./containers/Auth/authService";
import Notifications from "./containers/Notifications/Notifications";
import "./mobile.css";
import { Routes, Route, Navigate } from "react-router-dom";

import "./App.css";
import Nav from "./components/Nav/Nav";
import Notification from "./components/Notification/Notification";
import Home from "./containers/Home/Home";
import OidcLoginRedirect from "./containers/Auth/OidcLoginRedirect";
import ListConfigurationEntries from "./containers/ConfigurationEntries/ListConfigurationEntries/ListConfigurationEntries";
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
import Roles from "./containers/Roles/Roles";
import Login from "./containers/Auth/Login";
import Logout from "./containers/Auth/Logout";

function App() {
  const [, refresh] = useState(0);
  const location = useLocation();
  useEffect(() => {
    const update = () => refresh((value) => value + 1);
    window.addEventListener("session-changed", update);
    const listener = Capacitor.isNativePlatform()
      ? NativeApp.addListener("backButton", ({ canGoBack }) => {
          if (document.querySelector('[role="dialog"]')) {
            document.dispatchEvent(
              new KeyboardEvent("keydown", { key: "Escape", bubbles: true }),
            );
          } else if (canGoBack) {
            window.history.back();
          } else {
            void NativeApp.exitApp();
          }
        })
      : undefined;
    return () => {
      window.removeEventListener("session-changed", update);
      void listener?.then((handle) => handle.remove());
    };
  }, []);
  const authenticated = isAuthenticated();
  const navigate = useNavigate();
  useEffect(() => {
    async function opened() {
      const target = await consumePush();
      if (target && isAuthenticated()) {
        navigate(`/${target}`);
      }
    }
    void initializePush().catch(() => undefined);
    const handleOpened = () => void opened();
    window.addEventListener("push-opened", handleOpened);
    if (authenticated) {
      void syncPush().catch(() => undefined);
      void opened();
    }
    return () => window.removeEventListener("push-opened", handleOpened);
  }, [authenticated, navigate]);
  const authenticationRoute = [
    "/login",
    "/logout",
    "/oidc-login-redirect",
  ].includes(location.pathname);
  return (
    <div className="mobile-shell">
      <Nav />
      <main>
        {!authenticated && !authenticationRoute ? (
          <Home />
        ) : (
          <Routes>
            <Route path="/admin" element={<Admin />} />
            <Route path="/admin/playgrounds" element={<Admin view="playgrounds" />} />
            <Route path="/admin/playgrounds/mobile-push" element={<Admin view="mobilepush" />} />
            <Route path="/admin/playgrounds/email" element={<Admin view="email" />} />
            <Route path="/admin/playgrounds/sms" element={<Admin view="sms" />} />
            <Route path="/notifications" element={<Notifications />} />
            <Route path="/home" element={<Home />} />
            <Route
              path="/oidc-login-redirect"
              element={<OidcLoginRedirect />}
            />
            <Route path="/configurationentries" element={<ListConfigurationEntries />} />
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
            <Route path="/roles/:id" element={<Roles mode="view" />} />
            <Route path="/roles" element={<Roles mode="list" />} />
            <Route path="/auditlogs" element={<AuditLogs />} />
            <Route path="/login" element={<Login />} />
            <Route path="/logout" element={<Logout />} />
            <Route path="*" element={<Navigate to="/home" />} />
          </Routes>
        )}
      </main>
      {authenticated && <Notification key="authenticated-notifications" />}
    </div>
  );
}

export default App;
