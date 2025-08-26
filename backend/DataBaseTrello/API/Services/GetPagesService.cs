using API.DTO.Responses.Pages;
using API.DTO.Responses;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;
using API.Constants;
using API.Exceptions.ErrorContext;
using System.Net;
using API.DTO.Mappers.ToResponseModel;

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
        public async Task<HomePage> CreateHomePageDTOAsync(Guid userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user == null)
                throw new AppException(new ErrorContext(ServiceName.UserService,
                     OperationName.UploadUserAvatarAsync,
                     HttpStatusCode.NotFound,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"Произошла ошибка в процессе формирования HomePage, Пользователь id: {userId}, не найден в базе данных"));

            var projects = await context.Projects
                .Where(p => p.ProjectUsers.Any(u => u.UserId == userId))
                .Include(p => p.ProjectUsers).ThenInclude(pu => pu.User)
                .ToListAsync();
            var summaryProjects = projects.Select(ToResponseMapper.ToSummaryProjectResponse).ToList();
            return new HomePage { SummaryProject = summaryProjects };
        }
        public async Task<SettingsPage> CreateSettingsPageDTOAsync(Guid userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new AppException(new ErrorContext(ServiceName.UserService,
                     OperationName.UploadUserAvatarAsync,
                     HttpStatusCode.NotFound,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"Произошла ошибка в процессе формирования SettingsPage, Пользователь id: {userId}, не найден в базе данных"));


            var page = ToResponseMapper.ToSettingsPageResponse(user);
            return page;
        }
    }
}
