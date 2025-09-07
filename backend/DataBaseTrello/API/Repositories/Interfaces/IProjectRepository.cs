using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        public Task AddAsync(Project project);
        public Task<Project> GetProjectAsync(int projectId);
    }
}
