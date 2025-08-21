import api from "../pages/Authentication/components/axiosInstance.js";

// Функция для выполнения GET-запросов с токеном
// Функция для обновления токена (предполагаем, что Refresh Token в HttpOnly cookie)
const refreshAccessToken = async () => {
  try {
    const response = await api.post("/Auth/RefreshAccessToken");
    return response.data.accessToken;
  } catch (error) {
    // Если refresh токен недействителен, выбрасываем ошибку
    throw new Error("Failed to refresh token. Please log in again.");
  }
};

export const fetchWithAuth = async (url) => {
  let accessToken = localStorage.getItem("token");

  if (!accessToken) {
    throw new Error("Access Token not found. Please log in.");
  }

  try {
    // 1. Попытка выполнить запрос с текущим токеном
    const response = await api.get(url, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    return response.data;
  } catch (error) {
    // 2. Если получаем ошибку 401, пробуем обновить токен
    if (error.response?.status === 401) {
      console.log("Token expired. Attempting to refresh...");

      try {
        // 3. Обновляем токен
        const newAccessToken = await refreshAccessToken();
        localStorage.setItem("token", newAccessToken);

        // 4. Повторяем исходный запрос с новым токеном
        const newResponse = await api.get(url, {
          headers: {
            Authorization: `Bearer ${newAccessToken}`,
          },
        });
        return newResponse.data;
      } catch (refreshError) {
        // 5. Если обновление не удалось, выбрасываем ошибку и перенаправляем на логин
        throw new Error(
          "Unauthorized: Invalid or expired token. Please log in again."
        );
      }
    }
    // Если ошибка не 401, выбрасываем её как есть
    throw error;
  }
};
// export const fetchWithAuth = async (url) => {
//   const token = localStorage.getItem("token");

//   if (!token) {
//     throw new Error("Token not found. Please log in.");
//   }

//   try {
//     const response = await axios.get(url, {
//       headers: {
//         Authorization: `Bearer ${token}`,
//       },
//     });
//     return response.data;
//   } catch (error) {
//     if (error.response?.status === 401) {
//       throw new Error(
//         "Unauthorized: Invalid or expired token. Please log in again."
//       );
//     }
//     throw error;
//   }
// };

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
