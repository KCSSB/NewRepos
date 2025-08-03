import React from "react";
import "./Background.css";
import background_photo from "./background_photo.jpg";

export default function Background() {
  return (
    <div className="content-container">
      <img src={background_photo} alt="BACKGROUND" />
    </div>
  );
}
