import React, { useEffect } from "react";
import { Routes, Route, useNavigate } from "react-router-dom";
import LandingPage from "./pages/Landing/Landing";
import AuthPage from "./pages/Authentication/Auth";
import HomePage from "./pages/Home/Home";
import TaskPage from "./pages/Task/Task";
import SettingsPage from "./pages/Settings/Settings";
import { ToastProvider } from "./components/Toast/ToastContext";
import { AuthProvider } from "./context/AuthContext";

function App() {
  const navigate = useNavigate();

  useEffect(() => {
    localStorage.removeItem("token");
  }, []);

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
      <AuthProvider>
        {" "}
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route path="/auth/*" element={<AuthPage />} />
          <Route path="/home" element={<HomePage />} />
          <Route path="/task" element={<TaskPage />} />
          <Route path="/settings" element={<SettingsPage />} />
        </Routes>
      </AuthProvider>
    </ToastProvider>
  );
}

export default App;
