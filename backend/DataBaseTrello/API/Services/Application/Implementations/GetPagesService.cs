using API.DTO.Responses.Pages;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Exceptions.Context;
using API.DTO.Mappers;
using API.Exceptions.ContextCreator;
using API.Services.Application.Interfaces;

namespace API.Services.Application.Implementations
{
    public class GetPagesService: IGetPagesService
    {
        private readonly ILogger<IGetPagesService> _logger;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;


        public GetPagesService(ILogger<IGetPagesService> logger, IDbContextFactory<AppDbContext> contextFactory, IErrorContextCreatorFactory errCreatorFactory)
        {
          _logger = logger;  
          _contextFactory = contextFactory;
        _errCreatorFactory = errCreatorFactory;
            
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IGetPagesService));
        public async Task<HomePage> CreateHomePageDTOAsync(int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user == null)
                throw new AppException(_errCreator.NotFound($"Произошла ошибка в процессе формирования HomePage, Пользователь id: {userId}, не найден в базе данных"));

            var projects = await context.Projects
                .Where(p => p.ProjectUsers.Any(u => u.UserId == userId))
                .Include(p => p.ProjectUsers).ThenInclude(pu => pu.User)
                .ToListAsync();

            var summaryProjects = projects.Select(ToResponseMapper.ToSummaryProjectResponse).ToList();
            return new HomePage { SummaryProject = summaryProjects };
        }
        public async Task<SettingsPage> CreateSettingsPageDTOAsync(int userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new AppException(_errCreator.NotFound($"Произошла ошибка в процессе формирования SettingsPage, Пользователь id: {userId}, не найден в базе данных"));

            var page = ToResponseMapper.ToSettingsPageResponse(user);
            return page;
        }
    }
}
