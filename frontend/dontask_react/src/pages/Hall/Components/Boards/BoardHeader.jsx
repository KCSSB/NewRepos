import React, { useEffect, useState } from "react";
import { useProject } from "../../HallContext.jsx";
import { useNavigate } from "react-router-dom";
import "./BoardHeader.css";
import filter_icon from "./filter_icon.png";
import board_icon from "./board_icon.png";

export default function BoardHeader({ boardsCount }) {
  const {
    projectData,
    loading,
    showToast,
    isFilteredByMember,
    toggleFilter,
    isEditMode,
    setProjectData,
  } = useProject();
  const navigate = useNavigate();

  const [projectName, setProjectName] = useState("");

  useEffect(() => {
    if (!loading && projectData) {
      setProjectName(projectData.projectName);
    }
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

  const handleProjectNameChange = (e) => {
    setProjectName(e.target.value);
  };

  const handleSaveProjectName = async (e) => {
    if (e.key === "Enter" || e.type === "blur") {
      e.target.blur();
      const trimmedName = projectName.trim();
      if (!trimmedName || trimmedName === projectData.projectName) {
        setProjectName(projectData.projectName);
        return;
      }

      // логика отправки на сервер
      // try {
      //   await putWithAuth(`/project/${projectData.projectId}/UpdateName`, { NewName: trimmedName });
      //   // Обновляем контекст, чтобы изменения отразились
      //   setProjectData(prev => ({ ...prev, projectName: trimmedName }));
      //   showToast("Название проекта обновлено!", "success");
      // } catch (error) {
      //   showToast("Ошибка при обновлении названия.", "error");
      //   setProjectName(projectData.projectName);
      // }
      console.log("Сохранение нового названия проекта:", trimmedName);
    }
  };

  if (loading || !projectData) {
    return <div>Загрузка...</div>;
  }

  const handleFilterClick = () => {
    toggleFilter();
    showToast(
      isFilteredByMember
        ? "Показаны все доски"
        : "Показаны доски, в которых Вы состоите",
      "info"
    );
  };

  return (
    <div className="board-header-container">
      {isEditMode ? (
        <input
          type="text"
          className="project-title-input"
          value={projectName}
          onChange={handleProjectNameChange}
          onBlur={handleSaveProjectName}
          onKeyDown={(e) => {
            if (e.key === "Enter") handleSaveProjectName(e);
          }}
        />
      ) : (
        <h5 className="project-title">{projectData.projectName}</h5>
      )}

      <div className="action-container">
        <div className="action-left-container">
          <h5 className="text-style">Доски</h5>
          <div className="board-counter">
            <img src={board_icon} alt="BOARDS" />
            <p>{boardsCount}</p>
          </div>
        </div>
        <div className="action-right-container">
          <button
            className={`filter-button ${isFilteredByMember ? "active" : ""}`}
            onClick={handleFilterClick}
          >
            <img src={filter_icon} alt="FILTER" />
          </button>
        </div>
      </div>
      <div className="separating-line"> </div>
    </div>
  );
}
