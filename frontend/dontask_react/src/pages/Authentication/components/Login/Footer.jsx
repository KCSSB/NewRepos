import React from "react";
import { Link } from "react-router-dom";
import styles from "./Footer.module.css";

const Footer = () => (
  <div className={styles.footer}>
    <p>
      Нет аккаунта?{" "}
      <Link to="/auth/register" className={styles.createLink}>
        Создать
      </Link>
    </p>
  </div>
);

export default Footer;
