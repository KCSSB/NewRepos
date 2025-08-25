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
// Для страницы настроек
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
// Функция для выполнения GET-запросов с токеном и обновления токена (RefreshToken в HttpOnly cookie)
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

// Функция для выполнения POST-запросов с токеном и обновления токена
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
