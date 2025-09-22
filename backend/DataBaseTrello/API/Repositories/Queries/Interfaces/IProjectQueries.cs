using API.DTO.Responses.Pages.HallPage;
using DataBaseInfo.models;
using Org.BouncyCastle.Asn1.Mozilla;

namespace API.Repositories.Queries.Intefaces
{
    public interface IProjectQueries
    {
        public Task<List<Project?>?> GetAllProjectsWhereUserAsync(int userId);
        public Task<Project?> GetProjectWithUsersAsync(int projectId);
        public Task<Project?> GetProjectWithProjectUsersAsync(int projectId);
        public Task<Project?> GetProjectForHallAsync(int userId, int projectId);

    }
}
