import React, { useState } from "react";
import MembersList from "./MembersList.jsx";
import "./HallFooter.css";
import people_icon from "./people_icon.png";
import edit_icon from "./edit_icon.png";
import { useProject } from "../../HallContext.jsx";

export default function HallFooter() {
  const [showMembersList, setShowMembersList] = useState(false);
  const [isCreating, setIsCreating] = useState(false);

  const { projectData } = useProject();

  const handleMembersClick = () => {
    setIsCreating(false);
    setShowMembersList((prev) => !prev);
  };

  return (
    <div className="hall-footer-container">
      <div className={`hall-mode-wrapper ${showMembersList ? "active" : ""}`}>
        <div className="hall-mode-container">
          <button className="hall-mode-button">
            <img src={edit_icon} alt="EDIT" />
          </button>
          <button
            className={`hall-mode-button ${showMembersList ? "active" : ""}`}
            onClick={handleMembersClick}
          >
            <img src={people_icon} alt="MEMBERS" />
          </button>
        </div>
        {projectData && (
          <MembersList
            members={projectData.projectUsers}
            isCreating={isCreating}
            setIsCreating={setIsCreating}
          />
        )}
      </div>
    </div>
  );
}
