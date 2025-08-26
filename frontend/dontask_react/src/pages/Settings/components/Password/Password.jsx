import React from "react";

export default function Password() {
  return (
    <div className="password-container">
      <h2>Смена пароля</h2>
      <form>
        <div className="input-group">
          <input type="password" placeholder="Текущий пароль" />
        </div>
        <div className="input-group">
          <input type="password" placeholder="Новый пароль" />
        </div>
        <div className="input-group">
          <input type="password" placeholder="Повторите новый пароль" />
        </div>
        <button type="submit">Сменить пароль</button>
      </form>
    </div>
  );
}
