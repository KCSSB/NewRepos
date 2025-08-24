import React, { useState, useEffect } from "react";
import styles from "./Form.module.css";
import api from "../../../../service/axiosInstance.js";
import { useNavigate } from "react-router-dom";

const handleLogin = async (login, password, navigate, setError) => {
  try {
    const response = await api.post("/auth/login", {
      UserEmail: login,
      UserPassword: password,
    });

    localStorage.setItem("token", response.data.accessToken);
    navigate("/home");
  } catch (err) {
    setError(err.response?.data?.message || "Ошибка сервера");
  }
};

const Form = ({ isRegister }) => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [passwordConfirm, setPasswordConfirm] = useState("");
  const [error, setError] = useState(null);

  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    console.log("Значение токена из localStorage:", token);
    if (token) {
      navigate("/home");
    }
  }, [navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    if (isRegister) {
      if (password !== passwordConfirm) {
        setError("Пароли не совпадают");
        return;
      }
      try {
        await api.post("/auth/register", {
          UserEmail: login,
          UserPassword: password,
        });
        alert("Регистрация завершена!");
      } catch (err) {
        setError(err.response?.data?.message || "Ошибка сервера");
      }
    } else {
      handleLogin(login, password, navigate, setError);
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
