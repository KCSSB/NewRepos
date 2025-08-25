using API.DTO.Responses;
using DataBaseInfo.models;

namespace API.DTO.Mappers
{
    public static class Mapper
    {
        public static SummaryProjectResponse ToSummaryProjectResponse(Project project)
        {
            return new SummaryProjectResponse
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                CountProjectUsers = project.ProjectUsers.Count(),
                ProjectImageUrl = project.Avatar,
                ProjectLeader = project.ProjectUsers
                    .Where(pu => pu.ProjectRole == "ProjectOwner")
                    .Select(pl => ToProjectLeaderResponse(pl))
                    .FirstOrDefault()

            };
        }
        public static ProjectLeaderResponse ToProjectLeaderResponse(ProjectUser projectLead)
        {
            return new ProjectLeaderResponse
            {
                ProjectLeaderId = projectLead.Id,
                ProjectLeaderName = projectLead.User.FirstName + " " + projectLead.User.SecondName,
                ProjectLeaderImageUrl = projectLead.User.Avatar
            };
        }
    }
}
