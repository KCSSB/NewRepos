// AuthContext.js

import { createContext, useState, useEffect, useContext } from "react";
import { decodeToken } from "../service/api";
import default_avatar from "../pages/Home/components/Navbar/avatar.png";

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [userAvatar, setUserAvatar] = useState(default_avatar);

  const updateAvatar = () => {
    // Получаем токен из локального хранилища
    const storedToken = localStorage.getItem("token");

    if (storedToken) {
      try {
        // Декодируем токен, чтобы извлечь URL аватара
        const payload = decodeToken(storedToken);

        if (payload && payload.Avatar) {
          // Добавляем параметр timestamp для обхода кэширования браузера
          const newAvatarUrl = `${payload.Avatar}?ts=${new Date().getTime()}`;
          setUserAvatar(newAvatarUrl);
        } else {
          // Если в токене нет URL аватара, устанавливаем аватар по умолчанию
          setUserAvatar(default_avatar);
        }
      } catch (error) {
        // В случае ошибки декодирования, сбрасываем аватар
        console.error("Ошибка декодирования токена:", error);
        setUserAvatar(default_avatar);
      }
    } else {
      // Если токена нет, устанавливаем аватар по умолчанию
      setUserAvatar(default_avatar);
    }
  };

  useEffect(() => {
    // Вызываем функцию обновления аватара при первой загрузке компонента
    updateAvatar();

    // Добавляем слушателя событий "storage", чтобы реагировать на изменения токена в других вкладках
    window.addEventListener("storage", updateAvatar);

    // Функция очистки: убираем слушатель, когда компонент размонтируется
    return () => {
      window.removeEventListener("storage", updateAvatar);
    };
  }, []); // Пустой массив зависимостей гарантирует, что эффект сработает только один раз

  return (
    <AuthContext.Provider value={{ userAvatar, updateAvatar }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
