import React from "react";
import styles from "./Logo.module.css";

// Используем SVG из public
const Logo = () => (
  <div className={styles.logoWrapper}>
    <img src="/logotip.svg" alt="Logo" className={styles.logo} />
  </div>
);

export default Logo;
