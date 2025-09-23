// BoardHeader.jsx
import React, { useEffect } from "react";
import { useProject } from "../../HallContext.jsx";
import { useNavigate } from "react-router-dom";
import "./BoardHeader.css";
import filter_icon from "./filter_icon.png";

export default function BoardHeader({ boardsCount, loading }) {
  const { projectData, showToast } = useProject();
  const navigate = useNavigate();

  useEffect(() => {
    if (!loading && !projectData) {
      showToast(
        "Не удалось найти проект. Пожалуйста, попробуйте снова",
        "error"
      );
      const timer = setTimeout(() => {
        navigate("/home");
      }, 1000);
      return () => clearTimeout(timer);
    }
  }, [loading, projectData, navigate, showToast]);

  if (loading) {
    return <div>Загрузка...</div>; // СКЕЛЕТОН
  }

  return (
    <div className="board-header-container">
      <h5 className="project-title">{projectData.projectName}</h5>
      <div className="action-container">
        <div className="action-left-container">
          <h5 className="text-style">Доски</h5>
          <div className="board-counter">
            <p>{boardsCount}</p>
          </div>
        </div>
        <div className="action-right-container">
          <button className="filter-button">
            <img src={filter_icon} alt="FILTER" />
          </button>
        </div>
      </div>
      <div className="separating-line"> </div>
    </div>
  );
}
