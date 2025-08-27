import React, { useState, useEffect } from "react";
import { fetchWithAuth } from "../../../../service/api";
import "./Password.css";
import "../Profile/Profile.css";
import resetPassword_logo from "./resetPassword_logo.png";

export default function Password() {
  const [userEmail, setUserEmail] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showForm, setShowForm] = useState(false);

  const fetchUserEmail = async () => {
    try {
      setLoading(true);
      const data = await fetchWithAuth("/GetPages/GetSettingsPage");
      setUserEmail(data.userEmail || "");
      setLoading(false);
    } catch (err) {
      console.error("Ошибка при получении данных:", err);
      setError("Не удалось загрузить данные. Пожалуйста, попробуйте снова.");
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserEmail();
  }, []);

  const handleToggleForm = () => {
    setShowForm(!showForm);
  };

  if (loading) {
    return <div className="password-container">Загрузка...</div>;
  }

  if (error) {
    return (
      <div className="password-container" style={{ color: "red" }}>
        Ошибка: {error}
      </div>
    );
  }

  return (
    <div className="password-container">
      <div className="input-group">
        <input
          type="text"
          className="profile-input"
          value={userEmail}
          readOnly
        />
        <span className="info-label">Почта</span>
      </div>
      <div className="input-group">
        <input type="text" className="profile-input" readOnly />
        <span className="info-label">Пароль</span>
      </div>
      <div className="action-buttons-group">
        <button className="profile-button" onClick={handleToggleForm}>
          Изменить <img src={resetPassword_logo} alt="RESET" />
        </button>
      </div>
      {showForm && (
        <form className="password-form-container">
          <div className="input-group">
            <input
              type="password"
              className="profile-input"
              placeholder="Текущий пароль"
            />
          </div>
          <div className="input-group">
            <input
              type="password"
              className="profile-input"
              placeholder="Новый пароль"
            />
          </div>
          <div className="input-group">
            <input
              type="password"
              className="profile-input"
              placeholder="Повторите новый пароль"
            />
          </div>
          <div className="action-buttons-group">
            <button type="submit" className="profile-button">
              Принять
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
