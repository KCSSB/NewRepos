// src/pages/Workspace/components/Card/Card.jsx

import React, { useState } from "react";
import { useWorkspace } from "../../WorkspaceContext.jsx";
import add_icon from "./add_icon.png"; // Убедитесь, что этот путь верен
import "./Card.css";

export default function Card() {
  const { createCard, loading } = useWorkspace();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleCreateCardClick = async () => {
    if (loading || isSubmitting) return;

    setIsSubmitting(true);
    try {
      await createCard();
    } catch (e) {
      // Ошибка уже обработана в контексте
    } finally {
      setIsSubmitting(false);
    }
  };

  const isDisabled = loading || isSubmitting;

  return (
    <div className="card-container">
      {/* ПРЕОБРАЗУЕМ DIV В BUTTON */}
      <button
        type="button"
        className="card-create-container card-create-button-style" // Добавлен дополнительный класс для стилей кнопки
        onClick={handleCreateCardClick}
        disabled={isDisabled}
      >
        {isDisabled ? (
          "Создание карточки..."
        ) : (
          <>
            {/* Содержимое кнопки */}
            <img
              src={add_icon}
              alt="Добавить карточку"
              className="card-create-icon"
            />
            <p className="card-create-text">Добавить карточку</p>
          </>
        )}
      </button>

      {/* Здесь будут рендериться существующие карточки */}
    </div>
  );
}
