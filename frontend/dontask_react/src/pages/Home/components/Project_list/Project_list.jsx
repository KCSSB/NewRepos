import React from "react";
import "./Project_list.css";
import people_logo from "./people_logo.png";
import { projects } from "./project_data";

export default function Project_list() {
  const handleCardClick = (projectId) => {
    console.log(`Нажата карточка проекта с ID: ${projectId}`);
  };

  return (
    <div className="project-list-container">
      <div className="project-list-wrapper">
        {projects.map((project, index) => (
          <button
            key={index}
            className="project-card"
            onClick={() => handleCardClick(project.id)}
          >
            <div className="card-top">
              <img src={project.logo} alt="LOGO" className="project-logo" />
            </div>
            <div className="card-bottom">
              <div className="creator-info">
                <img
                  src={project.creator_avatar}
                  alt="AVATAR"
                  className="creator-avatar"
                />
                <div className="text-info">
                  <h5>{project.title}</h5>
                  <p>{project.creator_name}</p>
                </div>
              </div>
              <div className="people-count">
                <img src={people_logo} alt="People" className="people-logo" />
                <h6>{project.amount_of_people}</h6>
              </div>
            </div>
          </button>
        ))}
      </div>
    </div>
  );
}
