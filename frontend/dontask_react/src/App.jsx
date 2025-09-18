import React, { useEffect } from "react";
import { Routes, Route, useNavigate } from "react-router-dom";
import LandingPage from "./pages/Landing/Landing";
import AuthPage from "./pages/Authentication/Auth";
import HomePage from "./pages/Home/Home";
import BoardPage from "./pages/Board/Board";
import SettingsPage from "./pages/Settings/Settings";
import ProjectPage from "./pages/Project/Project";
import { ToastProvider } from "./components/Toast/ToastContext";

function App() {
  const navigate = useNavigate();

  useEffect(() => {
    const handleStorageChange = (e) => {
      if (e.key === "token" && !e.newValue) {
        navigate("/auth/login");
      }
    };
    window.addEventListener("storage", handleStorageChange);

    return () => {
      window.removeEventListener("storage", handleStorageChange);
    };
  }, [navigate]);

  return (
    <ToastProvider>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/auth/*" element={<AuthPage />} />
        <Route path="/home" element={<HomePage />} />
        <Route path="/board" element={<BoardPage />} />
        <Route path="/settings" element={<SettingsPage />} />
        <Route path="/project" element={<ProjectPage />} />
      </Routes>
    </ToastProvider>
  );
}

export default App;
