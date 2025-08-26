using API.DTO.Mappers;
using API.DTO.Responses;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Exceptions.ErrorContext;
using API.Constants;
using System.Net;

namespace API.Helpers
{
    public class ResponseCreator
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public ResponseCreator(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<SummaryProjectResponse> CreateSummaryProjectResponseAsync(Guid projectId)
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
            var summaryProject = Mapper.ToSummaryProjectResponse(project);
            return summaryProject;
        }
    }
}
