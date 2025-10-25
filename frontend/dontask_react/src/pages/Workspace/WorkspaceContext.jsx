// WorkspaceContext.jsx

import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
} from "react";
import { useParams, useLocation } from "react-router-dom";
import { useToast } from "../../components/Toast/ToastContext";
import {
  fetchWithAuth,
  postWithAuth,
  patchWithAuth,
} from "../../service/api.js";

// --- Вспомогательные функции для генерации уникальных ID ---
const generateUniqueId = (prefix = "temp") =>
  `${prefix}-${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;

const WorkspaceContext = createContext(null);

export const useWorkspace = () => {
  const context = useContext(WorkspaceContext);
  if (context === undefined) {
    throw new Error(
      "useWorkspace должен использоваться внутри WorkspaceProvider"
    );
  }
  return context;
};

// 🔑 НОРМАЛИЗАЦИЯ ДАННЫХ
const normalizeWorkspaceData = (data) => {
  if (!data) return data;

  const rawLists = data.boardLists || data.cards;

  if (!rawLists || !Array.isArray(rawLists)) {
    console.warn(
      "Данные WorkSpace не содержат ожидаемого массива колонок (boardLists или cards)."
    );
    return { ...data, boardLists: [] };
  }

  const normalizedLists = rawLists.map((list) => {
    const listId = String(
      list.CardId || list.cardId || list.listId || generateUniqueId("list")
    );

    const normalizedCards = (list.tasks || list.cards || []).map((card) => {
      // НОРМАЛИЗАЦИЯ ПОДЗАДАЧ
      const normalizedSubTasks = (card.subTasks || []).map((subtask) => ({
        ...subtask,
        subTaskId: String(
          subtask.subTaskId || subtask.SubTaskId || generateUniqueId("subtask")
        ),
        subTaskName:
          subtask.CardName ||
          subtask.cardName ||
          subtask.subTaskName ||
          "Новая подзадача",
        isCompleted: subtask.isCompleted || false,
      }));

      const idFromServer = card.cardId || card.CardId;
      const finalCardId = idFromServer
        ? String(idFromServer)
        : generateUniqueId("temp");

      return {
        ...card,
        cardId: finalCardId,
        subTasks: normalizedSubTasks,
      };
    });

    return {
      ...list,
      listId: listId,
      listName: list.CardName || list.cardName || list.listName,
      cards: normalizedCards,
    };
  });

  return {
    ...data,
    boardLists: normalizedLists,
  };
};

export const WorkspaceProvider = ({ children }) => {
  const { boardId } = useParams();
  const location = useLocation();
  const incomingProjectId = location.state?.projectId;
  const incomingProjectName = location.state?.projectName;
  const showToast = useToast();

  const [workspaceData, setWorkspaceData] = useState(null);
  const [loading, setLoading] = useState(true);

  // --- Функция обновления данных рабочей области ---
  const updateWorkspaceData = useCallback((newData) => {
    setWorkspaceData((prevData) => {
      if (!prevData) return prevData;

      let newBoardLists = [...(prevData.boardLists || [])];

      // 1. ЛОГИКА ДОБАВЛЕНИЯ НОВОЙ КОЛОНКИ
      if (newData.newCard) {
        const listIdFromServer =
          newData.newCard.CardId || newData.newCard.cardId;

        const normalizedNewList = {
          listId: String(listIdFromServer || generateUniqueId("list")),
          listName:
            newData.newCard.CardName ||
            newData.newCard.cardName ||
            "Новая карточка",
          ...newData.newCard,
          cards: newData.newCard.cards || newData.newCard.tasks || [],
        };

        if (!normalizedNewList.listId) {
          console.error(
            "Ошибка: Не удалось получить ID для новой колонки. Обновление состояния отменено."
          );
          return prevData;
        }

        const finalLists = [...newBoardLists, normalizedNewList];

        console.log(
          "✅ Добавлена новая Колонка/Список ('Карточка'):",
          normalizedNewList
        );

        return {
          ...prevData,
          boardLists: finalLists,
        };
      }

      // 2. ЛОГИКА ДОБАВЛЕНИЯ НОВОЙ ЗАДАЧИ
      if (newData.newTask && newData.listId) {
        const { newTask, listId } = newData;

        const targetListIndex = newBoardLists.findIndex(
          (l) => l.listId === listId
        );

        if (targetListIndex === -1) {
          console.error(
            `Не удалось найти список с ID: ${listId} для добавления задачи.`
          );
          return prevData;
        }

        const taskIdFromServer =
          newData.newTask.cardId || newData.newTask.CardId;

        if (!taskIdFromServer || isNaN(parseInt(taskIdFromServer))) {
          console.error(
            "Ошибка: Сервер не вернул корректный числовой ID для новой задачи. Требуется проверка бэкенда."
          );
          return prevData;
        }

        const normalizedNewTask = {
          cardId: String(taskIdFromServer),
          cardName:
            newData.newTask.CardName ||
            newData.newTask.cardName ||
            "Новая задача",
          ...newData.newTask,
          subTasks: newData.newTask.subTasks || [],
        };

        if (!normalizedNewTask.cardId) {
          console.error(
            "Ошибка: Не удалось получить ID для новой задачи. Обновление состояния отменено."
          );
          return prevData;
        }

        const targetList = newBoardLists[targetListIndex];

        const updatedList = {
          ...targetList,
          cards: [...(targetList.cards || []), normalizedNewTask],
        };

        const finalLists = [
          ...newBoardLists.slice(0, targetListIndex),
          updatedList,
          ...newBoardLists.slice(targetListIndex + 1),
        ];

        console.log(
          `✅ Добавлена новая задача в список ${listId}:`,
          normalizedNewTask
        );

        return {
          ...prevData,
          boardLists: finalLists,
        };
      }

      // 3. ЛОГИКА ДОБАВЛЕНИЯ НОВОЙ ПОДЗАДАЧИ
      if (newData.newSubTask && newData.taskId) {
        const { newSubTask, taskId } = newData;

        let targetListIndex = -1;
        let targetCardIndex = -1;

        for (let i = 0; i < newBoardLists.length; i++) {
          targetCardIndex = newBoardLists[i].cards.findIndex(
            (c) => String(c.cardId) === String(taskId)
          );
          if (targetCardIndex !== -1) {
            targetListIndex = i;
            break;
          }
        }

        if (targetListIndex === -1) {
          console.error(
            `Не удалось найти задачу с ID: ${taskId} для добавления подзадачи.`
          );
          return prevData;
        }

        const subTaskIdFromServer =
          newSubTask.subTaskId || newSubTask.SubTaskId;

        const normalizedNewSubTask = {
          subTaskId: String(subTaskIdFromServer || generateUniqueId("subtask")),
          subTaskName:
            newSubTask.SubTaskName ||
            newSubTask.subTaskName ||
            "Новая подзадача",
          isCompleted: newSubTask.isCompleted || false,
          ...newSubTask,
        };

        if (!normalizedNewSubTask.subTaskId) {
          console.error(
            "Ошибка: Не удалось получить ID для новой подзадачи. Обновление состояния отменено."
          );
          return prevData;
        }

        const targetList = newBoardLists[targetListIndex];
        const targetCard = targetList.cards[targetCardIndex];

        const updatedSubTasks = [
          ...(targetCard.subTasks || []),
          normalizedNewSubTask,
        ];

        const updatedCard = {
          ...targetCard,
          subTasks: updatedSubTasks,
        };

        const updatedCards = [
          ...targetList.cards.slice(0, targetCardIndex),
          updatedCard,
          ...targetList.cards.slice(targetCardIndex + 1),
        ];

        const updatedList = {
          ...targetList,
          cards: updatedCards,
        };

        const finalLists = [
          ...newBoardLists.slice(0, targetListIndex),
          updatedList,
          ...newBoardLists.slice(targetListIndex + 1),
        ];

        console.log(
          `✅ Добавлена новая подзадача в задачу ${taskId}:`,
          normalizedNewSubTask
        );

        return {
          ...prevData,
          boardLists: finalLists,
        };
      }

      return { ...prevData, ...newData };
    });
  }, []);

  // ----------------------------------------------------------------------
  // ФУНКЦИИ ЗАГРУЗКИ И ИЗВЛЕЧЕНИЯ ДАННЫХ
  // ----------------------------------------------------------------------

  const fetchWorkspaceData = useCallback(
    async (id) => {
      if (!id) return;
      setLoading(true);
      try {
        const data = await fetchWithAuth(`/GetPages/GetWorkSpacePage/${id}`);
        const normalizedData = normalizeWorkspaceData(data);

        setWorkspaceData(normalizedData);

        console.log(
          "Данные рабочей области успешно получены и нормализованы:",
          normalizedData
        );
      } catch (err) {
        console.error(
          "Ошибка при получении данных WorkSpace:",
          err.response || err.message
        );
        showToast(
          "Не удалось загрузить рабочую область. Попробуйте снова.",
          "error"
        );
      } finally {
        setLoading(false);
      }
    },
    [showToast]
  );

  useEffect(() => {
    fetchWorkspaceData(boardId);
  }, [boardId, fetchWorkspaceData]);

  const projectIdFromApi = workspaceData?.projectId;
  const projectId = projectIdFromApi || incomingProjectId;
  const projectNameFromApi = workspaceData?.projectName;
  const projectName =
    projectNameFromApi || incomingProjectName || "Загрузка проекта...";
  const boardName = workspaceData?.boardName || "Загрузка доски...";
  const members = workspaceData?.members || [];
  const lists = workspaceData?.boardLists || [];

  // 🔑 ФУНКЦИЯ ДЛЯ ПЕРЕКЛЮЧЕНИЯ СТАТУСА ПОДЗАДАЧИ
  const toggleSubTaskStatus = useCallback(async (subTaskId, isCompleted) => {
    if (!projectId || !boardId) {
      console.error("Отсутствует Project ID или Board ID.", { projectId, boardId });
      return false;
    }

    try {
      // Оптимистичное обновление UI
      setWorkspaceData(prevData => {
        if (!prevData?.boardLists) return prevData;
        
        return {
          ...prevData,
          boardLists: prevData.boardLists.map(list => ({
            ...list,
            cards: list.cards.map(card => ({
              ...card,
              subTasks: card.subTasks.map(subTask => 
                subTask.subTaskId === subTaskId 
                  ? { ...subTask, isCompleted }
                  : subTask
              )
            }))
          }))
        };
      });

      // Отправляем запрос на сервер
      const url = `/project/${projectId}/board/${boardId}/Task/UpdateSubTaskStatus/${subTaskId}`;
      
      const response = await patchWithAuth(url, { isCompleted });

      showToast("Статус подзадачи обновлен!", "success");
      return true;
    } catch (err) {
      console.error("Ошибка при обновлении статуса подзадачи:", err);
      
      // Откатываем изменения при ошибке
      setWorkspaceData(prevData => {
        if (!prevData?.boardLists) return prevData;
        
        return {
          ...prevData,
          boardLists: prevData.boardLists.map(list => ({
            ...list,
            cards: list.cards.map(card => ({
              ...card,
              subTasks: card.subTasks.map(subTask => 
                subTask.subTaskId === subTaskId 
                  ? { ...subTask, isCompleted: !isCompleted }
                  : subTask
              )
            }))
          }))
        };
      });
      
      showToast("Не удалось обновить статус подзадачи", "error");
      return false;
    }
  }, [projectId, boardId, showToast]);

  // ----------------------------------------------------------------------
  // ФУНКЦИИ API ДЛЯ СОЗДАНИЯ
  // ----------------------------------------------------------------------

  const createCard = useCallback(async () => {
    if (!projectId || !boardId) {
      console.error("Отсутствует Project ID или Board ID.");
      showToast(
        "Недостаточно данных для создания карточки (колонки).",
        "error"
      );
      return;
    }

    const url = `/project/${projectId}/board/${boardId}/Card/CreateCard`;

    const payload = {
      CardName: "Новая карточка",
    };

    try {
      const newCard = await postWithAuth(url, payload, {
        headers: { "Content-Type": "application/json" },
      });

      showToast("Новая колонка успешно создана!", "success");

      updateWorkspaceData({ newCard: newCard });

      return newCard;
    } catch (err) {
      console.error(
        "Ошибка при создании карточки (колонки):",
        err.response || err.message
      );
      showToast(
        "Ошибка при создании карточки (колонки). Попробуйте снова.",
        "error"
      );
      throw err;
    }
  }, [projectId, boardId, showToast, updateWorkspaceData]);

  const createTask = useCallback(
    async (listId, taskName = "Новая задача") => {
      if (!projectId || !boardId || !listId) {
        console.error("Отсутствует Project ID, Board ID или List ID.", {
          projectId,
          boardId,
          listId,
        });
        showToast("Недостаточно данных для создания задачи.", "error");
        return;
      }

      const baseUrl = `/project/${projectId}/board/${boardId}/Task/CreateTask?cardId=${listId}`;
      const payload = {};

      try {
        const newTask = await postWithAuth(baseUrl, payload, {
          headers: { "Content-Type": "application/json" },
        });

        console.log("проверка ", newTask);

        const taskIdFromServer = newTask?.taskId;

        if (!taskIdFromServer || isNaN(parseInt(taskIdFromServer))) {
          throw new Error(
            "Сервер не вернул корректный числовой ID для новой задачи."
          );
        }

        const numericIdString = String(taskIdFromServer);

        showToast("Новая задача успешно создана!", "success");

        updateWorkspaceData({
          newTask: { ...newTask, cardId: numericIdString },
          listId: listId,
        });

        return { ...newTask, cardId: numericIdString };
      } catch (err) {
        console.error(
          "Ошибка при создании задачи:",
          err.response || err.message
        );
        showToast(
          "Ошибка при создании задачи. Пожалуйста, проверьте консоль.",
          "error"
        );
        throw err;
      }
    },
    [projectId, boardId, showToast, updateWorkspaceData]
  );

  const createSubTask = useCallback(
    async (taskId, subTaskName = "Новая подзадача") => {
      console.log(
        `🚀 [SubTask] Попытка создать подзадачу. Task ID (исходный): ${taskId}`
      );

      if (!projectId || !boardId || !taskId) {
        console.error("Отсутствует Project ID, Board ID или Task ID.");
        showToast("Недостаточно данных для создания подзадачи.", "error");
        return;
      }

      const numericTaskId = parseInt(taskId);

      console.log(`🚀 [SubTask] Task ID после parseInt: ${numericTaskId}`);

      if (isNaN(numericTaskId) || numericTaskId <= 0) {
        console.error(
          `❌ [SubTask] БЛОКИРОВКА: Task ID не является числовым: ${taskId}. Отмена API-вызова.`
        );
        showToast(
          "Не удалось создать подзадачу: Сначала дождитесь сохранения задачи на сервере.",
          "error"
        );
        return;
      }

      console.log(
        `✅ [SubTask] Task ID прошел валидацию: ${numericTaskId}. Выполняется API-вызов.`
      );

      const url = `/project/${projectId}/board/${boardId}/Task/CreateSubTask?taskId=${numericTaskId}`;

      const payload = {
        SubTaskName: subTaskName,
      };

      try {
        const newSubTask = await postWithAuth(url, payload, {
          headers: { "Content-Type": "application/json" },
        });

        showToast("Новая подзадача успешно создана!", "success");

        updateWorkspaceData({ newSubTask: newSubTask, taskId: taskId });

        return newSubTask;
      } catch (err) {
        throw err;
      }
    },
    [projectId, boardId, showToast, updateWorkspaceData]
  );

  // ----------------------------------------------------------------------
  // 🔑 ФУНКЦИЯ API: PATCH для обновления названий задач/колонок
  // ----------------------------------------------------------------------

  const updateCardNames = useCallback(
    async (cardsToUpdate) => {
      if (!projectId || !boardId || cardsToUpdate.length === 0) {
        console.error(
          "Отсутствует Project ID, Board ID или нет данных для обновления."
        );
        return false;
      }

      const url = `/project/${projectId}/board/${boardId}/Card/ChangeCardsNames`;

      const cardArray = cardsToUpdate.map((item) => ({
        cardId: parseInt(item.id),
        cardName: item.name,
      }));

      const validCardArray = cardArray.filter(
        (item) => !isNaN(item.cardId) && item.cardId > 0
      );

      if (validCardArray.length === 0) {
        console.log("Нет сохраненных элементов для отправки PATCH-запроса.");
        return true;
      }

      const finalPayload = {
        cards: validCardArray,
      };

      try {
        await patchWithAuth(url, finalPayload);

        setWorkspaceData((prevData) => {
          if (!prevData || !prevData.boardLists) return prevData;

          const updatesMap = new Map(
            cardsToUpdate.map((card) => [String(card.id), card.name])
          );

          const newBoardLists = prevData.boardLists.map((list) => {
            const listId = String(list.listId);
            let isListUpdated = false;
            let updatedCards = list.cards;

            if (updatesMap.has(listId)) {
              isListUpdated = true;
              list = { ...list, listName: updatesMap.get(listId) };
            }

            updatedCards = list.cards.map((card) => {
              const cardId = String(card.cardId);
              if (updatesMap.has(cardId)) {
                isListUpdated = true;
                return { ...card, cardName: updatesMap.get(cardId) };
              }
              return card;
            });

            if (isListUpdated) {
              return { ...list, cards: updatedCards };
            }
            return list;
          });

          return { ...prevData, boardLists: newBoardLists };
        });

        showToast("Название карточки(ок) успешно изменено!", "success");
        return true;
      } catch (err) {
        console.error(
          "Ошибка при обновлении названий (PATCH):",
          err.response || err.message
        );
        showToast("Ошибка при сохранении изменений.", "error");
        return false;
      }
    },
    [projectId, boardId, showToast]
  );

  // ----------------------------------------------------------------------
  // КОНТЕКСТ
  // ----------------------------------------------------------------------

  const contextValue = {
    workspaceData,
    loading,
    projectName,
    boardName,
    members,
    projectId,
    boardId,
    createCard,
    createTask,
    createSubTask,
    fetchWorkspaceData,
    lists,
    updateCardNames,
    toggleSubTaskStatus,
  };

  return (
    <WorkspaceContext.Provider value={contextValue}>
      {children}
    </WorkspaceContext.Provider>
  );
};