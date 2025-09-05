using DataBaseInfo.models;

namespace API.Repositories.Queries.Interfaces
{
    public interface IProjectUserQueries
    {
        public Task<List<ProjectUser?>?> GetProjectUsersWithMembersByIds(List<int> projectUserIds);
    }
}
