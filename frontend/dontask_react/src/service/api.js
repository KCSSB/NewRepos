import axios from "axios";

// Функция для выполнения GET-запросов с токеном
export const fetchWithAuth = async (url) => {
  const token = localStorage.getItem("token");

  if (!token) {
    throw new Error("Token not found. Please log in.");
  }

  try {
    const response = await axios.get(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data;
  } catch (error) {
    if (error.response?.status === 401) {
      throw new Error(
        "Unauthorized: Invalid or expired token. Please log in again."
      );
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
