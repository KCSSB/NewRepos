import React, { useState, useRef, useEffect } from "react";
import {
  fetchWithAuth,
  patchWithAuth,
  refreshAndSetToken,
  autoCropImageToSquare,
  postWithAuth,
} from "../../../../service/api";
import "./Profile.css";
import default_avatar from "../../../Home/components/Navbar/avatar.png";
import load_image_logo from "./load_image_logo.png";
import copy_inviteId_logo from "./copy_inviteId_logo.png";

export default function Profile() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [userAvatar, setUserAvatar] = useState(default_avatar);
  const [serverAvatar, setServerAvatar] = useState(default_avatar);
  const [selectedFile, setSelectedFile] = useState(null);
  const [croppedFile, setCroppedFile] = useState(null);
  const [firstName, setFirstName] = useState("");
  const [secondName, setSecondName] = useState("");
  const [inviteId, setInviteId] = useState("");
  const fileInputRef = useRef(null);
  const [gender, setGender] = useState("unknown");
  const [userEmail, setUserEmail] = useState("");
  const [initialData, setInitialData] = useState(null);

  const fetchSettingsPageData = async () => {
    try {
      setLoading(true);
      const data = await fetchWithAuth("/GetPages/GetSettingsPage");
      console.log("Данные настроек получены:", data);

      const avatarUrl = data.userAvatarUrl || default_avatar;
      setUserAvatar(avatarUrl);
      setServerAvatar(avatarUrl);
      setFirstName(data.firstUserName || "");
      setSecondName(data.lastUserName || "");
      setInviteId(data.inviteId || "");
      setInitialData(data);

      let initialGender;
      if (data.sex === 0) {
        initialGender = "unknown";
      } else if (data.sex === 1) {
        initialGender = "male";
      } else if (data.sex === 2) {
        initialGender = "female";
      }
      setGender(initialGender);

      setUserEmail(data.userEmail || "");

      setLoading(false);
    } catch (err) {
      console.error("Ошибка при получении данных настроек:", err);
      setError("Не удалось загрузить данные. Пожалуйста, попробуйте снова.");
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSettingsPageData();
  }, []);

  const handleLoadClick = () => {
    fileInputRef.current.click();
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setUserAvatar(URL.createObjectURL(file));
      setSelectedFile(file);
      setError(null);

      autoCropImageToSquare(file, (blob) => {
        setCroppedFile(blob);
      });
    }
  };

  const handleDeleteClick = () => {
    setUserAvatar(serverAvatar);
    setSelectedFile(null);
    setCroppedFile(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleUpload = async () => {
    if (!croppedFile) {
      setError("Пожалуйста, сначала выберите файл.");
      return;
    }
    setLoading(true);
    setError(null);

    const formData = new FormData();
    formData.append("File", croppedFile, "avatar.jpg");

    try {
      await postWithAuth("/User/UploadUserAvatar", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      await refreshAndSetToken();
      window.dispatchEvent(new Event("tokenUpdated"));
      await fetchSettingsPageData();
      setSelectedFile(null);
      setCroppedFile(null);
    } catch (err) {
      console.error("Ошибка при загрузке аватара:", err);
      setError("Не удалось загрузить аватар. Попробуйте снова.");
      setLoading(false);
    }
  };

  const handleSaveChanges = async () => {
    setLoading(true);
    setError(null);
    try {
      let sexValue;
      if (gender === "unknown") {
        sexValue = 0;
      } else if (gender === "male") {
        sexValue = 1;
      } else if (gender === "female") {
        sexValue = 2;
      }

      const payload = {
        firstUserName: firstName,
        lastUserName: secondName,
        sex: sexValue,
      };

      await patchWithAuth("/User/UpdateGeneralUserInfo", payload);
      setInitialData(payload);
      await fetchSettingsPageData();
    } catch (err) {
      console.error("Ошибка при сохранении изменений:", err);
      setError("Не удалось сохранить изменения. Попробуйте снова.");
    } finally {
      setLoading(false);
    }
  };

  const handleCopyInviteId = () => {
    navigator.clipboard.writeText(inviteId);
  };

  const handleResetChanges = () => {
    if (initialData) {
      setFirstName(initialData.firstUserName || "");
      setSecondName(initialData.lastUserName || "");

      let initialGender;
      if (initialData.sex === 0) {
        initialGender = "unknown";
      } else if (initialData.sex === 1) {
        initialGender = "male";
      } else if (initialData.sex === 2) {
        initialGender = "female";
      }
      setGender(initialGender);

      setError(null);
    }
  };

  if (error) {
    return (
      <div className="profile-container">
        <p style={{ color: "red" }}>Ошибка: {error}</p>
      </div>
    );
  }

  return (
    <div className="profile-container">
      <div className="profile-header-group">
        <div className="profile-avatar-container">
          <img src={userAvatar} alt="AVATAR" className="profile-avatar" />
          <input
            type="file"
            accept="image/*"
            style={{ display: "none" }}
            ref={fileInputRef}
            onChange={handleFileChange}
          />
          <button className="load-avatar-button" onClick={handleLoadClick}>
            <img src={load_image_logo} alt="LOAD" />
          </button>
        </div>
        <div className="profile-button-group">
          <button
            className="profile-button"
            onClick={handleUpload}
            disabled={!croppedFile || loading}
          >
            {loading ? "Загрузка..." : "Загрузить"}
          </button>
          <button className="profile-button delete" onClick={handleDeleteClick}>
            Удалить
          </button>
        </div>
      </div>
      <div className="profile-info-group">
        <div className="input-group">
          <input
            type="text"
            className="profile-input"
            value={secondName}
            onChange={(e) => setSecondName(e.target.value)}
          />
          <span className="info-label">Фамилия</span>
        </div>
        <div className="input-group">
          <input
            type="text"
            className="profile-input"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
          />
          <span className="info-label">Имя</span>
        </div>
        <div className="profile-actions-row">
          <div className="action-buttons-group">
            <button
              className={
                gender === "unknown"
                  ? "profile-button active"
                  : "profile-button inactive"
              }
              onClick={() => setGender("unknown")}
            >
              Не указан
            </button>
            <button
              className={
                gender === "male"
                  ? "profile-button active"
                  : "profile-button inactive"
              }
              onClick={() => setGender("male")}
            >
              Мужской
            </button>
            <button
              className={
                gender === "female"
                  ? "profile-button active"
                  : "profile-button inactive"
              }
              onClick={() => setGender("female")}
            >
              Женский
            </button>
          </div>
          <span className="info-label">Пол</span>
        </div>
        <div className="input-group">
          <div className="invite-group">
            <input
              type="text"
              className="profile-input"
              value={inviteId}
              readOnly
            />
            <button className="button-light-style" onClick={handleCopyInviteId}>
              <img src={copy_inviteId_logo} alt="COPY" />
            </button>
          </div>
          <span className="info-label">ID</span>
        </div>
        <div className="action-buttons-group">
          <button className="profile-button" onClick={handleSaveChanges}>
            Принять
          </button>
          <button className="profile-button" onClick={handleResetChanges}>
            Сбросить
          </button>
        </div>
      </div>
      {error && <p style={{ color: "red", marginTop: "10px" }}>{error}</p>}
    </div>
  );
}
