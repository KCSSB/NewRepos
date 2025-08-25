import React, { useState, useRef, useEffect } from "react";
import {
  decodeToken,
  postWithAuth,
  refreshAndSetToken,
} from "../../../../service/api";
import "./Profile.css";
import default_avatar from "../../../Home/components/Navbar/avatar.png";
import load_image_logo from "./load_image_logo.png";

const autoCropImageToSquare = (imageFile, callback) => {
  const reader = new FileReader();
  reader.readAsDataURL(imageFile);
  reader.onload = (event) => {
    const img = new Image();
    img.src = event.target.result;

    img.onload = () => {
      const canvas = document.createElement("canvas");
      let side = Math.min(img.width, img.height);
      let dx = 0,
        dy = 0;

      if (img.width > img.height) {
        dx = (img.width - img.height) / 2;
      } else if (img.height > img.width) {
        dy = (img.height - img.width) / 2;
      }

      canvas.width = side;
      canvas.height = side;
      const ctx = canvas.getContext("2d");

      ctx.drawImage(img, dx, dy, side, side, 0, 0, side, side);

      canvas.toBlob(
        (blob) => {
          callback(blob);
        },
        "image/jpeg",
        0.9
      );
    };
  };
};

export default function Profile() {
  const [userAvatar, setUserAvatar] = useState(default_avatar);
  const [selectedFile, setSelectedFile] = useState(null);
  const [croppedFile, setCroppedFile] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [firstName, setFirstName] = useState("");
  const [secondName, setSecondName] = useState("");
  const [inviteId, setInviteId] = useState("");
  const fileInputRef = useRef(null);
  const [gender, setGender] = useState("male");

  const updateProfileFromToken = () => {
    const token = localStorage.getItem("token");
    if (token) {
      const payload = decodeToken(token);
      if (payload) {
        if (payload.Avatar) {
          setUserAvatar(payload.Avatar);
        } else {
          setUserAvatar(default_avatar);
        }
        setFirstName(payload.FirstName || "");
        setSecondName(payload.SecondName || "");
        setInviteId(payload.InviteId || "");
      } else {
        setUserAvatar(default_avatar);
        setFirstName("");
        setSecondName("");
        setInviteId("");
      }
    } else {
      setUserAvatar(default_avatar);
      setFirstName("");
      setSecondName("");
      setInviteId("");
    }
  };

  useEffect(() => {
    updateProfileFromToken();
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
      updateProfileFromToken();
      setSelectedFile(null);
      setCroppedFile(null);
    } catch (err) {
      console.error("Ошибка при загрузке аватара:", err);
      setError("Не удалось загрузить аватар. Попробуйте снова.");
      updateProfileFromToken();
    } finally {
      setLoading(false);
    }
  };

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
          <button className="profile-button delete">Удалить</button>
        </div>
      </div>
      <div className="profile-info-group">
        <div className="input-group">
          <input
            type="text"
            className="profile-input"
            value={secondName}
            onChange={(e) => setSecondName(e.target.value)}
            placeholder="Фамилия"
          />
          <span className="info-label">Фамилия</span>
        </div>
        <div className="input-group">
          <input
            type="text"
            className="profile-input"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            placeholder="Имя"
          />
          <span className="info-label">Имя</span>
        </div>
        <div className="profile-actions-row">
          <div className="action-buttons-group">
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
          <input
            type="text"
            className="profile-input"
            value={inviteId}
            readOnly
          />
          <span className="info-label">ID</span>
        </div>
        <div className="action-buttons-group">
          <button className="profile-button">Принять</button>
        </div>
      </div>
      {error && <p style={{ color: "red", marginTop: "10px" }}>{error}</p>}
    </div>
  );
}
// import React, { useState, useRef, useEffect } from "react";
// import {
//   decodeToken,
//   postWithAuth,
//   refreshAndSetToken,
// } from "../../../../service/api";
// import "./Profile.css";
// import default_avatar from "../../../Home/components/Navbar/avatar.png";
// import load_image_logo from "./load_image_logo.png";

// export default function Profile() {
//   const [userAvatar, setUserAvatar] = useState(default_avatar);
//   const [selectedFile, setSelectedFile] = useState(null);
//   const [loading, setLoading] = useState(false);
//   const [error, setError] = useState(null);
//   const fileInputRef = useRef(null);

//   const updateAvatarFromToken = () => {
//     const token = localStorage.getItem("token");
//     if (token) {
//       const payload = decodeToken(token);
//       if (payload && payload.Avatar) {
//         setUserAvatar(payload.Avatar);
//       } else {
//         setUserAvatar(default_avatar);
//       }
//     } else {
//       setUserAvatar(default_avatar);
//     }
//   };

//   useEffect(() => {
//     updateAvatarFromToken();
//   }, []);

//   const handleLoadClick = () => {
//     fileInputRef.current.click();
//   };

//   const handleFileChange = (e) => {
//     const file = e.target.files[0];
//     if (file) {
//       setSelectedFile(file);
//       setUserAvatar(URL.createObjectURL(file));
//       setError(null);
//     }
//   };

//   const handleUpload = async () => {
//     if (!selectedFile) {
//       setError("Пожалуйста, сначала выберите файл.");
//       return;
//     }
//     setLoading(true);
//     setError(null);

//     const formData = new FormData();
//     formData.append("File", selectedFile);

//     try {
//       await postWithAuth("/User/UploadUserAvatar", formData, {
//         headers: {
//           "Content-Type": "multipart/form-data",
//         },
//       });

//       await refreshAndSetToken();
//       window.dispatchEvent(new Event("tokenUpdated"));
//       updateAvatarFromToken();
//       setSelectedFile(null);
//     } catch (err) {
//       console.error("Ошибка при загрузке аватара:", err);
//       setError("Не удалось загрузить аватар. Попробуйте снова.");
//       updateAvatarFromToken();
//     } finally {
//       setLoading(false);
//     }
//   };

//   return (
//     <div className="profile-container">
//       <div className="profile-header-group">
//         <div className="profile-avatar-container">
//           <img src={userAvatar} alt="AVATAR" className="profile-avatar" />
//           <input
//             type="file"
//             accept="image/*"
//             style={{ display: "none" }}
//             ref={fileInputRef}
//             onChange={handleFileChange}
//           />
//           <button className="load-avatar-button" onClick={handleLoadClick}>
//             <img src={load_image_logo} alt="LOAD" />
//           </button>
//         </div>
//         <div className="profile-button-group">
//           <button
//             className="profile-button"
//             onClick={handleUpload}
//             disabled={!selectedFile || loading}
//           >
//             {loading ? "Загрузка..." : "Загрузить"}
//           </button>
//           <button className="profile-button delete">Удалить</button>
//         </div>
//       </div>
//       {error && <p style={{ color: "red", marginTop: "10px" }}>{error}</p>}
//     </div>
//   );
// }
