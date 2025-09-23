import React, { createContext, useContext, useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { fetchWithAuth } from "../../service/api";
import { useToast } from "../../components/Toast/ToastContext";

const ProjectContext = createContext(null);

export const ProjectProvider = ({ children }) => {
  const { projectId } = useParams();
  const showToast = useToast();
  const [projectData, setProjectData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchProjectData = async () => {
      if (!projectId) {
        setLoading(false);
        return;
      }

      try {
        const data = await fetchWithAuth(`/getpages/GetHallPage/${projectId}`);
        setProjectData(data);
      } catch (err) {
        console.error("Ошибка при получении данных проекта:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchProjectData();
  }, [projectId]);

  const value = { projectData, loading, showToast };

  return (
    <ProjectContext.Provider value={value}>{children}</ProjectContext.Provider>
  );
};

export const useProject = () => {
  const context = useContext(ProjectContext);
  if (context === undefined) {
    throw new Error("useProject должен использоваться внутри ProjectProvider");
  }
  return context;
};
