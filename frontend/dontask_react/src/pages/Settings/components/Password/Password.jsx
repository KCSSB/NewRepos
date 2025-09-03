import React, { useState, useEffect } from "react";
import { fetchWithAuth, patchWithAuth } from "../../../../service/api";
import { useToast } from "../../../../components/Toast/ToastContext";
import "./Password.css";
import "../Profile/Profile.css";
import resetPassword_logo from "./resetPassword_logo.png";
import hide_logo from "./hide_logo.png";
import show_logo from "./show_logo.png";
import "../../../../service/errors.css";

export default function Password() {
  const showToast = useToast();
  const [userEmail, setUserEmail] = useState("");
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);

  const [oldPassword, setOldPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [repeatNewPassword, setRepeatNewPassword] = useState("");
  const [showOldPassword, setShowOldPassword] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showRepeatNewPassword, setShowRepeatNewPassword] = useState(false);

  const [error, setError] = useState("");

  const fetchUserEmail = async () => {
    try {
      setLoading(true);
      const data = await fetchWithAuth("/GetPages/GetSettingsPage");
      setUserEmail(data.userEmail || "");
    } catch (err) {
      console.error("Ошибка при получении данных:", err);
      showToast(
        "Не удалось загрузить данные. Пожалуйста, попробуйте снова.",
        "error"
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserEmail();
  }, [showToast]);

  const handleToggleForm = () => {
    setShowForm(!showForm);
    setOldPassword("");
    setNewPassword("");
    setRepeatNewPassword("");
    setError("");
  };

  const handleChangePassword = async (event) => {
    event.preventDefault();

    setError("");

    if (!oldPassword || !newPassword || !repeatNewPassword) {
      setError("Все поля обязательны для заполнения");
      return;
    }

    if (newPassword !== repeatNewPassword) {
      setError("Пароли не совпадают.");
      return;
    }

    if (newPassword === oldPassword) {
      setError("Новый пароль не может совпадать со старым");
      return;
    }

    setLoading(true);
    try {
      const payload = {
        oldPassword: oldPassword,
        newPassword: newPassword,
      };
      await patchWithAuth("/User/ChangePassword", payload);
      showToast("Пароль успешно изменен!", "success");
      handleToggleForm();
    } catch (err) {
      console.error("Ошибка при смене пароля:", err);
      setError(
        "Не удалось изменить пароль. Проверьте текущий пароль и попробуйте снова"
      );
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="password-container">
      <div className="input-group floating-label-group">
        <input
          type="text"
          className="profile-input read-only-input"
          value={userEmail}
          readOnly
        />
        <label className="floating-label">Почта</label>
      </div>
      <div className="input-group floating-label-group">
        <input
          type="password"
          className="profile-input read-only-input"
          value="********"
          readOnly
        />
        <label className="floating-label">Пароль</label>
      </div>
      <div className="action-buttons-group">
        <button className="profile-button" onClick={handleToggleForm}>
          Изменить <img src={resetPassword_logo} alt="RESET" />
        </button>
      </div>
      {showForm && (
        <form
          className="password-form-container"
          onSubmit={handleChangePassword}
        >
          <div className="input-group floating-label-group password-input-group">
            <input
              type={showOldPassword ? "text" : "password"}
              className="profile-input"
              value={oldPassword}
              onChange={(e) => setOldPassword(e.target.value)}
              required
            />
            <label className="floating-label">Текущий пароль</label>
            <button
              type="button"
              className="toggle-password-button"
              onClick={() => setShowOldPassword(!showOldPassword)}
            >
              <img
                src={showOldPassword ? hide_logo : show_logo}
                alt="Показать/Скрыть"
              />
            </button>
          </div>

          <div className="input-group floating-label-group password-input-group">
            <input
              type={showNewPassword ? "text" : "password"}
              className="profile-input"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              required
            />
            <label className="floating-label">Новый пароль</label>
            <button
              type="button"
              className="toggle-password-button"
              onClick={() => setShowNewPassword(!showNewPassword)}
            >
              <img
                src={showNewPassword ? hide_logo : show_logo}
                alt="Показать/Скрыть"
              />
            </button>
          </div>

          <div className="input-group floating-label-group password-input-group">
            <input
              type={showRepeatNewPassword ? "text" : "password"}
              className="profile-input"
              value={repeatNewPassword}
              onChange={(e) => setRepeatNewPassword(e.target.value)}
              required
            />
            <label className="floating-label">Повторите новый пароль</label>
            <button
              type="button"
              className="toggle-password-button"
              onClick={() => setShowRepeatNewPassword(!showRepeatNewPassword)}
            >
              <img
                src={showRepeatNewPassword ? hide_logo : show_logo}
                alt="Показать/Скрыть"
              />
            </button>
          </div>
          {error && <div className="error-message">{error}</div>}

          <div className="action-buttons-group">
            <button type="submit" className="profile-button" disabled={loading}>
              {loading ? "Загрузка..." : "Принять"}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
