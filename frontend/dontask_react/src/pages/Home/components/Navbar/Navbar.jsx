import React, { useState, useEffect } from "react";
import { Link, useLocation } from "react-router-dom";
import "./Navbar.css";
import { getAvatarFromToken } from "../../../../service/api";
import dontask_logo from "./dontask_logo.png";
import home_logo from "./home_logo.png";
import home_logo_active from "./home_logo_active.png";
import board_logo from "./board_logo.png";
import board_logo_active from "./board_logo_active.png";
import settings_logo from "./settings_logo.png";
import settings_logo_active from "./settings_logo_active.png";
import project_logo from "./project_logo.png";
import project_logo_active from "./project_logo_active.png";
import default_avatar from "./avatar.png";

export default function Navbar() {
  const location = useLocation();
  const isActiveHome = location.pathname === "/home";
  const isActiveProject = location.pathname === "/project";
  const isActiveBoard = location.pathname === "/board";
  const isActiveSettings = location.pathname === "/settings";
  const [userAvatar, setUserAvatar] = useState(default_avatar);

  const updateAvatarFromToken = () => {
    const token = localStorage.getItem("token");
    if (token) {
      const payload = getAvatarFromToken(token);
      console.log("Содержимое токена:", payload);
      if (payload && payload.Avatar) {
        setUserAvatar(payload.Avatar);
      } else {
        setUserAvatar(default_avatar);
      }
    } else {
      setUserAvatar(default_avatar);
    }
  };

  useEffect(() => {
    updateAvatarFromToken();

    const handleTokenChange = () => {
      console.log("Событие 'tokenUpdated' получено. Обновляем аватар.");
      updateAvatarFromToken();
    };

    window.addEventListener("tokenUpdated", handleTokenChange);

    return () => {
      window.removeEventListener("tokenUpdated", handleTokenChange);
    };
  }, []);

  return (
    <div className="navbar-container">
      <div className="navbar-top">
        <img src={dontask_logo} alt="DONTASK" />
      </div>
      <div className="navbar-center-buttons">
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
        <Link to="/project">
          <div className="navbar-container-item">
            <button
              className={
                isActiveProject ? "navbar-button active" : "navbar-button"
              }
            >
              <img
                src={isActiveProject ? project_logo_active : project_logo}
                alt="PROJECT"
              />
            </button>
          </div>
        </Link>
        <Link to="/board">
          <div className="navbar-container-item">
            <button
              className={
                isActiveBoard ? "navbar-button active" : "navbar-button"
              }
            >
              <img
                src={isActiveBoard ? board_logo_active : board_logo}
                alt="BOARD"
              />
            </button>
          </div>
        </Link>
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
      </div>
      <div className="navbar-bottom">
        <div className="navbar-container-item profile">
          <img src={userAvatar} alt="AVATAR" className="avatar-image" />
        </div>
      </div>
    </div>
  );
}
