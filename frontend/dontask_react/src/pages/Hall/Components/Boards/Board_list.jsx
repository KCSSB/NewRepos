import React, { useState } from "react";
import { useToast } from "../../../../components/Toast/ToastContext";
import { postWithAuth } from "../../../../service/api";
import { useProject } from "../../HallContext.jsx";
import create_board_icon from "./create_board_icon.png";
import sand_watches_icon from "./sand_watches_icon.png";
import people_icon from "./people_icon.png";
import deleteBoard_icon from "./deleteBoard_icon.png";
import "./Board_list.css";

const BOARD_COLORS = [
  "#8E8DFF",
  "#6868EE",
  "#EE6868",
  "#EEDB68",
  "#68EE68",
  "#EE68BB",
];

const BoardCard = ({
  board,
  index,
  isEditMode,
  handleDeleteBoard,
  setProjectData,
  showToast,
}) => {
  const [boardName, setBoardName] = useState(board.boardName);

  const formatDate = (dateString) => {
    if (!dateString) return null;
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    return `${day}.${month}`;
  };

  const handleCardClick = () => {
    if (isEditMode) {
      return;
    }
  };

  const handleBoardNameChange = (e) => {
    setBoardName(e.target.value);
  };

  const handleSaveBoardName = async (e) => {
    if (e.key === "Enter" || e.type === "blur") {
      e.target.blur();
      const trimmedName = boardName.trim();
      if (!trimmedName || trimmedName === board.boardName) {
        setBoardName(board.boardName);
        return;
      }

      // логика отправки нового имени доски на сервер
      // try {
      //    await putWithAuth(`/board/${board.boardId}/UpdateName`, { BoardName: trimmedName });
      //    setProjectData(prev => ({
      //      ...prev,
      //      boards: prev.boards.map(b =>
      //        b.boardId === board.boardId ? { ...b, boardName: trimmedName } : b
      //      )
      //    }));
      //    showToast("Название доски обновлено!", "success");
      // } catch (error) {
      //    showToast("Ошибка при обновлении названия доски.", "error");
      //    setBoardName(board.boardName);
      // }
      console.log(
        `Сохранение нового названия для доски ${board.boardId}:`,
        trimmedName
      );
    }
  };

  return (
    <div
      className={`board-card ${isEditMode ? "disabled-edit" : ""}`}
      style={{
        backgroundColor: BOARD_COLORS[index % BOARD_COLORS.length],
      }}
      onClick={handleCardClick}
    >
      <div className="board-card-top">
        <div className="board-card-title-container">
          {isEditMode ? (
            <input
              type="text"
              className="board-card-title-input"
              value={boardName}
              onChange={handleBoardNameChange}
              onBlur={handleSaveBoardName}
              spellCheck="false"
              onKeyDown={(e) => {
                if (e.key === "Enter") handleSaveBoardName(e);
              }}
            />
          ) : (
            <h5 className="board-card-title">{board.boardName}</h5>
          )}

          {isEditMode && (
            <button
              className="board-card-delete-button"
              onClick={() => handleDeleteBoard(board.boardId, board.boardName)}
            >
              <img
                src={deleteBoard_icon}
                alt="DELETE"
                className="board-card-delete"
              />
            </button>
          )}
        </div>
        <div className="board-card-info">
          {(board.dateOfStartwork || board.dateOfDeadline) && (
            <>
              <img
                src={sand_watches_icon}
                alt="DEADLINE"
                className="board-card-info watches"
              />
              <div className="board-card-info deadline">
                <p>
                  {board.dateOfStartwork
                    ? formatDate(board.dateOfStartwork)
                    : ""}
                  {board.dateOfStartwork && board.dateOfDeadline ? " - " : ""}
                  {board.dateOfDeadline ? formatDate(board.dateOfDeadline) : ""}
                </p>
              </div>
            </>
          )}
          <div className="board-card-info board-people">
            <img src={people_icon} alt="PEOPLE" />
            <p>{board.membersCount}</p>
          </div>
        </div>
      </div>
      <div className="board-card-bottom">
        <div className="progress-bar-container">
          <div
            className="progress-bar-fill"
            style={{ width: `${board.progressBar}%` }}
          ></div>
        </div>
        <p className="progress-text">{board.progressBar}%</p>
      </div>
    </div>
  );
};

export default function Board_list({ boards, loading, projectId }) {
  const showToast = useToast();
  const { updateBoards, isEditMode, setProjectData } = useProject();
  const [isCreating, setIsCreating] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [boardName, setBoardName] = useState("");

  const handleCreateBoardClick = () => {
    setIsCreating(true);
  };

  const handleCreateBoard = async (e) => {
    e.preventDefault();

    if (!boardName.trim()) {
      showToast("Название доски не может быть пустым.", "error");
      return;
    }

    setIsSubmitting(true);

    try {
      const newBoard = await postWithAuth(
        `/project/${projectId}/Board/CreateBoard`,
        { BoardName: boardName },
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      updateBoards(newBoard);

      setIsCreating(false);
      setBoardName("");
      showToast("Доска успешно создана!", "success");
    } catch (err) {
      console.error("Ошибка при создании доски:", err.response || err.message);
      showToast("Ошибка при создании доски. Попробуйте снова.", "error");
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteBoard = (boardId, boardName) => {
    if (
      window.confirm(`Вы уверены, что хотите удалить доску "${boardName}"?`)
    ) {
      // Логика удаления доски через API
      // try {
      //    await deleteWithAuth(`/project/${projectId}/Board/DeleteBoard/${boardId}`);
      setProjectData((prev) => ({
        ...prev,
        boards: prev.boards.filter((b) => b.boardId !== boardId),
      }));
      showToast(`Доска "${boardName}" удалена!`, "success");
      // } catch (error) {
      //    showToast("Ошибка при удалении доски.", "error");
      // }
      console.log("Удаление доски с ID:", boardId);
    }
  };

  if (loading) {
    return <div>Загрузка...</div>;
  }

  return (
    <div className="board-list-container">
      <div className="board-list-wrapper">
        {!isEditMode &&
          (isCreating ? (
            <form
              key="create-form"
              className="board-create-card-form"
              onSubmit={handleCreateBoard}
            >
              <div className="board-create-card-top">
                <div className="board-input-group board-floating-label-group">
                  <input
                    type="text"
                    className="board-name-input"
                    value={boardName}
                    onChange={(e) => setBoardName(e.target.value)}
                    required
                    disabled={isSubmitting}
                    placeholder=" "
                  />
                  <label className="board-floating-label">Название доски</label>
                </div>
              </div>
              <div className="board-create-card-bottom">
                <button
                  className="board-create-button"
                  type="submit"
                  disabled={isSubmitting}
                >
                  {isSubmitting ? "Создание..." : "Создать"}
                </button>
              </div>
            </form>
          ) : (
            <button
              key="create-button"
              className="board-card board-create-card"
              onClick={handleCreateBoardClick}
            >
              <img
                src={create_board_icon}
                alt="CREATE BOARD"
                className="board-create-logo"
              />
              <p className="board-create-text">Добавить доску</p>
            </button>
          ))}
        {boards.map((board, index) => (
          <BoardCard
            key={board.boardId}
            board={board}
            index={index}
            isEditMode={isEditMode}
            handleDeleteBoard={handleDeleteBoard}
            setProjectData={setProjectData}
            showToast={showToast}
          />
        ))}
      </div>
    </div>
  );
}
