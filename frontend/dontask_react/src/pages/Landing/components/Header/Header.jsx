import React from "react";
import { Link, useNavigate } from "react-router-dom";
import "./Header.css";
import dontask_logo from "./dontask_logo.png";

export default function Header() {
  const navigate = useNavigate();

  const handleDeleteTokenClick = () => {
    localStorage.removeItem("token");
    navigate("/auth/login");
  };
  return (
    <div className="header-container">
      <div className="header-left">
        <img src={dontask_logo} alt="DONTASK" />
        <h1>DONTASK</h1>
      </div>
      <div className="header-right">
        <button className="text-button help" onClick={handleDeleteTokenClick}>
          Удалить accessToken
        </button>
        <Link to="/auth/login">
          <button>Войти</button>
        </Link>
      </div>
    </div>
  );
}
