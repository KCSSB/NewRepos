import React, {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
} from "react";
import { useParams, useLocation } from "react-router-dom";
import { useToast } from "../../components/Toast/ToastContext";
import { fetchWithAuth, postWithAuth } from "../../service/api.js";

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

export const WorkspaceProvider = ({ children }) => {
  // Получаем boardId из URL
  const { boardId } = useParams();

  // Получаем данные состояния (state) из навигации (там должны быть projectId и projectName)
  const location = useLocation();
  const incomingProjectId = location.state?.projectId; // ID проекта, переданный из BoardCard
  const incomingProjectName = location.state?.projectName; // Name проекта, переданный из BoardCard

  const showToast = useToast();

  const [workspaceData, setWorkspaceData] = useState(null);
  const [loading, setLoading] = useState(true);

  // --- Функция загрузки данных ---
  const fetchWorkspaceData = useCallback(
    async (id) => {
      if (!id) return;
      setLoading(true);
      setWorkspaceData(null);
      try {
        const data = await fetchWithAuth(`/GetPages/GetWorkSpacePage/${id}`);
        setWorkspaceData(data);
        console.log("Данные рабочей области успешно получены:", data);

        // Логирование для отладки
      } catch (err) {
        console.error(
          "Ошибка при получении данных WorkSpace:",
          err.response || err.message
        );
        showToast(
          "Не удалось загрузить рабочую область. Пожалуйста, попробуйте снова.",
          "error"
        );
      } finally {
        setLoading(false);
      }
    },
    // Добавляем incomingProjectId и incomingProjectName в зависимости
    [showToast, incomingProjectId, incomingProjectName]
  );

  useEffect(() => {
    fetchWorkspaceData(boardId);
  }, [boardId, fetchWorkspaceData]);

  // --- Данные, извлеченные из workspaceData ---

  // Приоритет ID: API -> State -> undefined
  const projectIdFromApi = workspaceData?.projectId;
  const projectId = projectIdFromApi || incomingProjectId;

  // Приоритет Name: API -> State -> Заглушка
  const projectNameFromApi = workspaceData?.projectName;
  const projectName =
    projectNameFromApi || incomingProjectName || "Загрузка проекта...";

  const boardName = workspaceData?.boardName || "Загрузка доски...";
  const members = workspaceData?.workSpaceMembers || [];

  // --- Функция создания карточки ---
  const createCard = useCallback(async () => {
    // Проверка, что ID проекта и доски доступны (projectId теперь должен работать)
    if (!projectId || !boardId) {
      console.error("Отсутствует Project ID или Board ID.", {
        projectId,
        boardId,
      });
      showToast(
        "Недостаточно данных для создания карточки. Проверьте консоль.",
        "error"
      );
      return;
    }

    // API-путь: /project/{projectId}/board/{boardId}/Card/CreateCard
    const url = `/project/${projectId}/board/${boardId}/Card/CreateCard`;

    const payload = {
      CardName: "Новая карточка",
    };

    try {
      const newCard = await postWithAuth(url, payload, {
        headers: {
          "Content-Type": "application/json",
        },
      });

      showToast("Новая карточка успешно создана!", "success");
      console.log("Новая карточка создана:", newCard);

      // TODO: Здесь должна быть логика обновления состояния доски (списков/карточек)

      return newCard;
    } catch (err) {
      console.error(
        "Ошибка при создании карточки:",
        err.response || err.message
      );
      showToast("Ошибка при создании карточки. Попробуйте снова.", "error");
      throw err;
    }
  }, [projectId, boardId, showToast]);

  const contextValue = {
    workspaceData,
    loading,
    projectName,
    boardName,
    members,
    projectId, // <-- Выставленный Project ID
    createCard,
    fetchWorkspaceData,
  };

  return (
    <WorkspaceContext.Provider value={contextValue}>
      {children}
    </WorkspaceContext.Provider>
  );
};
