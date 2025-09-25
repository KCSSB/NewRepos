import React, { createContext, useContext, useState, useCallback } from "react";
import { useToast } from "../../components/Toast/ToastContext";
import { fetchWithAuth, patchWithAuth } from "../../service/api.js";

const ProjectContext = createContext(null);

export const ProjectProvider = ({ children }) => {
  const [projectData, setProjectData] = useState(null);
  const [initialProjectData, setInitialProjectData] = useState(null);
  // Сохраняет состояние проекта в момент входа в режим редактирования (EDIT)
  const [snapshotProjectData, setSnapshotProjectData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isFilteredByMember, setIsFilteredByMember] = useState(false);
  const [isEditMode, setIsEditMode] = useState(false);

  // Объект для хранения всех изменений, накопленных в режиме EDIT
  const [projectChanges, setProjectChanges] = useState({
    newProjectName: null,
    newProjectDescription: null,
    boardsToDelete: [],
    boardsToUpdate: [],
    membersToKick: [],
  });
  const showToast = useToast();

  const updateBoards = useCallback((newBoard) => {
    // Обновляем текущие данные, так как создание доски - это не "изменение",
    // которое должно отменяться кнопкой "Отменить", если было сделано до входа в EDIT.
    setProjectData((prevData) => {
      if (!prevData) return null;
      return {
        ...prevData,
        boards: [...(prevData.boards || []), newBoard],
      };
    });

    // ВАЖНО: Обновляем и снапшот, и initial, чтобы новая доска не удалялась
    // при перезагрузке или отмене, если она была создана вне режима EDIT.
    setSnapshotProjectData((prevData) => {
      if (!prevData) return null;
      return {
        ...prevData,
        boards: [...(prevData.boards || []), newBoard],
      };
    });
    setInitialProjectData((prevData) => {
      if (!prevData) return null;
      return {
        ...prevData,
        boards: [...(prevData.boards || []), newBoard],
      };
    });
  }, []);

  const toggleFilter = useCallback(() => {
    setIsFilteredByMember((prev) => !prev);
  }, []);

  // ------------------------------------------
  // ФУНКЦИИ УПРАВЛЕНИЯ ДАННЫМИ
  // ------------------------------------------

  // 1. Функция для обновления projectData (только UI-данных)
  const setProjectDataUI = useCallback((updater) => {
    setProjectData(updater);
  }, []);

  // 2. Функция для фиксации изменения названия в projectChanges
  const updateProjectNameChange = useCallback((newName) => {
    setProjectChanges((prev) => ({
      ...prev,
      newProjectName: newName,
    }));
  }, []);

  // ------------------------------------------
  // ФУНКЦИИ КНОПОК РЕЖИМА РЕДАКТИРОВАНИЯ
  // ------------------------------------------

  // ОТМЕНА ИЗМЕНЕНИЙ (Reset)
  const resetChanges = useCallback(() => {
    // 1. Откатываем UI-данные (projectData) к состоянию, сохраненному при входе в режим EDIT
    if (snapshotProjectData) {
      setProjectData(snapshotProjectData);
    } else if (initialProjectData) {
      // Запасной вариант на случай, если снапшот не был создан
      setProjectData(initialProjectData);
    }

    // 2. Сбрасываем все накопленные изменения
    setProjectChanges({
      newProjectName: null,
      newProjectDescription: null,
      boardsToDelete: [],
      boardsToUpdate: [],
      membersToKick: [],
    });
    // 3. Выходим из режима редактирования
    setIsEditMode(false);
    showToast("Последние изменения отменены");
  }, [snapshotProjectData, initialProjectData, showToast]);

  // ПРИМЕНЕНИЕ ИЗМЕНЕНИЙ (Confirm)
  const applyChanges = useCallback(async () => {
    const projectId = projectData?.projectId;
    if (!projectId) {
      setIsEditMode(false);
      return;
    }

    const promises = [];

    // 1. ОБНОВЛЕНИЕ НАЗВАНИЯ ПРОЕКТА
    if (projectChanges.newProjectName !== null) {
      const newName = projectChanges.newProjectName.trim();
      const originalName = initialProjectData.projectName.trim();

      // Отправляем запрос только если новое название отличается от оригинального
      if (newName && newName !== originalName) {
        const updateNamePromise = patchWithAuth(
          `/Projects/${projectId}/UpdateProjectName`,
          { updatedProjectName: newName } // Правильный ключ для бэкенда
        )
          .then(() => {
            showToast("Название проекта успешно обновлено! ✅");
            // Обновляем оригинальные данные после успешного запроса
            setInitialProjectData((prev) => ({
              ...prev,
              projectName: newName,
            }));
            // 💡 Обновляем snapshotProjectData, чтобы зафиксировать новую точку отката
            setSnapshotProjectData((prev) => ({
              ...prev,
              projectName: newName,
            }));
          })
          .catch((error) => {
            showToast("Ошибка при обновлении названия проекта", "error");
            console.error("Ошибка обновления названия:", error);
            // Если ошибка, откатываем UI-данные к оригинальному названию
            setProjectData((prev) => ({ ...prev, projectName: originalName }));
          });

        promises.push(updateNamePromise);
      }
    }

    // Ждем выполнения всех запросов (параллельное выполнение)
    await Promise.allSettled(promises);

    // Сбрасываем изменения
    setProjectChanges({
      newProjectName: null,
      newProjectDescription: null,
      boardsToDelete: [],
      boardsToUpdate: [],
      membersToKick: [],
    });

    // Выходим из режима редактирования
    setIsEditMode(false);
  }, [projectChanges, projectData, initialProjectData, showToast]);

  // 💡 ИЗМЕНЕННАЯ ФУНКЦИЯ: Сохраняем "снапшот" при входе
  const toggleEditMode = useCallback(() => {
    if (!isEditMode) {
      // 🚀 ВХОД В РЕЖИМ РЕДАКТИРОВАНИЯ:
      // Создаем глубокую копию текущих данных проекта.
      // JSON.parse(JSON.stringify()) - надежный способ скопировать объект.
      if (projectData) {
        setSnapshotProjectData(JSON.parse(JSON.stringify(projectData)));
      }
    } else {
      // 🛑 ВЫХОД ИЗ РЕЖИМА (через кнопку EDIT/RESET):
      resetChanges();
    }
    setIsEditMode((prev) => !prev);
  }, [isEditMode, resetChanges, projectData]);

  // ------------------------------------------
  // ФУНКЦИЯ ЗАГРУЗКИ ДАННЫХ
  // ------------------------------------------
  const setProjectDataAndInitial = useCallback((data) => {
    setProjectData(data);
    if (data) {
      setInitialProjectData(data); // Сохраняем оригинальные данные
      // При первичной загрузке, также устанавливаем snapshot
      setSnapshotProjectData(data);
    }
  }, []);

  const value = {
    projectData,
    setProjectData: setProjectDataAndInitial, // Для загрузки данных (Hall.jsx)
    setProjectDataUI, // Для изменения UI-данных (BoardHeader.jsx)
    loading,
    setLoading,
    showToast,
    updateBoards,
    isFilteredByMember,
    toggleFilter,
    isEditMode,
    toggleEditMode,
    resetChanges,
    applyChanges,
    projectChanges,
    updateProjectNameChange,
  };

  return (
    <ProjectContext.Provider value={value}>{children}</ProjectContext.Provider>
  );
};

export const useProject = () => {
  const context = useContext(ProjectContext);
  if (context === undefined) {
    throw new Error("useProject должен использоваться внутри ProjectProvider");
  }
  return context;
};
