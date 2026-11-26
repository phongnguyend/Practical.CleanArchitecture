"use client";

import Link from "next/link";
import classes from "./Nav.module.css";
import { usePathname } from "next/navigation";
import ActionIcon from "../ActionIcon/ActionIcon";
import ThemeSwitcher from "../ThemeSwitcher/ThemeSwitcher";

interface NavProps {
  isAuthenticated: boolean;
}

const Nav = ({ isAuthenticated }: NavProps) => {
  const pageTitle = "ClassifiedAds.NextJs";

  const pathname = usePathname();

  const isActive = (path: string) => {
    return pathname === path || pathname.startsWith(path + "/");
  };

  return (
    <nav
      aria-label="Main navigation"
      className={"navbar navbar-expand bg-body " + classes.Nav}
    >
      <div className={classes.Header}>
        <Link className="navbar-brand" href="/">
          <ActionIcon action="home" />
          {pageTitle}
        </Link>
        <div className="d-flex align-items-center gap-2 ms-auto flex-shrink-0">
          {!isAuthenticated ? <Link className="nav-link" href="/login"><ActionIcon action="login" /> Login</Link> : null}
          {isAuthenticated ? <Link className="nav-link" href="/logout"><ActionIcon action="logout" /> Logout</Link> : null}
          <ThemeSwitcher />
        </div>
      </div>
      <ul className={"nav nav-tabs flex-wrap " + classes.Tabs}>
        <li>
          <Link
            className={
              isActive("/home") || isActive("/")
                ? "nav-link active"
                : "nav-link"
            }
            href="/home"
          >
            <ActionIcon action="home" /> Home
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/settings") ? "nav-link active" : "nav-link"}
            href="/settings"
          >
            <ActionIcon action="settings" /> Settings
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/files") ? "nav-link active" : "nav-link"}
            href="/files"
          >
            <ActionIcon action="files" /> Files
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/products") ? "nav-link active" : "nav-link"}
            href="/products"
          >
            <ActionIcon action="products" /> Products
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/users") ? "nav-link active" : "nav-link"}
            href="/users"
          >
            <ActionIcon action="users" /> Users
          </Link>
        </li>
        <li><Link className={isActive("/roles") ? "nav-link active" : "nav-link"} href="/roles"><ActionIcon action="roles" /> Roles</Link></li>
        <li>
          <Link
            className={isActive("/auditlogs") ? "nav-link active" : "nav-link"}
            href="/auditlogs"
          >
            <ActionIcon action="audit" /> Audit Logs
          </Link>
        </li>

      <li><Link className={isActive("/admin") ? "nav-link active" : "nav-link"} href="/admin"><ActionIcon action="settings" /> Admin</Link></li>
      </ul>
    </nav>
  );
};

export default Nav;
