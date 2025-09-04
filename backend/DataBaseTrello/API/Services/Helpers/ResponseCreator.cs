using API.DTO.Responses;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Exceptions.Context;
using API.Constants;
using System.Net;
using API.DTO.Mappers;
using API.Middleware;
using API.Exceptions.ContextCreator;

namespace API.Services.Helpers
{
    public class ResponseCreator
    {

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;


        public ResponseCreator(IDbContextFactory<AppDbContext> contextFactory,IErrorContextCreatorFactory errCreatorFactory)
        {
        _errCreatorFactory = errCreatorFactory;
            _contextFactory = contextFactory;
       
        }
private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(ResponseCreator));
        public async Task<SummaryProjectResponse> CreateSummaryProjectResponseAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var project = await context.Projects.Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User).FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                throw new AppException(_errCreator.InternalServerError($"Ошибка во время маппинга project {projectId} в SummaryProjectResponse"));

            var summaryProject = ToResponseMapper.ToSummaryProjectResponse(project);
            return summaryProject;
        }
    }
}
