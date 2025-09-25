import React from "react";
import { useProject } from "../../HallContext.jsx";
import "./ProjectDescription.css";

export default function ProjectDescription() {
  const { projectData, loading } = useProject();

  if (loading) {
    return <div>Загрузка...</div>; // СКЕЛЕТОН
  }

  const description =
    projectData?.description || "Описание проекта отсутствует.";

  return (
    <div className="project-description-container">
            <p>{description}</p>   {" "}
    </div>
  );
}
