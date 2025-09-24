import React from "react";
import { useProject } from "../../HallContext.jsx";
import BoardHeader from "./BoardHeader.jsx";
import BoardList from "./Board_list.jsx";
import "./Boards.css";

export default function Boards() {
  const { projectData, loading, isFilteredByMember } = useProject();

  const allBoards = projectData?.boards || [];
  const filteredBoards = isFilteredByMember
    ? allBoards.filter((board) => board.isMember)
    : allBoards;
  const boardsCount = filteredBoards.length;

  return (
    <div className="board-container">
      <BoardHeader boardsCount={boardsCount} />{" "}
      <BoardList
        boards={filteredBoards}
        loading={loading}
        projectId={projectData?.projectId}
      />{" "}
    </div>
  );
}
