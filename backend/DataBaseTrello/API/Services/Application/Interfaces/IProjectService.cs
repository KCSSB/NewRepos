
namespace API.Services.Application.Interfaces
{
    public interface IProjectService
    {
        public Task<int> CreateProjectAsync(string projectName);
        public Task<int> AddUserInProjectAsync(int userId, int projectId);
        public Task UpdateProjectImageAsync(int projectId, string imageUrl);
        public Task IsProjectOwner(int userId, int projectId);
    }
}
