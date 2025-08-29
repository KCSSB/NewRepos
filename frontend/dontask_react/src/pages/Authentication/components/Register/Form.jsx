import React, { useState, useEffect } from "react";
import styles from "./Form.module.css";
import api from "../../../../service/axiosInstance.js";
import { useNavigate } from "react-router-dom";

const Form = ({ isRegister }) => {
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [passwordConfirm, setPasswordConfirm] = useState("");
  const [error, setError] = useState(null);

  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      navigate("/home");
    }
  }, [navigate]);

  const handleSuccessAuth = (token) => {
    localStorage.setItem("token", token);
    navigate("/home");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    if (isRegister) {
      if (password !== passwordConfirm) {
        setError("Пароли не совпадают");
        return;
      }
      try {
        const response = await api.post("/auth/register", {
          UserEmail: login,
          UserPassword: password,
        });

        const loginResponse = await api.post("/auth/login", {
          UserEmail: login,
          UserPassword: password,
        });

        handleSuccessAuth(loginResponse.data.accessToken);
      } catch (err) {
        setError(err.response?.data?.message || "Ошибка сервера");
      }
    } else {
      try {
        const response = await api.post("/auth/login", {
          UserEmail: login,
          UserPassword: password,
        });

        handleSuccessAuth(response.data.accessToken);
      } catch (err) {
        setError(err.response?.data?.message || "Ошибка сервера");
      }
    }
  };

  return (
    <form className={styles.form} onSubmit={handleSubmit}>
      <div className={styles["floating-label-group"]}>
        <input
          type="text"
          className={styles["profile-input"]}
          value={login}
          onChange={(e) => setLogin(e.target.value)}
          placeholder=" "
          required
        />
        <label className={styles["floating-label"]}>Логин</label>
      </div>
      <div className={styles["floating-label-group"]}>
        <input
          type="password"
          className={styles["profile-input"]}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder=" "
          required
        />
        <label className={styles["floating-label"]}>Пароль</label>
      </div>
      {isRegister && (
        <div className={styles["floating-label-group"]}>
          <input
            type="password"
            className={styles["profile-input"]}
            value={passwordConfirm}
            onChange={(e) => setPasswordConfirm(e.target.value)}
            placeholder=" "
            required
          />
          <label className={styles["floating-label"]}>Повторите пароль</label>
        </div>
      )}
      {error && (
        <div style={{ color: "red", marginBottom: "10px" }}>{error}</div>
      )}
      <button type="submit" className={styles["profile-button"]}>
        {isRegister ? "Создать" : "Войти"}
      </button>
    </form>
  );
};

export default Form;
