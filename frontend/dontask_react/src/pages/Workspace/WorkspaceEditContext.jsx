import React, { createContext, useContext, useState, useCallback } from "react";
import { useToast } from "../../components/Toast/ToastContext";
import { useWorkspace } from "./WorkspaceContext.jsx";

const WorkspaceEditContext = createContext(null);

export const WorkspaceEditProvider = ({ children }) => {
  const showToast = useToast();
  const { updateCardNames, fetchWorkspaceData } = useWorkspace();

  const [isEditMode, setIsEditMode] = useState(false);
  
  // 🔑 РАЗДЕЛЕНИЕ: отдельные состояния для колонок и задач
  const [listNameChanges, setListNameChanges] = useState({});
  const [taskNameChanges, setTaskNameChanges] = useState({});

  /**
   * Обновляет название колонки (списка)
   */
  const updateListChanges = useCallback((listId, newName) => {
    setListNameChanges((prevChanges) => ({
      ...prevChanges,
      [listId]: newName,
    }));
  }, []);

  /**
   * Обновляет название задачи (карточки)
   */
  const updateTaskChanges = useCallback((taskId, newName) => {
    setTaskNameChanges((prevChanges) => ({
      ...prevChanges,
      [taskId]: newName,
    }));
  }, []);

  /**
   * 🚀 ФУНКЦИЯ: Применяет изменения, отправляет PATCH-запрос и выходит из режима.
   */
  const applyChanges = useCallback(async () => {
    console.log("Применение изменений...");

    // 🔑 РАЗДЕЛЕНИЕ: объединяем изменения из обоих состояний
    const listChangesArray = Object.entries(listNameChanges)
      .map(([id, name]) => ({ id, name }))
      .filter((item) => String(item.id).length > 0 && item.name.trim().length > 0);

    const taskChangesArray = Object.entries(taskNameChanges)
      .map(([id, name]) => ({ id, name }))
      .filter((item) => String(item.id).length > 0 && item.name.trim().length > 0);

    const allChanges = [...listChangesArray, ...taskChangesArray];

    if (allChanges.length > 0) {
      const success = await updateCardNames(allChanges);

      if (success) {
        showToast(
          `Успешно сохранено ${allChanges.length} изменений!`,
          "success"
        );
        fetchWorkspaceData();
      } else {
        showToast("Не удалось сохранить все изменения.", "error");
        return;
      }
    } else {
      showToast("Нет изменений для сохранения.", "info");
    }

    // 🔑 Очищаем оба состояния
    setListNameChanges({});
    setTaskNameChanges({});
    setIsEditMode(false);
  }, [listNameChanges, taskNameChanges, updateCardNames, fetchWorkspaceData, showToast]);

  /**
   * ФУНКЦИЯ: Отменяет все локальные изменения и выходит из режима.
   */
  const resetChanges = useCallback(() => {
    console.log("Отмена изменений. Откат локального состояния...");

    setListNameChanges({});
    setTaskNameChanges({});
    fetchWorkspaceData();
    showToast("Изменения отменены.", "info");

    setIsEditMode(false);
  }, [fetchWorkspaceData, showToast]);

  /**
   * Переключает режим редактирования.
   */
  const toggleEditMode = useCallback(() => {
    setIsEditMode((prev) => !prev);
    if (isEditMode) {
      resetChanges();
    }
  }, [isEditMode, resetChanges]);

  const value = {
    isEditMode,
    toggleEditMode,
    applyChanges,
    resetChanges,
    // 🔑 РАЗДЕЛЕНИЕ: передаем отдельные состояния и функции
    listNameChanges,
    taskNameChanges,
    updateListChanges,
    updateTaskChanges,
  };

  return (
    <WorkspaceEditContext.Provider value={value}>
      {children}
    </WorkspaceEditContext.Provider>
  );
};

export const useWorkspaceEdit = () => {
  const context = useContext(WorkspaceEditContext);
  if (context === undefined) {
    throw new Error(
      "useWorkspaceEdit должен использоваться внутри WorkspaceEditProvider"
    );
  }
  return context;
};