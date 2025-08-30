import React, { useState, useRef, useEffect } from "react";
import {
  fetchWithAuth,
  patchWithAuth,
  refreshAndSetToken,
  autoCropImageToSquare,
  postWithAuth,
  logout,
} from "../../../../service/api";
import { useNavigate } from "react-router-dom"; // 👈 Added import
import "./Profile.css";
import default_avatar from "../../../Home/components/Navbar/avatar.png";
import load_image_logo from "./load_image_logo.png";
import copy_inviteId_logo from "./copy_inviteId_logo.png";
import { useToast } from "../../../../components/Toast/ToastContext";

export default function Profile() {
  const showToast = useToast();
  const navigate = useNavigate(); // 👈 Initialized hook
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
      showToast(
        "Не удалось загрузить данные. Пожалуйста, попробуйте снова.",
        "error"
      );
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
    showToast("Выбор изображения отменен.", "info");
  };

  const handleUpload = async () => {
    if (!croppedFile) {
      showToast("Пожалуйста, сначала выберите файл.", "error");
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
      const newAvatarUrl = URL.createObjectURL(croppedFile);
      setUserAvatar(newAvatarUrl);
      setServerAvatar(newAvatarUrl);

      setSelectedFile(null);
      setCroppedFile(null);
      showToast("Аватар успешно загружен!", "success");
      setLoading(false);
    } catch (err) {
      console.error("Ошибка при загрузке аватара:", err);
      showToast("Не удалось загрузить аватар. Попробуйте снова.", "error");
      setError("Не удалось загрузить аватар. Попробуйте снова.");
      setLoading(false);
    }
  };

  const handleSaveChanges = async () => {
    if (initialData) {
      let currentSexValue;
      if (gender === "unknown") {
        currentSexValue = 0;
      } else if (gender === "male") {
        currentSexValue = 1;
      } else if (gender === "female") {
        currentSexValue = 2;
      }

      const isFirstNameChanged =
        firstName !== (initialData.firstUserName || "");
      const isSecondNameChanged =
        secondName !== (initialData.lastUserName || "");
      const isGenderChanged = currentSexValue !== initialData.sex;

      if (!isFirstNameChanged && !isSecondNameChanged && !isGenderChanged) {
        showToast("Вы не внесли никаких изменений.", "info");
        return;
      }
    }

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
      setInitialData((prev) => ({
        ...prev,
        firstUserName: firstName,
        lastUserName: secondName,
        sex: sexValue,
      }));
      showToast("Изменения успешно сохранены!", "success");
    } catch (err) {
      console.error("Ошибка при сохранении изменений:", err);
      showToast("Не удалось сохранить изменения. Попробуйте снова.", "error");
      setError("Не удалось сохранить изменения. Попробуйте снова.");
    } finally {
      setLoading(false);
    }
  };

  const handleCopyInviteId = () => {
    navigator.clipboard.writeText(inviteId);
    showToast("ID скопирован в буфер обмена!", "success");
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
      showToast("Изменения отменены.", "info");
    }
  };

  const handleLogout = async () => {
    try {
      await logout();
    } catch (err) {
      console.error("Ошибка при выходе из системы:", err);
      showToast(
        "Не удалось полностью выйти из системы. Попробуйте снова.",
        "error"
      );
    } finally {
      localStorage.removeItem("token");
      navigate("/auth/login");
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
            Отменить
          </button>
        </div>
      </div>
      <div className="profile-info-group">
        <div className="input-group floating-label-group">
          <input
            type="text"
            className="profile-input"
            value={secondName}
            onChange={(e) => setSecondName(e.target.value)}
          />
          <label className="floating-label">Фамилия</label>
        </div>
        <div className="input-group floating-label-group">
          <input
            type="text"
            className="profile-input"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
          />
          <label className="floating-label">Имя</label>
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
        <div className="input-group floating-label-group invite-input-group">
          <input
            type="text"
            className="profile-input read-only-input"
            value={inviteId}
            readOnly
          />
          <label className="floating-label">ID</label>
          <button className="button-light-style" onClick={handleCopyInviteId}>
            <img src={copy_inviteId_logo} alt="COPY" />
          </button>
        </div>
        <div className="action-buttons-group">
          <button className="profile-button" onClick={handleSaveChanges}>
            Принять
          </button>
          <button className="profile-button" onClick={handleResetChanges}>
            Сбросить
          </button>
          <button className="profile-button delete" onClick={handleLogout}>
            Выйти
          </button>
        </div>
      </div>
      {error && <p style={{ color: "red", marginTop: "10px" }}>{error}</p>}
    </div>
  );
}
