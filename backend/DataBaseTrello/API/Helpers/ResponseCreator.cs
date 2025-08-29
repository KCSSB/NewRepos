using API.DTO.Responses;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Exceptions.Context;
using API.Constants;
using System.Net;
using API.DTO.Mappers.ToResponseModel;

namespace API.Helpers
{
    public class ResponseCreator
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public ResponseCreator(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<SummaryProjectResponse> CreateSummaryProjectResponseAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var project = await context.Projects.Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User).FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null)
                throw new AppException(new ErrorContext(ServiceName.ResponseCreator,
                    OperationName.CreateSummaryProjectResponseAsync,
                    HttpStatusCode.InternalServerError,
                    UserExceptionMessages.InternalExceptionMessage,
                    $"Ошибка во время маппинга project {projectId} в SummaryProjectResponse"
                    ));
            var summaryProject = ToResponseMapper.ToSummaryProjectResponse(project);
            return summaryProject;
        }
    }
}
