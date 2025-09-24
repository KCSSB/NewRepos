import React, { createContext, useContext, useState, useCallback } from "react";
import { useToast } from "../../components/Toast/ToastContext";

const ProjectContext = createContext(null);

export const ProjectProvider = ({ children }) => {
  const [projectData, setProjectData] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isFilteredByMember, setIsFilteredByMember] = useState(false);
  const showToast = useToast();

  const updateBoards = useCallback((newBoard) => {
    setProjectData((prevData) => {
      if (!prevData) return null;
      return {
        ...prevData,
        boards: [...(prevData.boards || []), newBoard],
      };
    });
  }, []);

  const toggleFilter = useCallback(() => {
    setIsFilteredByMember((prev) => !prev);
  }, []);

  const value = {
    projectData,
    setProjectData,
    loading,
    setLoading,
    showToast,
    updateBoards,
    isFilteredByMember,
    toggleFilter,
  };

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
