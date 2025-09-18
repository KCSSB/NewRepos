import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { getFirstNameFromToken } from "../../../../service/api";
import "./Welcome.css";
import welcome_photo from "./welcome_logo.png";
import arrow_right_logo from "./arrow_right_logo.png";

export default function Welcome() {
  const [firstName, setFirstName] = useState("User");

  useEffect(() => {
    const name = getFirstNameFromToken();
    if (name) {
      setFirstName(name);
    }
  }, []);

  return (
    <Link to="/project" className="welcome-link">
      <div className="welcome-container">
        <div className="title-container">
          <h5 className="welcome-title">С возвращением, {firstName}!</h5>{" "}
          <div className="row">
            <h5 className="welcome-paragraph">
              На чем Вы остановились в прошлый раз?
            </h5>
            <img src={arrow_right_logo} alt="ARROW_RIGHT" />
          </div>
        </div>
        <div className="logo-container">
          <img src={welcome_photo} alt="WELCOME" />
        </div>
      </div>
    </Link>
  );
}
