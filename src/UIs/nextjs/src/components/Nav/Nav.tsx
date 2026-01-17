"use client";

import Link from "next/link";
import classes from "./Nav.module.css";
import { usePathname } from "next/navigation";

interface NavProps {
  isAuthenticated: boolean;
}

const Nav = ({ isAuthenticated }: NavProps) => {
  const pageTitle = "ClassifiedAds.NextJs";
  const nextVersion = "16.0.6";

  const pathname = usePathname();

  const isActive = (path: string) => {
    return pathname === path || pathname.startsWith(path + "/");
  };

  return (
    <nav
      className={"navbar navbar-expand navbar-light bg-light " + classes.Nav}
      style={{ paddingLeft: "1rem", paddingRight: "1rem" }}
    >
      <Link className="navbar-brand" href="/">
        {pageTitle + " " + nextVersion}
      </Link>
      <ul className="nav nav-pills">
        <li>
          <Link
            className={
              isActive("/home") || isActive("/")
                ? "nav-link active"
                : "nav-link"
            }
            href="/home"
          >
            Home
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/settings") ? "nav-link active" : "nav-link"}
            href="/settings"
          >
            Settings
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/files") ? "nav-link active" : "nav-link"}
            href="/files"
          >
            Files
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/products") ? "nav-link active" : "nav-link"}
            href="/products"
          >
            Products
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/users") ? "nav-link active" : "nav-link"}
            href="/users"
          >
            Users
          </Link>
        </li>
        <li>
          <Link
            className={isActive("/auditlogs") ? "nav-link active" : "nav-link"}
            href="/auditlogs"
          >
            Audit Logs
          </Link>
        </li>

        {!isAuthenticated ? (
          <li>
            <Link className="nav-link" href="/login">
              Login
            </Link>
          </li>
        ) : null}

        {isAuthenticated ? (
          <li>
            <Link className="nav-link" href="/logout">
              Logout
            </Link>
          </li>
        ) : null}
      </ul>
    </nav>
  );
};

export default Nav;
