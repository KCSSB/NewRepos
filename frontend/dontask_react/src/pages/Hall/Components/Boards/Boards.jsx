// Boards.jsx
import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { fetchWithAuth } from "../../../../service/api";
import BoardHeader from "./BoardHeader.jsx";
import BoardList from "./Board_list.jsx";
import "./Boards.css";

export default function Boards() {
  const { projectId } = useParams();
  const [boards, setBoards] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchBoards = async () => {
      try {
        const response = await fetchWithAuth(
          `/GetPages/GetHallPage/${projectId}`
        );
        console.log("Данные списка досок получены: ", response);
        setBoards(response.boards || []);
      } catch (err) {
        console.error(
          "Ошибка при загрузке досок:",
          err.response || err.message
        );
      } finally {
        setLoading(false);
      }
    };
    fetchBoards();
  }, [projectId]);

  return (
    <div className="board-container">
      <BoardHeader boardsCount={boards.length} loading={loading} />
      {/* Pass projectId as a prop */}
      <BoardList
        boards={boards}
        setBoards={setBoards}
        loading={loading}
        projectId={projectId}
      />
    </div>
  );
}
