import { NavLink } from "react-router-dom";
import { isAuthenticated } from "../../containers/Auth/authService";
import ActionIcon from "../ActionIcon/ActionIcon";
import ThemeSwitcher from "../ThemeSwitcher/ThemeSwitcher";
export const destinations = [
  ["/home", "Home", "home"],
  ["/products", "Products", "products"],
  ["/files", "Files", "files"],
  ["/users", "Users", "users"],
  ["/roles", "Roles", "roles"],
  ["/settings", "Settings", "settings"],
  ["/auditlogs", "Audit logs", "audit"],
  ["/notifications", "Notifications", "notification"],
] as const;
export default function Nav() {
  return (
    <div className="mobile-chrome">
      <header className="mobile-header">
        <span>CLASSIFIED ADS (Capacitor)</span>
        <ThemeSwitcher />
      </header>
      {isAuthenticated() && (
        <nav aria-label="Main navigation" className="mobile-navigation">
          {destinations.map(([path, title, icon]) => (
            <NavLink className="nav-link" key={path} to={path}>
              <ActionIcon action={icon} />
              {title}
            </NavLink>
          ))}
        </nav>
      )}
    </div>
  );
}
