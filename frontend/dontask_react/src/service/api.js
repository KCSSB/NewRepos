import api from "./axiosInstance.js";

// Функция для обновления токена
const refreshAccessToken = async () => {
  try {
    const response = await api.post("/Auth/RefreshAccessToken");
    return response.data.accessToken;
  } catch (error) {
    throw new Error("Failed to refresh token. Please log in again.");
  }
};
// Функция для обновления и установки токена (загрузка аватарки)
export const refreshAndSetToken = async () => {
  try {
    const newAccessToken = await refreshAccessToken();
    localStorage.setItem("token", newAccessToken);
    console.log("RefreshAndSetToken success");
    return newAccessToken;
  } catch (error) {
    console.error("RefreshAndSetToken failed", error);
    throw error;
  }
};
// Функция для GET-запросов с токеном и обновления токена
export const fetchWithAuth = async (url) => {
  let accessToken = localStorage.getItem("token");

  if (!accessToken) {
    throw new Error("Access Token not found. Please log in.");
  }

  try {
    const response = await api.get(url, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    return response.data;
  } catch (error) {
    if (error.response?.status === 401) {
      console.log("Token expired. Attempting to refresh...");

      try {
        const newAccessToken = await refreshAccessToken();
        localStorage.setItem("token", newAccessToken);

        const newResponse = await api.get(url, {
          headers: {
            Authorization: `Bearer ${newAccessToken}`,
          },
        });
        return newResponse.data;
      } catch (refreshError) {
        throw new Error(
          "Unauthorized: Invalid or expired token. Please log in again."
        );
      }
    }
    throw error;
  }
};

// Функция для POST-запросов с токеном и обновления токена
export const postWithAuth = async (url, data, config = {}) => {
  let accessToken = localStorage.getItem("token");

  if (!accessToken) {
    throw new Error("Access Token not found. Please log in.");
  }

  try {
    const response = await api.post(url, data, {
      ...config,
      headers: {
        ...config.headers,
        Authorization: `Bearer ${accessToken}`,
      },
    });
    return response.data ? response.data : response;
  } catch (error) {
    if (error.response?.status === 401) {
      console.log("Token expired. Attempting to refresh...");
      try {
        const newAccessToken = await refreshAccessToken();
        localStorage.setItem("token", newAccessToken);

        const newResponse = await api.post(url, data, {
          ...config,
          headers: {
            ...config.headers,
            Authorization: `Bearer ${newAccessToken}`,
          },
        });
        return newResponse.data;
      } catch (refreshError) {
        throw new Error(
          "Unauthorized: Invalid or expired token. Please log in again."
        );
      }
    }
    throw error;
  }
};

// Функция для PATCH-запросов (обновление конкретных данных)
export const patchWithAuth = async (url, data, config = {}) => {
  let accessToken = localStorage.getItem("token");

  if (!accessToken) {
    throw new Error("Access Token not found. Please log in.");
  }

  try {
    const response = await api.patch(url, data, {
      ...config,
      headers: {
        ...config.headers,
        Authorization: `Bearer ${accessToken}`,
      },
    });
    return response.data ? response.data : response;
  } catch (error) {
    if (error.response?.status === 401) {
      console.log("Token expired. Attempting to refresh...");
      try {
        const newAccessToken = await refreshAccessToken();
        localStorage.setItem("token", newAccessToken);

        const newResponse = await api.patch(url, data, {
          ...config,
          headers: {
            ...config.headers,
            Authorization: `Bearer ${newAccessToken}`,
          },
        });
        return newResponse.data;
      } catch (refreshError) {
        throw new Error(
          "Unauthorized: Invalid or expired token. Please log in again."
        );
      }
    }
    throw error;
  }
};

// Функция для декодирования аватара из токена
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

// Функция для кропа изображения до квадрата
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
