import React, { useState } from "react";
import styles from "./Form.module.css";
import api from "../../axiosInstance.js";

const Form = ({ isRegister }) => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [passwordConfirm, setPasswordConfirm] = useState("");
  const [error, setError] = useState(null); // исправлено usestate -> useState

  const handleSubmit = async (e) => {
    // добавлено async
    e.preventDefault();
    setError(null);
    if (isRegister) {
      if (password !== passwordConfirm) {
        setError("Пароли не совпадают");
        return;
      }
    }

    try {
      if (isRegister) {
        const response = await api.post("/auth/register", {
          UserEmail: login,
          UserPassword: password,
        });
        alert("Регистрация завершена!");
        // Можно сохранить токен, если он приходит в ответе
        // localStorage.setItem("token", response.data.token)
      } else {
        const response = await api.post("/auth/login", {
          UserEmail: login,
          UserPassword: password,
        });
        alert("Вход успешен");
        localStorage.setItem("token", response.data.token);
      }
    } catch (err) {
      setError(err.response?.data?.message || "Ошибка сервера");
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Логин"
        value={login}
        onChange={(e) => setLogin(e.target.value)}
        className={styles.input}
        required
      />
      <input
        type="password"
        placeholder="Пароль"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        className={styles.input}
        required
      />
      {isRegister && (
        <input
          type="password"
          placeholder="Повторите пароль"
          value={passwordConfirm}
          onChange={(e) => setPasswordConfirm(e.target.value)}
          className={styles.input}
          required
        />
      )}
      {error && (
        <div style={{ color: "red", marginBottom: "10px" }}>{error}</div>
      )}
      <button type="submit" className={styles.button}>
        {isRegister ? "Создать" : "Войти"}
      </button>
    </form>
  );
};

export default Form;
