using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo.models;
using System;
using API.Exceptions.ErrorContext;
using System.Net;
using API.Extensions;
using API.Constants;

namespace API.Services
{
    public class GroupService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;


        public GroupService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        //Добавить транзацкции и уникальные индексы, Сделать кастомные ошибки
        public async Task<Guid> CreateGlobalGroupAsync(Guid projectUserId)
        {

            using var context = await _contextFactory.CreateDbContextAsync();

            var ProjectUser = await context.ProjectUsers.Include(g => g.Groups)
                .FirstOrDefaultAsync(pj => pj.Id == projectUserId);

            if (ProjectUser == null)
                throw new AppException(new ErrorContext(ServiceName.GroupService,
                    OperationName.CreateGlobalGroupAsync,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.CreateProjectExceptionMessage,
                    $"ProjectUser по указанному Id: {projectUserId}, не найден"));

            Group group = new Group
            {
                Name = "Global"
            };

            await context.Groups.AddAsync(group);

            await context.SaveChangesWithContextAsync(ServiceName.GroupService,
                OperationName.CreateGlobalGroupAsync,
                "Произошла ошибка во время сохранения группы Global",
                UserExceptionMessages.CreateProjectExceptionMessage,
                HttpStatusCode.InternalServerError);

            return group.Id;

        }
        public async Task<Guid> CreateGroupAsync(Guid projectUserId, string groupName)
        {
            
                using var context = await _contextFactory.CreateDbContextAsync();
                var ProjectUser = await context.ProjectUsers.Include(g => g.Groups)
                    .FirstOrDefaultAsync(pj => pj.Id == projectUserId);
                if (ProjectUser == null)
                throw new AppException(new ErrorContext(ServiceName.GroupService,
                     OperationName.CreateGroupAsync,
                     HttpStatusCode.InternalServerError,
                     UserExceptionMessages.CreateGroupExceptionMessage,
                     $"ProjectUser по указанному Id: {projectUserId}, не найден"));

            Group group = new Group
                {
                    Name = groupName
                };

                await context.Groups.AddAsync(group);
                await context.SaveChangesWithContextAsync(ServiceName.GroupService,
                    OperationName.CreateGroupAsync,
                    $"Произошла ошибка во время сохранения группы {groupName}",
                    UserExceptionMessages.CreateGroupExceptionMessage,
                    HttpStatusCode.InternalServerError);
                return group.Id;
           
        }
        public async Task<Guid> AddFirstUserInGroupAsync(Guid projectUserId, Guid groupId)
        {

            using var context = await _contextFactory.CreateDbContextAsync();
            ProjectUser? projectUser = await context.ProjectUsers.Include(m => m.Groups)
                .FirstOrDefaultAsync(pu => pu.Id == projectUserId);

            Group? group = await context.Groups.Include(m => m.Members)
                .FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                throw new AppException(new ErrorContext(ServiceName.GroupService,
                    OperationName.AddFirstUserInGroupAsync,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.CreateProjectExceptionMessage,
                    $"Group id: {groupId}, не найден"));

            if (projectUser == null)
                throw new AppException(new ErrorContext(ServiceName.GroupService,
                    OperationName.AddFirstUserInGroupAsync,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.CreateProjectExceptionMessage,
                    $"ProjectUser id: {projectUserId}, не найден"));
            MemberOfGroup member = new MemberOfGroup
            {
                ProjectUserId = projectUserId,
                GroupId = groupId,
                GroupRole = "Lead",

            };
            await context.MembersOfGroups.AddAsync(member);
            await context.SaveChangesWithContextAsync(ServiceName.GroupService,
                OperationName.AddFirstUserInGroupAsync,
                $"Произошла ошибка в момент добавления first member, в group id {groupId}",
                UserExceptionMessages.CreateProjectExceptionMessage,
                HttpStatusCode.InternalServerError);
            projectUser.Groups.Add(member);
            group.Members.Add(member);
            group.LeadId = member.Id;
            await context.SaveChangesWithContextAsync(ServiceName.GroupService,
                OperationName.AddFirstUserInGroupAsync,
                $"Произошла ошибка в момент настройки связей между group id: {groupId}, projectUser id: {projectUserId} и first member",
                UserExceptionMessages.CreateProjectExceptionMessage,
                HttpStatusCode.InternalServerError);
            return member.Id;
        }
        public async Task<Guid> AddUserInGroupAsync(Guid projectUserId, Guid groupId)
        {
            
            using var context = await _contextFactory.CreateDbContextAsync();
            ProjectUser? projectUser = await context.ProjectUsers.Include(m => m.Groups)
                .FirstOrDefaultAsync(pu => pu.Id == projectUserId);
                
            Group? group = await context.Groups.Include(m => m.Members)
                .FirstOrDefaultAsync(g =>g.Id == groupId);
            if (group == null)
                throw new AppException(new ErrorContext(ServiceName.GroupService,
                    OperationName.AddUserInGroupAsync,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.CreateGroupExceptionMessage,
                    $"Group id: {groupId}, не найден"));
            
            if (projectUser == null)
                throw new AppException(new ErrorContext(ServiceName.GroupService,
                    OperationName.AddUserInGroupAsync,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.CreateGroupExceptionMessage,
                    $"ProjectUser id: {projectUserId}, не найден"));
            MemberOfGroup member = new MemberOfGroup
                {
                    ProjectUserId = projectUserId,
                    GroupId = groupId,
                    GroupRole = "Member",
                    
                };
                await context.MembersOfGroups.AddAsync(member);
                await context.SaveChangesWithContextAsync(ServiceName.GroupService,
                    OperationName.AddUserInGroupAsync,
                    $"Произошла ошибка в момент добавления member, в group id {groupId}",
                    UserExceptionMessages.CreateGroupExceptionMessage,
                    HttpStatusCode.InternalServerError);
                projectUser.Groups.Add(member);
                group.Members.Add(member);
                group.LeadId = member.Id;
            await context.SaveChangesWithContextAsync(ServiceName.GroupService,
                OperationName.AddUserInGroupAsync,
                $"Произошла ошибка в момент настройки связей между group id: {groupId}, projectUser id: {projectUserId} и member",
                UserExceptionMessages.CreateGroupExceptionMessage,
                HttpStatusCode.InternalServerError);
            return member.Id;
            }
            
        }
    }

