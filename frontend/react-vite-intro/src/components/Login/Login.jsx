import React from "react";
import styles from "./Login.module.css";
import Logo from "./Logo";
import Title from "./Title";
import Form from "./Form";
import Footer from "./Footer";

const Login = () => (
  <div className={styles.wrapper}>
    <div className={styles.container}>
      <Logo />
      <Title text="ВХОД" />
      <Form />
      <Footer />
    </div>
  </div>
);

export default Login;
