using API.Constants.Roles;
using API.DTO.Domain;
using API.DTO.Responses.Pages.HallPage;
using API.DTO.Responses.Pages.HomePage;
using API.DTO.Responses.Pages.SettingsPage;
using DataBaseInfo.models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

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
                    .Where(pu => pu.ProjectRole == ProjectRoles.Owner)
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
        public static HallPage ToHallPage(Project project, int userId)
        {
            return new HallPage
            {
                ProjectId = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,

                ProjectRole = project.ProjectUsers
                   .FirstOrDefault(pu => pu.UserId == userId)?.ProjectRole,

                ProjectUsers = project.ProjectUsers.Select(pu => new HallProjectUser
                {
                    ProjectUserId = pu.Id,
                    FirstName = pu.User?.FirstName ?? string.Empty,
                    LastName = pu.User?.SecondName ?? string.Empty,
                    ProjectRole = pu.ProjectRole
                }).ToList(),

                Boards = project.ProjectUsers
                   .SelectMany(pu => pu.MembersOfBoards.Select(mob => mob.Board))
                   .Distinct()
                   .Select(b => new HallBoard
                   {
                       BoardId = b.Id,
                       BoardName = b.Name,
                       MembersCount = b.MemberOfBoards.Count,
                       ProgressBar = b.ProgressBar,

                       BoardLeadId = b.LeadOfBoardId,

                       DateOfStartWork = b.DateStartOfWork,

                       DateOfDeadline = b.DateOfDeadline,
                       IsMember = b.MemberOfBoards.Any(mb => mb.ProjectUser.UserId == userId)

                   }).ToList()
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
        public static HallBoard ToHallBoard(Board board)
        {
            return new HallBoard
            {
                BoardId = board.Id,
                BoardName = board.Name,
                BoardLeadId= board.LeadOfBoardId,
                DateOfStartWork = board.DateStartOfWork,
                DateOfDeadline= board.DateOfDeadline,
                MembersCount = board.MemberOfBoards.Count,
                ProgressBar= board.ProgressBar,
            };
        }
    }
}
