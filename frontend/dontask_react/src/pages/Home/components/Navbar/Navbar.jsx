import React, { useState, useEffect } from "react";
import { Link, useLocation } from "react-router-dom";
import "./Navbar.css";
import { getAvatarFromToken } from "../../../../service/api";
import dontask_logo from "./dontask_logo.png";
import home_logo from "./home_logo.png";
import workspace_logo from "./workspace_logo.png";
import settings_logo from "./settings_logo.png";
import hall_logo from "./hall_logo.png";
import default_avatar from "./avatar.png";

export default function Navbar() {
  const location = useLocation();
  const isActiveHome = location.pathname === "/home";
  const isActiveHall = location.pathname.startsWith("/hall");
  const isActiveWorkspace = location.pathname === "/workspace";
  const isActiveSettings = location.pathname === "/settings";
  const [userAvatar, setUserAvatar] = useState(default_avatar);
  const [lastProjectId, setLastProjectId] = useState(null);

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
    const storedProjectId = localStorage.getItem("lastVisitedProjectId");
    if (storedProjectId) {
      setLastProjectId(storedProjectId);
    }

    const handleTokenChange = () => {
      updateAvatarFromToken();
    };

    window.addEventListener("tokenUpdated", handleTokenChange);

    return () => {
      window.removeEventListener("tokenUpdated", handleTokenChange);
    };
  }, []);

  const hallLink = lastProjectId ? `/hall/${lastProjectId}` : "/hall";

  const isHallLinkDisabled = !lastProjectId && !isActiveHall;

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
              <img src={home_logo} alt="HOME" />
            </button>
          </div>
        </Link>
        <Link
          to={hallLink}
          className={isHallLinkDisabled ? "disabled-link" : ""}
        >
          <div className="navbar-container-item">
            <button
              className={
                isActiveHall ? "navbar-button active" : "navbar-button"
              }
            >
              <img src={hall_logo} alt="HALL" />
            </button>
          </div>
        </Link>
        <Link to="/workspace">
          <div className="navbar-container-item">
            <button
              className={
                isActiveWorkspace ? "navbar-button active" : "navbar-button"
              }
            >
              <img src={workspace_logo} alt="WORKSPACE" />
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
              <img src={settings_logo} alt="SETTINGS" />
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
