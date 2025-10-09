using API.Constants.Roles;
using API.DTO.Domain;
using API.DTO.Responses.Pages.HallPage;
using API.DTO.Responses.Pages.HomePage;
using API.DTO.Responses.Pages.SettingsPage;
using API.DTO.Responses.Pages.WorkSpacePage;
using DataBaseInfo.Entities;
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
                    UserId = pu.UserId,
                    userUrl = pu.User.Avatar,
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
                UserId = projectUser.UserId,
                ProjectRole = projectUser.ProjectRole,
                userUrl = projectUser.User.Avatar,
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
        public static WorkSpace ToWorkSpacePage(int userId, Board board)
        {
            return new WorkSpace
            {
                BoardId = board.Id,
                BoardName = board.Name,
                ProjectName = board.MemberOfBoards
            .Where(m => m.ProjectUser.UserId == userId)
            .Select(m => m.ProjectUser.Project.ProjectName)
            .FirstOrDefault(),

                BoardRole = board.MemberOfBoards?
            .Where(m => m.ProjectUser?.UserId == userId)
            .Select(m => m.BoardRole)
            .FirstOrDefault(),

               Cards = board.Cards.Select(ToWorkSpaceCard).ToList(),
                Members = board.Project?.ProjectUsers?
    .Select(pu => ToWorkSpaceMember(pu, board.Id))  
    .ToList() ?? new List<WorkSpaceMember>()

            };
        }
        public static WorkSpaceCard ToWorkSpaceCard(Card card)
        {
            return new WorkSpaceCard
            {
                CardId = card.Id,
                CardName = card.Name,
                DateOfStartWork = card.DateStartOfWork,
                DateOfDeadline = card.DateOfDeadline,
                Tasks = card.Tasks?.Select(ToWorkSpaceTask).ToList() ?? new List<WorkSpaceTask>(),
            }; 
        }
        public static WorkSpaceTask ToWorkSpaceTask(_Task task)
        {
            return new WorkSpaceTask
            {
                TaskId = task.Id,
                DateOfStartWork = task.DateOfStartWork,
                DateOfDeadline = task.DateOfDeadline,
                Priority = task.Priority,
                ProgressBar = task.Progress,
                TaskDescription = task.Description,
                TaskName = task.Name,
                ResponsibleIds = task.ResponsibleIds,
                SubTasks = task.SubTasks?.Select(ToWorkSpaceSubTask).ToList() ?? new List<WorkSpaceSubTask>()
            };
        }
        public static WorkSpaceSubTask ToWorkSpaceSubTask(SubTask subTask)
        {
            return new WorkSpaceSubTask
            {
                SubTaskId = subTask.Id,
                SubTaskName = subTask.Name,
                IsCompleted = subTask.IsCompleted,
            };
        }
        public static WorkSpaceMember ToWorkSpaceMember(ProjectUser projectUser, int boardId)
        {
            var memberOfBoard = projectUser.MembersOfBoards
    .FirstOrDefault(mb => mb.BoardId == boardId);


            return new WorkSpaceMember
            {
                UserId = projectUser.UserId,
                ProjectUserId = projectUser.Id,
                MemberId = memberOfBoard?.Id ?? 0,
                BoardRole = memberOfBoard?.BoardRole ?? null,
                ProjectRole = projectUser.ProjectRole,
                FirstName = projectUser.User.FirstName,
                SecondName = projectUser.User.SecondName,
                UserAvatar = projectUser.User.Avatar,
            };
        }
    }
}
