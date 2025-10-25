using DataBaseInfo.models;

namespace API.Repositories.Queries.Interfaces
{
    public interface IProjectUserQueries
    {
        public Task<List<ProjectUser?>?> GetProjectUsersWithMembersByIdsAsync(List<int> projectUserIds);
    }
}
