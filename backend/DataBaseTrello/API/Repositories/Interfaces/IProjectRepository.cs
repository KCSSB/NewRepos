using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        public Task AddDbProject(Project project);
        public Task<ProjectUser> ConnectWithProjectUser(ProjectUser projectUser);
        public Task<Project> GetProject(int projectId);
    }
}
