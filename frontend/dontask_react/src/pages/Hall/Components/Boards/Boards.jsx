import React from "react";
import { useProject } from "../../HallContext.jsx";
import BoardHeader from "./BoardHeader.jsx";
import BoardList from "./Board_list.jsx";
import "./Boards.css";

export default function Boards() {
  const { projectData, loading } = useProject();
  const boards = projectData?.boards || [];

  return (
    <div className="board-container">
      <BoardHeader boardsCount={boards.length} />
      <BoardList
        boards={boards}
        loading={loading}
        projectId={projectData?.projectId}
      />
    </div>
  );
}
