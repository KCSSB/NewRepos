import React from "react";
import { Link } from "react-router-dom";
import "./Demo.css";

export default function Demo() {
  return (
    <div className="demo-container">
      <h5>УСКОРЬТЕ РАБОТУ В ОДИН КЛИК</h5>
      <Link to="/auth/register">
        <button>Попробовать Демо</button>
      </Link>
    </div>
  );
}
