// src/api.js
import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5155/api", 
  
});

// Добавим интерцептор для добавления токена, если он есть
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
