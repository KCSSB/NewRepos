using DataBaseInfo.models;
using Org.BouncyCastle.Asn1.Mozilla;

namespace API.Repositories.Queries.Intefaces
{
    public interface IProjectQueries
    {
        public Task<List<Project?>?> GetAllProjectsWhereUser(int userId);
        public Task<Project?> GetProjectWithUsers(int projectId);
        public Task<Project?> GetProjectWithProjectUsers(int projectId);
    }
}
