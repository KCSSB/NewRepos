import React from "react";
import { Link } from "react-router-dom";
import styles from "./Footer.module.css";

const Footer = () => (
  <div className={styles.footer}>
    <p>
      Есть аккаунт?{" "}
      <Link to="/auth/login" className={styles.createLink}>
        Войти
      </Link>
    </p>
  </div>
);

export default Footer;
