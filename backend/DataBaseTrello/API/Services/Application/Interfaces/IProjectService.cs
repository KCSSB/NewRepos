
using DataBaseInfo.models;

namespace API.Services.Application.Interfaces
{
    public interface IProjectService
    {
        public Task<int> CreateProjectAsync(string projectName);
        public Task<int> AddUserInProjectAsync(int userId, int projectId);
        public Task UpdateProjectImageAsync(int projectId, string imageUrl);
        public Task DeleteProjectUsersAsync(List<int> projectUsers);
        public Task<string> UpdateProjectNameAsync(int projectId, string projectName);
        public Task<string> UpdateProjectDescriptionAsync(int projectId, string projectDescription);
        //public Task IsProjectOwner(int userId, int projectId);
        //public Task UpdateProjectDescription(int projectId, string desctiption);
        //public Task UpdateProjectName(int projectId, string projectName);
        //public Task DeleteProjectUserAsync(int projectId, List<string> projectUsers);

    }
}
