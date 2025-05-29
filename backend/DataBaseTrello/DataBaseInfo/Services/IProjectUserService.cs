using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseInfo.models;

namespace DataBaseInfo.Services
{
    internal interface IProjectUserService
    {
        Task<ProjectUser?> GetProjectUserByIdAsync(int id);
        Task<bool> CreateProjectUserAsync(ProjectUser ProjectUser, User user, Project project);
        Task<bool> UpdateProjectUserAsync(int id, ProjectUser updatedUser);
        Task<bool> DeleteProjectUserAsync(int id);
    }
}
