import React, { useState } from "react";
import { useProject } from "../../HallContext.jsx";
import MembersList from "./MembersList.jsx";
import "./HallFooter.css";
import people_icon from "./people_icon.png";
import edit_icon from "./edit_icon.png";
import confirmChanges_icon from "./confirmChanges_icon.png";
import resetChanges_icon from "./resetChanges_icon.png";

export default function HallFooter() {
  const { projectData, isEditMode, toggleEditMode } = useProject();

  const [showMembersList, setShowMembersList] = useState(false);
  const [isCreating, setIsCreating] = useState(false);

  const handleEditClick = () => {
    toggleEditMode(); // Включаем режим редактирования
    setShowMembersList(true); // Показываем список участников, чтобы можно было сразу удалять
    setIsCreating(false);
  };

  const handleConfirmClick = () => {
    // TODO: Здесь должна быть логика сохранения изменений (отправка API запросов)
    console.log("Сохранение изменений...");
    // После сохранения выключаем режим редактирования
    toggleEditMode();
  };

  const handleResetClick = () => {
    // Выключаем режим редактирования, отменяя все несохраненные изменения
    toggleEditMode();
  };

  const handleMembersClick = () => {
    // Блокируем кнопку Members, если включен режим редактирования
    if (isEditMode) return;

    setIsCreating(false);
    setShowMembersList((prev) => !prev);
  };

  return (
    <div className="hall-footer-container">
      <div className={`hall-mode-wrapper ${showMembersList ? "active" : ""}`}>
        <div className="hall-mode-container">
          {isEditMode ? (
            // В режиме редактирования: показываем Confirm и Reset
            <>
              <button className="hall-mode-button" onClick={handleConfirmClick}>
                <img src={confirmChanges_icon} alt="CONFIRM" />
              </button>
              <button className="hall-mode-button" onClick={handleResetClick}>
                <img src={resetChanges_icon} alt="RESET" />
              </button>
            </>
          ) : (
            // В обычном режиме: показываем только Edit
            <button
              className={`hall-mode-button ${isEditMode ? "active" : ""}`}
              onClick={handleEditClick}
            >
              <img src={edit_icon} alt="EDIT" />
            </button>
          )}

          {/* Кнопка Members: Рендерится всегда, но отключается в режиме редактирования.
              🛑 Удален дублирующий блок, который был ниже. */}
          <button
            className={`hall-mode-button ${showMembersList ? "active" : ""} ${
              isEditMode ? "disabled-member" : ""
            }`}
            onClick={handleMembersClick}
            disabled={isEditMode}
          >
            <img src={people_icon} alt="MEMBERS" />
          </button>
        </div>
        {projectData && (
          <MembersList
            members={projectData.projectUsers}
            isCreating={isCreating}
            setIsCreating={setIsCreating}
            isEditMode={isEditMode}
          />
        )}
      </div>
    </div>
  );
}
