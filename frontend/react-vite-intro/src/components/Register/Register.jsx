import React from "react";
import styles from "./Login.module.css";
import Logo from "./Logo";
import Title from "./Title";
import Form from "./Form";
import Footer from "./Footer";

const Register = () => (
  <div className={styles.wrapper}>
    <div className={styles.container}>
      <Logo />
      <Title text="РЕГИСТРАЦИЯ" />
      <Form isRegister={true} />
      <Footer />
    </div>
  </div>
);

export default Register;
