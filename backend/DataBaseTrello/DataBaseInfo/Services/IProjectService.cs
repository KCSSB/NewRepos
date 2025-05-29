using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;
namespace DataBaseInfo.Services
{
    internal interface IProjectService
    {
        Task<Project?> GetProjectByIdAsync(int id);
        Task<bool> CreateProjectAsync(Project project, int temporaryUserId, string ProjectName);
        Task<bool> UpdateProjectAsync(int id,Project project);
        Task<bool> DeleteProjectAsync(int id);

    }
}
