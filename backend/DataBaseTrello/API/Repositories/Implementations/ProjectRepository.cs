using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;

namespace API.Repositories.Implementations
{
    public class ProjectRepository: IProjectRepository
    {
        private readonly AppDbContext _context;
        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task AddDbProject(Project project)
        {

        }
        public Task<ProjectUser> ConnectWithProjectUser(ProjectUser projectUser)
        {

        }
        public Task<Project> GetProject(int projectId)
        {

        }
    }
}
