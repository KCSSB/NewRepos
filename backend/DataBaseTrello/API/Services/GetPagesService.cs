using API.DTO.Responses.Pages;
using API.DTO.Responses;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;
using API.Constants;
using API.Exceptions.ErrorContext;
using System.Net;

namespace API.Services
{
    public class GetPagesService
    {
        private readonly ILogger<GetPagesService> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public GetPagesService(ILogger<GetPagesService> logger, IDbContextFactory<AppDbContext> contextFactory)
        {
          _logger = logger;  
          _contextFactory = contextFactory;
        }
        public async Task<HomePage> GetHomePageAsync(Guid userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if(user == null)
                throw new AppException(new ErrorContext(ServiceName.UserService,
                     OperationName.UploadUserAvatarAsync,
                     HttpStatusCode.NotFound,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"Произошла ошибка в процессе формирования HomePage, Пользователь id: {userId}, не найден в базе данных"));

            var avatar = user.Avatar;
            var projects = await context.Projects.Where(p => p.ProjectUsers.Any(u => u.UserId == userId))
                .Select(p => new SummaryProjectResponse
                {
                    ProjectId = p.Id,
                    ProjectName = p.ProjectName,
                    CountProjectUsers = p.ProjectUsers.Count(),
                    ProjectLeader = p.ProjectUsers
                    .Where(pu => pu.ProjectRole == "Owner")
                    .Select(pl => new ProjectLeaderResponse
                    {
                        ProjectLeaderId = pl.UserId,
                        ProjectLeaderName = pl.User.FirstName + " " + pl.User.SecondName,
                        ProjectLeaderImageUrl = pl.User.Avatar
                    }).FirstOrDefault()
                }).ToListAsync();
            return new HomePage { SummaryProject = projects };
        }
    }
}
