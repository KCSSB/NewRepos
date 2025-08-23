import React, { useEffect, useState, useRef } from "react";
import "./Project_list.css";
import people_logo from "./people_logo.png";
import create_logo from "./create_temporary.png";
import ProjectSkeleton from "./Project_list_skeleton";
import { fetchWithAuth, postWithAuth } from "../../../../service/api";
import { useNavigate } from "react-router-dom";

export default function Project_list() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [isCreating, setIsCreating] = useState(false);
  const [projectName, setProjectName] = useState("");
  const [projectImage, setProjectImage] = useState(null);
  const fileInputRef = useRef(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchProjects = async () => {
      try {
        console.log("Начинаем отправку запроса к API...");
        const data = await fetchWithAuth("/getpages/GetHomePage");
        console.log("Данные получены:", data);
        setProjects(data.summaryProject || []);
      } catch (err) {
        console.error("Ошибка при получении проектов:", err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchProjects();
  }, []);

  const handleCreateProjectClick = () => {
    setIsCreating(true);
  };

  const handleUploadClick = () => {
    fileInputRef.current.click();
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setProjectImage(file);
    }
  };

  const handleCreateProject = async () => {
    if (!projectName || !projectImage) {
      alert("Пожалуйста, введите название проекта и загрузите обложку.");
      return;
    }

    try {
      const formData = new FormData();
      formData.append("ProjectName", projectName);
      formData.append("Image", projectImage);

      const newProject = await postWithAuth(
        "/Projects/CreateProject",
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      console.log("Проект успешно создан:", newProject);

      setProjects((prevProjects) => [...prevProjects, newProject]);
      setIsCreating(false);
      setProjectName("");
      setProjectImage(null);
    } catch (err) {
      console.error(
        "Ошибка при создании проекта:",
        err.response || err.message
      );
      alert("Ошибка при создании проекта. Попробуйте снова.");
    }
  };

  if (loading) {
    return (
      <div className="project-list-container">
        <div className="project-list-wrapper">
          {[...Array(6)].map((_, index) => (
            <ProjectSkeleton key={index} />
          ))}
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="project-list-container">
        <div style={{ color: "red" }}>Ошибка: {error}</div>
      </div>
    );
  }

  return (
    <div className="project-list-container">
      <div className="project-list-wrapper">
        {projects.map((project) => (
          <button key={project.projectId} className="project-card">
            <div className="card-top">
              <img
                src={project.projectImageUrl}
                alt="Project Logo"
                className="project-logo"
              />
            </div>
            <div className="card-bottom">
              <div className="creator-info">
                <img
                  src={project.projectLeader.projectLeaderImageUrl}
                  alt="Leader Avatar"
                  className="creator-avatar"
                />
                <div className="text-info">
                  <h5>{project.projectName}</h5>
                  <p>{project.projectLeader.projectLeaderName}</p>
                </div>
              </div>
              <div className="people-count">
                <img src={people_logo} alt="People" className="people-logo" />
                <h6>{project.countProjectUsers}</h6>
              </div>
            </div>
          </button>
        ))}
        {isCreating ? (
          <div className="project-card create-project-card">
            <div className="card-top">
              <button className="upload-button" onClick={handleUploadClick}>
                Загрузить обложку проекта
              </button>
              <button className="create-button" onClick={handleCreateProject}>
                Создать
              </button>
              <input
                type="file"
                ref={fileInputRef}
                style={{ display: "none" }}
                onChange={handleImageChange}
                required
              />
            </div>
            <div className="card-bottom">
              <input
                type="text"
                placeholder="Название проекта"
                className="project-name-input"
                value={projectName}
                onChange={(e) => setProjectName(e.target.value)}
                required
              />
            </div>
          </div>
        ) : (
          <button
            className="project-card create-card"
            onClick={handleCreateProjectClick}
          >
            <img
              src={create_logo}
              alt="Create Project"
              className="create-project-logo"
            />
          </button>
        )}
      </div>
    </div>
  );
}
