import React, { useState } from "react";
import { useToast } from "../../../../components/Toast/ToastContext";
import { postWithAuth } from "../../../../service/api";
import { useProject } from "../../HallContext.jsx";
import create_board_icon from "./create_board_icon.png";
import sand_watches_icon from "./sand_watches_icon.png";
import people_icon from "./people_icon.png";
import "./Board_list.css";

const BOARD_COLORS = [
  "#8E8DFF",
  "#6868EE",
  "#EE6868",
  "#EEDB68",
  "#68EE68",
  "#EE68BB",
];

export default function Board_list({ boards, loading, projectId }) {
  const showToast = useToast();
  const { updateBoards } = useProject();
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

  const formatDate = (dateString) => {
    if (!dateString) return null;
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, "0");
    const month = String(date.getMonth() + 1).padStart(2, "0");
    return `${day}.${month}`;
  };

  if (loading) {
    return <div>Загрузка...</div>;
  }

  return (
    <div className="board-list-container">
      <div className="board-list-wrapper">
        {isCreating ? (
          <form className="board-create-card-form" onSubmit={handleCreateBoard}>
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
        )}
        {boards.map((board, index) => (
          <div
            key={board.boardId}
            className="board-card"
            style={{
              backgroundColor: BOARD_COLORS[index % BOARD_COLORS.length],
            }}
          >
            <div className="board-card-top">
              <h5 className="board-card-title">{board.boardName}</h5>
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
                        {board.dateOfStartwork && board.dateOfDeadline
                          ? " - "
                          : ""}
                        {board.dateOfDeadline
                          ? formatDate(board.dateOfDeadline)
                          : ""}
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
        ))}
      </div>
    </div>
  );
}
