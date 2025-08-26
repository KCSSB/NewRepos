import React from "react";
import "./Mode.css";
import Profile_logo_active from "./Profile_logo_active.png";
import Profile_logo from "./Profile_logo.png";
import Password_logo_active from "./Password_logo_active.png";
import Password_logo from "./Password_logo.png";
import Theme_logo_light from "./Theme_logo_light.png";
import Theme_logo_dark from "./Theme_logo_dark.png";

export default function Mode({ activeMode, setActiveMode }) {
  const isProfileActive = activeMode === "profile";
  const isPasswordActive = activeMode === "password";

  return (
    <div className="mode-container">
      <button
        className={isProfileActive ? "mode-button active" : "mode-button"}
        onClick={() => setActiveMode("profile")}
      >
        <img
          src={isProfileActive ? Profile_logo_active : Profile_logo}
          alt="PROFILE"
        />
      </button>

      <button
        className={isPasswordActive ? "mode-button active" : "mode-button"}
        onClick={() => setActiveMode("password")}
      >
        <img
          src={isPasswordActive ? Password_logo_active : Password_logo}
          alt="PASSWORD"
        />
      </button>

      <button className="mode-button">
        <img src={Theme_logo_dark} alt="THEME" />
      </button>
    </div>
  );
}
