import React from "react";
import "./Background.css";
import plus_logo from "./plus_logo.png";
import dots_logo from "./dots_logo.png";
import avatar from "./avatar.png";
import bell from "./bell.png";

export default function Background({ children }) {
  return (
    <div className="landing-page">
      <div className="content">{children}</div>
      <div className="frame-tasks">
        <h1>Задачи</h1>
        <div className="icons">
          <img src={plus_logo} alt="PLUS" />
          <img src={dots_logo} alt="DOTS" />
        </div>
        <div className="frame-tasks-inside">
          <h2>17 мая 2024</h2>
          <h2>Квартальный отчет</h2>
          <div className="priority-row">
            <p className="frame-tasks-inside-bottom">Высокий приоритет</p>
            <img src={avatar} alt="avatar" />
          </div>
        </div>
      </div>
      <div className="frame-one-place">
        <p>Все задачи в одном месте, без лишних бумаг</p>
      </div>
      <div className="frame-notice">
        <div className="notice-header">
          <img src={bell} alt="BELL" />
          <h2>Уведомление</h2>
        </div>
        <p>Вас хотят добавить в новый проект</p>
      </div>
    </div>
  );
}
