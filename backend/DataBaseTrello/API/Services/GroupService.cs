using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using DataBaseInfo.models;
using System;

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
        public async Task<int> CreateGroupAsync(int? projectUserId, string groupName = "Global")
        {
            try
            {
                if (projectUserId == null)
                    throw new ArgumentNullException("Ошибка при получении ProjectUserId");
                using var context = await _contextFactory.CreateDbContextAsync();
                var ProjectUser = await context.ProjectUsers.Include(g => g.Groups)
                    .FirstOrDefaultAsync(pj => pj.Id == projectUserId);
                if (ProjectUser == null || ProjectUser.Groups == null)
                    throw new ArgumentNullException("Ошибка при получении списка групп");
                int GroupsCount = ProjectUser.Groups.Count();
                if (GroupsCount > 0 && groupName == "Global")
                    throw new InvalidOperationException("Ошибка в именовании группы, вы указали имя группы Global");

                Group group = new Group
                {
                    Name = groupName
                };

                await context.Groups.AddAsync(group);
                await context.SaveChangesAsync();
                return group.Id;
            }
            catch (DbUpdateException ex)
            {
                //Логирование ошибки DbUpdateException
                throw;
            }
            catch (InvalidOperationException ex)
            {
                //Логирование ошибки отсутствии данных
                throw;
            }
            catch (InvalidOperationException ex)
            {
                //Логирование ошибки Об именовании группы Global
                throw;
            }
            catch(Exception)
            {
                //Логирование непредвиденной ошибки
                throw;
            }
        }
        public async Task<int?> AddUserInGroupAsync(int projectUserId, int groupId)
        {
            try
            {

            using var context = await _contextFactory.CreateDbContextAsync();
            ProjectUser? projectUser = await context.ProjectUsers.Include(m => m.Groups)
                .FirstOrDefaultAsync(pu => pu.Id == projectUserId);
            Group? group = await context.Groups.Include(m => m.Members)
                .FirstOrDefaultAsync(g =>g.Id == groupId);
                if (projectUser == null || group == null)
                    throw new InvalidOperationException("Указанная Group или User не были найдены");
                MemberOfGroup member = new MemberOfGroup
                {
                    ProjectUserId = projectUserId,
                    GroupId = groupId,
                    GroupRole = "Lead", //Временно на первое время
                    
                };
                await context.MembersOfGroups.AddAsync(member);
                await context.SaveChangesAsync();
                projectUser.Groups.Add(member);
                group.Members.Add(member);
                group.LeadId = member.Id;
                await context.SaveChangesAsync();
                return member.Id;
            }
            catch (InvalidOperationException ex)
            {
                //Логирование ошибки InvalidOperationException
                return null;
            }
        }
    }
}
