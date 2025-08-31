import api from "./axiosInstance.js";

export const refreshAccessToken = async () => {
  try {
    const response = await api.post("/Auth/RefreshAccessToken", {});
    const newAccessToken = response.data.accessToken;
    localStorage.setItem("token", newAccessToken);
    return true;
  } catch (error) {
    console.error("Ошибка при обновлении токена:", error);
    localStorage.removeItem("token");
    return false;
  }
};

// Функция для GET-запросов с токеном
export const fetchWithAuth = async (url) => {
  try {
    const response = await api.get(url);
    return response.data;
  } catch (error) {
    console.error("Ошибка в fetchWithAuth:", error);
    throw error;
  }
};

// Функция для POST-запросов с токеном и обновления токена
export const postWithAuth = async (url, data, config = {}) => {
  try {
    const response = await api.post(url, data, config);
    return response.data ? response.data : response;
  } catch (error) {
    console.error("Ошибка в postWithAuth:", error);
    throw error;
  }
};

// Функция для PATCH-запросов
export const patchWithAuth = async (url, data, config = {}) => {
  try {
    const response = await api.patch(url, data, config);
    return response.data ? response.data : response;
  } catch (error) {
    console.error("Ошибка в patchWithAuth:", error);
    throw error;
  }
};

// Функция выхода из аккаунта
export const logout = async () => {
  try {
    await api.delete("/Auth/Logout");
    localStorage.removeItem("token");
    console.log("Успешный выход из аккаунта на сервере.");
  } catch (error) {
    console.error("Ошибка при выходе из системы:", error);
  }
};

// Функция для декодирования токена
export const decodeToken = (token) => {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map(function (c) {
          return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join("")
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error("Failed to decode token", e);
    return null;
  }
};

// Функция для кропа изображения
export const autoCropImageToSquare = (imageFile, callback) => {
  const reader = new FileReader();
  reader.readAsDataURL(imageFile);
  reader.onload = (event) => {
    const img = new Image();
    img.src = event.target.result;

    img.onload = () => {
      const canvas = document.createElement("canvas");
      let side = Math.min(img.width, img.height);
      let dx = 0,
        dy = 0;

      if (img.width > img.height) {
        dx = (img.width - img.height) / 2;
      } else if (img.height > img.width) {
        dy = (img.height - img.width) / 2;
      }

      canvas.width = side;
      canvas.height = side;
      const ctx = canvas.getContext("2d");

      ctx.drawImage(img, dx, dy, side, side, 0, 0, side, side);

      canvas.toBlob(
        (blob) => {
          callback(blob);
        },
        "image/jpeg",
        0.9
      );
    };
  };
};
