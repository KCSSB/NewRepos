import React, { useState } from "react";
import Navbar from "../Home/components/Navbar/Navbar.jsx";
import ProfileContainer from "./components/Profile/Profile.jsx";
import PasswordContainer from "./components/Password/Password.jsx";
import Mode from "./components/Mode/Mode.jsx";
import "./Settings.css";
import "../../fonts/fonts.css";

export default function Settings() {
  const [activeMode, setActiveMode] = useState("profile");

  return (
    <div className="settings-container">
      <Navbar />
      <div className="settings-main-content">
        {activeMode === "profile" ? (
          <ProfileContainer />
        ) : (
          <PasswordContainer />
        )}
        <Mode activeMode={activeMode} setActiveMode={setActiveMode} />
      </div>
    </div>
  );
}
