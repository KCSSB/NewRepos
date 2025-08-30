using API.DTO.Responses;
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using API.Exceptions.Context;
using API.Constants;
using System.Net;
using API.DTO.Mappers.ToResponseModel;
using API.Exceptions;

namespace API.Helpers
{
    public class ResponseCreator
    {

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ErrorContextCreator _errCreator;
        public ResponseCreator(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _errCreator = new ErrorContextCreator(ServiceName.ResponseCreator);
        }
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
