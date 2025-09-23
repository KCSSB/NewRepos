import React from "react";
import { ProjectProvider, useProject } from "./HallContext.jsx";
import Navbar from "../Home/components/Navbar/Navbar.jsx";
import Boards from "./Components/Boards/Boards.jsx";
import "./Hall.css";
import "../../fonts/fonts.css";

export default function Hall() {
  return (
    <div className="hall-container">
      <Navbar />
      <ProjectProvider>
        <div className="hall-main-content">
          <Boards />
        </div>
      </ProjectProvider>
    </div>
  );
}
