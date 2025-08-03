import React from "react";
import { Routes, Route, Link } from "react-router-dom";
import LandingPage from "./pages/Landing/Landing";
import AuthPage from "./pages/Authentication/Auth";
import HomePage from "./pages/Home/Home";

function App() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />
      <Route path="/auth/*" element={<AuthPage />} />
      <Route path="/home" element={<HomePage />} />
    </Routes>
  );
}

export default App;
