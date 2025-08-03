import { Link, useLocation } from "react-router-dom";
import "./Navbar.css";
import dontask_logo from "./dontask_logo.png";
import home_logo from "./home_logo.png";
import home_logo_active from "./home_logo_active.png";
import tasks_logo from "./tasks_logo.png";
import tasks_logo_active from "./tasks_logo_active.png";
import settings_logo from "./settings_logo.png";
import settings_logo_active from "./settings_logo_active.png";
import avatar from "./avatar.png";

export default function Navbar() {
  const location = useLocation();
  const isActiveHome = location.pathname === "/home";
  const isActiveTasks = location.pathname === "/tasks";
  const isActiveSettings = location.pathname === "/settings";
  return (
    <div className="navbar-container">
      <div className="navbar-top-buttons">
        <img src={dontask_logo} alt="DONTASK" />
        <Link to="/home">
          <div className="navbar-container-item">
            <button
              className={
                isActiveHome ? "navbar-button active" : "navbar-button"
              }
            >
              <img
                src={isActiveHome ? home_logo_active : home_logo}
                alt="HOME"
              />
            </button>
          </div>
        </Link>
        <Link to="/tasks">
          <div className="navbar-container-item">
            <button
              className={
                isActiveTasks ? "navbar-button active" : "navbar-button"
              }
            >
              <img
                src={isActiveTasks ? tasks_logo_active : tasks_logo}
                alt="TASKS"
              />
            </button>
          </div>
        </Link>
      </div>
      <div className="navbar-bottom-buttons">
        <Link to="/settings">
          <div className="navbar-container-item">
            <button
              className={
                isActiveSettings ? "navbar-button active" : "navbar-button"
              }
            >
              <img
                src={isActiveSettings ? settings_logo_active : settings_logo}
                alt="SETTINGS"
              />
            </button>
          </div>
        </Link>
        <Link to="/profile">
          <div className="navbar-container-item profile">
            <button className="navbar-button">
              <img src={avatar} alt="AVATAR" />
            </button>
          </div>
        </Link>
      </div>
    </div>
  );
}
