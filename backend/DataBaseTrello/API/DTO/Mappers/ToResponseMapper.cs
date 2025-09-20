using API.DTO.Domain;
using API.DTO.Responses.Pages.HallPage;
using API.DTO.Responses.Pages.HomePage;
using API.DTO.Responses.Pages.SettingsPage;
using DataBaseInfo.models;

namespace API.DTO.Mappers
{
    public static class ToResponseMapper
    {
        public static HomeProject ToHomeProject(Project project)
        {
            return new HomeProject
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                CountProjectUsers = project.ProjectUsers.Count(),
                ProjectImageUrl = project.Avatar,
                ProjectLeader = project.ProjectUsers
                    .Where(pu => pu.ProjectRole == "ProjectOwner")
                    .Select(pl => ToHomeProjectLeader(pl))
                    .FirstOrDefault()

            };
        }
        public static HomeProjectLeader ToHomeProjectLeader(ProjectUser projectLead)
        {
            return new HomeProjectLeader
            {
                ProjectLeaderId = projectLead.Id,
                ProjectLeaderName = projectLead.User.FirstName + " " + projectLead.User.SecondName,
                ProjectLeaderImageUrl = projectLead.User.Avatar
            };
        }
        public static SettingsPage ToSettingsPage(User user)
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
        public static SettingsUserInfo ToUpdateUserModel(UpdateUserModel updateUser)
        {
            return new SettingsUserInfo
            {
                FirstUserName = updateUser.FirstUserName,
                LastUserName = updateUser.LastUserName,
                Sex = updateUser.Sex
            };
        }
        public static HallProjectUser ToHallProjectUser(ProjectUser projectUser)
        {
            return new HallProjectUser
            {
                ProjectUserId = projectUser.Id,
                FirstName = projectUser.User.FirstName,
                LastName = projectUser.User.SecondName
            };
        }
    }
}
