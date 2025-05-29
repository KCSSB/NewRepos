import React from "react";
import { Link } from "react-router-dom";
import styles from "./Footer.module.css";

const Footer = () => (
  <div className={styles.footer}>
    Нет аккаунта?{" "}
    <Link to="/register" className={styles.createLink}>
      Создать
    </Link>
  </div>
);

export default Footer;
