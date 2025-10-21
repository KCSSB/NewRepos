// src/pages/Workspace/Workspace.jsx

import React, { useState } from "react";
import { useParams } from "react-router-dom"; // Добавляем импорт useParams
import default_avatar from "./components/Header/avatar.png";
import Header from "./components/Header/Header.jsx";
import Navbar from "../Home/components/Navbar/Navbar.jsx";
import Card from "./components/Card/Card.jsx";
import { WorkspaceProvider, useWorkspace } from "./WorkspaceContext.jsx"; // Импортируем провайдер и хук
import "./Workspace.css";
import "../../fonts/fonts.css";

// --- Вспомогательный компонент для отображения контента ---
const WorkspaceContent = ({ defaultAvatarUrl }) => {
  // Получаем данные из контекста
  const { loading, projectName, boardName, members, projectId } =
    useWorkspace();

  // Для Header передадим только те аватарки, которые нам нужны.
  // Предполагаем, что Header ожидает массив аватарок
  const memberAvatars = members.map((member) => ({
    url: member.userImageUrl,
    alt: member.userName,
  }));

  if (loading) {
    return (
      <div className="workspace-main-content">
        <Header
          projectType="Загрузка..."
          projectName="Загрузка..."
          memberAvatars={[]}
          defaultAvatarUrl={defaultAvatarUrl}
          loading={true}
        />
        <div className="loading-state">Загрузка данных рабочей области...</div>
      </div>
    );
  }

  return (
    <div className="workspace-main-content">
      <Header
        projectName={projectName}
        boardName={boardName}
        memberAvatars={memberAvatars}
        defaultAvatarUrl={defaultAvatarUrl}
        loading={false}
      />
      <Card />
    </div>
  );
};

// --- Основной компонент Workspace ---
export default function Workspace() {
  return (
    <div className="workspace-container">
      <Navbar />
      {/* Оборачиваем контент в провайдер контекста */}
      <WorkspaceProvider>
        <WorkspaceContent defaultAvatarUrl={default_avatar} />
      </WorkspaceProvider>
    </div>
  );
}
