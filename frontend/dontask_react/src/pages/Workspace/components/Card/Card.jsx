import React, { useState, useCallback, useMemo } from "react";
import { useWorkspace } from "../../WorkspaceContext.jsx";
import add_icon from "./add_icon.png";
import "./Card.css";
import edit_icon from "./edit_icon.png";
import priority_icon from "./priorities.png";
import deadline_icon from "./deadline.png";
import { useWorkspaceEdit } from "../../WorkspaceEditContext.jsx";

// Вспомогательная функция для форматирования даты
const formatDate = (dateString) => {
  if (!dateString) return null;
  const date = new Date(dateString);
  if (isNaN(date.getTime())) return null;

  const day = date.getDate().toString().padStart(2, "0");
  const month = (date.getMonth() + 1).toString().padStart(2, "0");
  return `${day}.${month}`;
};

// Расчет крайнего срока по подзадачам
const calculateDeadline = (subTasks) => {
  if (!subTasks || subTasks.length === 0) {
    return null;
  }

  const deadlines = subTasks
    .map((subtask) => subtask.deadline)
    .filter((d) => d)
    .map((d) => new Date(d).getTime())
    .filter((t) => !isNaN(t));

  if (deadlines.length === 0) {
    return null;
  }

  const latestTimestamp = Math.max(...deadlines);
  return formatDate(latestTimestamp);
};

// Компонент для отображения одной ПОДЗАДАЧИ (SubTask)
const SubTaskItem = ({ subtask }) => {
  const { toggleSubTaskStatus, loading } = useWorkspace();
  const [isUpdating, setIsUpdating] = useState(false);

  console.log("🔴 SubTaskItem РЕНДЕРИТСЯ", { 
    subTaskId: subtask.subTaskId,
    isCompleted: subtask.isCompleted 
  });

  const handleToggle = async (e) => {
    const newStatus = e.target.checked;
    
    console.log("🔴🔴🔴 ЧЕКБОКС НАЖАТ!", {
      subTaskId: subtask.subTaskId,
      newStatus,
      currentStatus: subtask.isCompleted,
      isUpdating,
      loading
    });

    if (isUpdating || loading) {
      console.log("🔴 БЛОКИРОВКА: уже обновляется или загрузка");
      return;
    }

    setIsUpdating(true);
    
    try {
      console.log("🔴 ВЫЗОВ toggleSubTaskStatus...");
      await toggleSubTaskStatus(subtask.subTaskId, newStatus);
      console.log("🔴 toggleSubTaskStatus ВЫПОЛНЕНА");
    } catch (error) {
      console.error("🔴 ОШИБКА В handleToggle:", error);
    } finally {
      setIsUpdating(false);
    }
  };

  return (
    <div className="subtask-item">
      <input
        type="checkbox"
        checked={subtask.isCompleted}
        onChange={handleToggle}
        disabled={isUpdating || loading}
        className="subtask-checkbox"
      />
      <span className={`subtask-name ${subtask.isCompleted ? "completed" : ""}`}>
        {subtask.subTaskName}
      </span>
    </div>
  );
};

// Компонент для отображения существующей ЗАДАЧИ (Task)
const TaskItem = ({ card }) => {
  const { createSubTask, loading } = useWorkspace();
  // 🔑 ИСПРАВЛЕНИЕ: используем отдельные состояния для задач
  const { isEditMode, taskNameChanges, updateTaskChanges } = useWorkspaceEdit();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const taskIdForApi = card.taskId;
  const isTemporaryId = String(taskIdForApi).startsWith("temp-");

  // 🔑 ИСПРАВЛЕНИЕ: используем taskNameChanges вместо cardNameChanges
  const currentTaskName =
    taskNameChanges[taskIdForApi] !== undefined
      ? taskNameChanges[taskIdForApi]
      : card.cardName || "Новая задача";

  const handleNameChange = useCallback(
    (e) => {
      if (isTemporaryId) return;
      // 🔑 ИСПРАВЛЕНИЕ: используем updateTaskChanges вместо updateCardChanges
      updateTaskChanges(taskIdForApi, e.target.value);
    },
    [taskIdForApi, updateTaskChanges, isTemporaryId]
  );

  const handleCreateSubTask = useCallback(async () => {
    if (loading || isSubmitting) {
      console.warn("Попытка создать подзадачу во время загрузки или сабмита.");
      return;
    }

    setIsSubmitting(true);
    try {
      await createSubTask(taskIdForApi);
    } catch (e) {
      // Ошибка будет обработана в контексте с помощью Toast
    } finally {
      setIsSubmitting(false);
    }
  }, [taskIdForApi, createSubTask, loading, isSubmitting]);

  const subtasksToRender = card.subTasks || [];
  const taskDeadline = calculateDeadline(subtasksToRender);
  const taskPriority = card.priority || "Высокий";

  const totalSubtasks = subtasksToRender.length;
  const completedSubtasks = subtasksToRender.filter(
    (s) => s.isCompleted
  ).length;
  const progressPercent =
    totalSubtasks > 0
      ? Math.round((completedSubtasks / totalSubtasks) * 100)
      : 0;

  return (
    <div key={card.cardId} className="list-card-item">
      <div className="task-header-row">
        {isEditMode ? (
          <input
            type="text"
            className="list-task-input-edit"
            value={currentTaskName}
            onChange={handleNameChange}
          />
        ) : (
          <div className="list-task-content">
            {card.cardName || "Новая задача"}
          </div>
        )}
        {taskDeadline && (
          <div className="task-info-compact-indicator">{taskDeadline}</div>
        )}
      </div>
      <div className="task-description-placeholder">
        Место для дополнительной информации и описания задачи
      </div>
      <div className="progress-bar-container">
        <div
          className="progress-bar-fill"
          style={{ width: `${progressPercent}%` }}
        ></div>
      </div>
      <div className="list-subtask">
        <div className="subtask-progress-status">
          {totalSubtasks > 0 ? (
            <span className="subtask-status-text">
              &#10003; {completedSubtasks} из {totalSubtasks}
            </span>
          ) : (
            <span className="subtask-status-text-empty">&#9711; 0 из 0</span>
          )}
        </div> подзадач:
{subtasksToRender.map((subtask) => (
  <SubTaskItem 
    key={subtask.subTaskId} 
    subtask={subtask}
    // 🔑 projectId и boardId больше не передаем пропсами - берем из контекста
  />
))}

        <button
          type="button"
          className="add-subtask-button"
          onClick={handleCreateSubTask}
          disabled={loading || isSubmitting}
        >
          {isSubmitting ? "Создание..." : "+ Добавить подзадачу"}
        </button>
      </div>
      <div className="task-details-footer">
        <div
          className={`task-footer-item priority-footer priority-${taskPriority.toLowerCase()}`}
        >
          <img src={priority_icon} alt="Приоритет" className="indicator-icon" />
          <span className="priority-text">{taskPriority}</span>
        </div>

        {taskDeadline && (
          <div className="task-footer-item deadline-footer">
            <img src={deadline_icon} alt="Дедлайн" className="indicator-icon" />
            <span className="deadline-text">{taskDeadline}</span>
          </div>
        )}

        <div className="task-footer-item members-footer"></div>
      </div>
    </div>
  );
};

// Компонент для отображения одной "Карточки" (Колонки/Списка)
const ListColumn = ({ list }) => {
  const { createTask, loading } = useWorkspace();
  // 🔑 ИСПРАВЛЕНИЕ: используем отдельные состояния для колонок
  const { isEditMode, listNameChanges, updateListChanges } = useWorkspaceEdit();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const listIdForApi = list.listId;
  const isListSaved = !String(listIdForApi).startsWith("temp-");

  // 🔑 ИСПРАВЛЕНИЕ: используем listNameChanges вместо cardNameChanges
  const currentListName =
    listNameChanges[listIdForApi] !== undefined
      ? listNameChanges[listIdForApi]
      : list.listName || `ID: ${list.listId}`;

  const handleNameChange = useCallback(
    (e) => {
      if (!isListSaved) return;
      // 🔑 ИСПРАВЛЕНИЕ: используем updateListChanges вместо updateCardChanges
      updateListChanges(listIdForApi, e.target.value);
    },
    [listIdForApi, updateListChanges, isListSaved]
  );

  const handleCreateTask = async () => {
    if (loading || isSubmitting) return;
    setIsSubmitting(true);
    try {
      await createTask(list.listId);
    } catch (e) {
      // Ошибка будет обработана в контексте с помощью Toast
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="existing-list-container">
      <div className="list-header-wrapper">
        {isEditMode ? (
          <input
            type="text"
            className="list-title-input-edit"
            value={currentListName}
            onChange={handleNameChange}
          />
        ) : (
          <h3 className="list-title">
            {list.listName || `ID: ${list.listId}`}
          </h3>
        )}
      </div>
      <div className="list-cards-wrapper">
        {(list.cards || []).map((card) => (
          <TaskItem key={card.cardId} card={card} />
        ))}
        <button
          type="button"
          className="add-task-button"
          onClick={handleCreateTask}
          disabled={loading || isSubmitting}
        >
          {isSubmitting ? "Создание..." : "+ Добавить задачу"}
        </button>
      </div>
    </div>
  );
};

// Основной компонент доски (Card)
export default function Card() {
  const { createCard, loading, lists } = useWorkspace();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleCreateCardColumn = async () => {
    if (loading || isSubmitting) return;

    setIsSubmitting(true);
    try {
      await createCard();
    } catch (e) {
      // Ошибка будет обработана в контексте с помощью Toast
    } finally {
      setIsSubmitting(false);
    }
  };

  const listsToRender = lists || [];

  return (
    <div className="card-container">
      {listsToRender.map((list) => (
        <ListColumn key={list.listId} list={list} />
      ))}
      <button
        type="button"
        className="list-create-container list-create-button-style"
        onClick={handleCreateCardColumn}
        disabled={loading || isSubmitting}
      >
        <>
          <img
            src={add_icon}
            alt="Добавить карточку (колонку)"
            className="card-create-icon"
          />
          <p className="card-create-text">Добавить карточку</p>
        </>
      </button>
    </div>
  );
}