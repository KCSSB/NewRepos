using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IProjectUserRepository
    {
        public Task<ProjectUser?> GetProjectUser(int userId, int projectId);
        public Task<ProjectUser?> GetProjectUser(int projectUserId);
        public void RemoveProjectUser(ProjectUser projectUser);
    }
}
