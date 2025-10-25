import React, { useState, useEffect, useCallback } from "react";
// Импортируем из общего контекста, который включает isEditMode и updateTaskName
import { useWorkspace } from "../../WorkspaceContext.jsx";
import { useWorkspaceEdit } from "../../WorkspaceEditContext.jsx";
import "./TaskItem.css";

// Этот компонент отвечает за рендеринг одной задачи
const TaskItem = ({ task }) => {
  // 🔑 Получаем состояние редактирования и функцию обновления
  const { updateSubTaskStatus } = useWorkspace();
  const { isEditMode, updateCardChanges } = useWorkspaceEdit();
  // 🔑 ВРЕМЕННЫЙ ЛОГ
  console.log(`Task ${task.cardId}: isEditMode =`, isEditMode);
  // Используем ID задачи (cardId)
  const taskId = task.cardId;

  // Локальное состояние для управления текстом в поле ввода
  const [name, setName] = useState(task.cardName || "Новая задача");

  // Синхронизация: если внешние данные (task.cardName) изменились, обновляем локальное состояние
  useEffect(() => {
    setName(task.cardName || "Новая задача");
  }, [task.cardName]);

  // Обработчик сохранения при потере фокуса или нажатии Enter
  const handleSave = async () => {
    const newName = name.trim();

    // 1. Проверяем, изменилось ли имя
    if (
      newName === (task.cardName || "Новая задача").trim() ||
      newName === ""
    ) {
      // Если пусто, или не изменилось, просто возвращаем
      if (newName === "") setName(task.cardName || "Новая задача");
      return;
    }

    // 2. Вызываем API из контекста
    // 2. Вызываем функцию из WorkspaceEditContext для записи изменения в локальный state
    updateCardChanges(taskId, newName); // ✅ ИСПРАВЛЕНИЕ
  };

  // Обработчик нажатия Enter
  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      e.preventDefault(); // Предотвращаем стандартное действие (например, отправку формы)
      e.target.blur(); // Принудительно вызываем onBlur, который сохранит данные
    }
  };

  // ----------------------------------------------------
  // 🔑 УСЛОВНЫЙ РЕНДЕРИНГ НАЗВАНИЯ ЗАДАЧИ
  // ----------------------------------------------------
  return (
    <div className="task-item-container">
      <div className="task-name-wrapper">
        {isEditMode ? (
          // РЕЖИМ РЕДАКТИРОВАНИЯ: Поле ввода
          <input
            type="text"
            className="task-name-input"
            value={name}
            onChange={(e) => setName(e.target.value)}
            onBlur={handleSave}
            onKeyDown={handleKeyDown}
            // Фокусируемся только если перешли в режим редактирования и элемент активен
            autoFocus
          />
        ) : (
          // ОБЫЧНЫЙ РЕЖИМ: Отображение текста (например, "Элемент внутри колонки")
          <div className="task-name-display">
            {task.cardName || "Элемент внутри колонки"}
          </div>
        )}
      </div>
      {/* 🔑 Блок для рендеринга подзадач */}
      {task.subTasks && task.subTasks.length > 0 && (
        <div className="subtasks-list">
          {task.subTasks.map((subtask) => (
            <SubTaskItem
              key={subtask.subTaskId}
              subtask={subtask}
              taskId={task.cardId}
              updateSubTaskStatus={updateSubTaskStatus}
            />
          ))}
        </div>
      )}
      {/* ... Остальной контент задачи: подзадачи, приоритет, дедлайн и т.д. ... */}
    </div>
  );
};

// 🔑 НОВЫЙ КОМПОНЕНТ: Элемент Подзадачи с чекбоксом
const SubTaskItem = ({ subtask, taskId, updateSubTaskStatus }) => {
  const [isChecked, setIsChecked] = useState(subtask.isCompleted);

  // Синхронизация: если внешние данные (контекст) изменились, обновляем локальное состояние
  useEffect(() => {
    setIsChecked(subtask.isCompleted);
  }, [subtask.isCompleted]);

  const handleToggle = useCallback(
    async (e) => {
      if (!updateSubTaskStatus) {
        console.error(
          "❌ Ошибка: updateSubTaskStatus не предоставлена из контекста."
        );
        return;
      }
      const newStatus = e.target.checked;

      // Оптимистичное обновление: сразу меняем состояние
      setIsChecked(newStatus);

      // Отправка запроса на сервер
      const success = await updateSubTaskStatus(
        taskId,
        subtask.subTaskId,
        newStatus
      );

      // Откат при ошибке
      if (!success) {
        setIsChecked(!newStatus);
      }
    },
    [taskId, subtask.subTaskId, updateSubTaskStatus]
  );

  return (
    <div className={`subtask-item ${isChecked ? "completed" : ""}`}>
      <input
        type="checkbox"
        checked={isChecked}
        onChange={handleToggle}
        id={`subtask-${subtask.subTaskId}`}
        // Отключаем клик, если идет загрузка или не хотим разрешать
        disabled={false} // Можете добавить логику дизейбла при необходимости
      />
      <label htmlFor={`subtask-${subtask.subTaskId}`} className="subtask-name">
        {subtask.subTaskName}
      </label>
    </div>
  );
};

export default TaskItem;
