using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Exceptions.Context;
using API.DTO.Mappers;
using API.Exceptions.ContextCreator;
using API.Services.Application.Interfaces;
using API.Repositories.Queries;
using API.Repositories.Uof;
using API.DTO.Responses.Pages.HomePage;
using API.DTO.Responses.Pages.SettingsPage;

namespace API.Services.Application.Implementations
{
    public class GetPagesService: IGetPagesService
    {
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly ILogger<IGetPagesService> _logger;
        private ErrorContextCreator? _errorContextCreator;
        private readonly IQueries _query;
        private readonly IUnitOfWork _unitOfWork;
        public GetPagesService(ILogger<IGetPagesService> logger,
            AppDbContext context, 
            IErrorContextCreatorFactory errCreatorFactory,
            IQueries query,
            IUnitOfWork unitOfWork)
        {
          _logger = logger;  
          _errCreatorFactory = errCreatorFactory;
            _query = query;
            _unitOfWork = unitOfWork;
            
        }
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IGetPagesService));
        public async Task<HomePage> CreateHomePageDTOAsync(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetDbUserAsync(userId);

            if(user == null)
                throw new AppException(_errCreator.NotFound($"Произошла ошибка в процессе формирования HomePage, Пользователь id: {userId}, не найден в базе данных"));

            var projects = await _query.ProjectQueries.GetAllProjectsWhereUserAsync(userId);

            var summaryProjects = projects.Select(ToResponseMapper.ToSummaryProjectResponse).ToList();
            return new HomePage { SummaryProject = summaryProjects };
        }
        public async Task<SettingsPage> CreateSettingsPageDTOAsync(int userId)
        {

            var user = await _unitOfWork.UserRepository.GetDbUserAsync(userId);
            if (user == null)
                throw new AppException(_errCreator.NotFound($"Произошла ошибка в процессе формирования SettingsPage, Пользователь id: {userId}, не найден в базе данных"));

            var page = ToResponseMapper.ToSettingsPageResponse(user);
            return page;
        }
    }
}
