using API.DTO.Domain;
using API.DTO.Responses;
using API.DTO.Responses.Pages;
using DataBaseInfo.models;

namespace API.DTO.Mappers
{
    public static class ToResponseMapper
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
        public static SettingsPage ToSettingsPageResponse(User user)
        {
            return new SettingsPage
            {
                UserEmail = user.UserEmail,
                FirstUserName = user.FirstName,
                LastUserName = user.SecondName,
                Sex = user.Sex,
                InviteId = user.InviteId,
                UserAvatarUrl = user.Avatar,
            };
        }
        public static UpdateUserResponse ToUpdateUserResponse(UpdateUserModel updateUser)
        {
            return new UpdateUserResponse
            {
                FirstUserName = updateUser.FirstUserName,
                LastUserName = updateUser.LastUserName,
                Sex = updateUser.Sex
            };
        }
    }
}
