import React from "react";
import { Link } from "react-router-dom";
import styles from "./Footer.module.css";

const Footer = () => (
  <div className={styles.footer}>
    Есть аккаунт?{" "}
    <Link to="/login" className={styles.createLink}>
      Войти
    </Link>
  </div>
);

export default Footer;
