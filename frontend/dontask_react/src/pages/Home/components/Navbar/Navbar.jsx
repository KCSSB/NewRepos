import React, { useState, useEffect } from "react";
import { Link, useLocation } from "react-router-dom";
import "./Navbar.css";
import { decodeToken } from "../../../../service/api";
import dontask_logo from "./dontask_logo.png";
import home_logo from "./home_logo.png";
import home_logo_active from "./home_logo_active.png";
import tasks_logo from "./tasks_logo.png";
import tasks_logo_active from "./tasks_logo_active.png";
import settings_logo from "./settings_logo.png";
import settings_logo_active from "./settings_logo_active.png";
import default_avatar from "./avatar.png";

export default function Navbar() {
  const location = useLocation();
  const isActiveHome = location.pathname === "/home";
  const isActiveTasks = location.pathname === "/tasks";
  const isActiveSettings = location.pathname === "/settings";
  const [userAvatar, setUserAvatar] = useState(default_avatar);

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      const payload = decodeToken(token);
      console.log("Содержимое токена:", payload);
      if (payload && payload.Avatar) {
        setUserAvatar(payload.Avatar);
      }
    }
  }, []);

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
        <Link to="/task">
          <div className="navbar-container-item">
            <button
              className={
                isActiveTasks ? "navbar-button active" : "navbar-button"
              }
            >
              <img
                src={isActiveTasks ? tasks_logo_active : tasks_logo}
                alt="TASK"
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

        <div className="navbar-container-item profile">
          <img src={userAvatar} alt="AVATAR" />
        </div>
      </div>
    </div>
  );
}
