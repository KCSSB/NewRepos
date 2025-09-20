using DataBaseInfo.models;

namespace API.Services.Application.Interfaces
{
    public interface IBoardService
    {
        public Task<int> CreateBoardAsync(string boardName);
        public Task AddLeadToBoardAsync(int boardId, int leadId, int projectId);
        public Task<List<int>> AddProjectUsersInBoardAsync(int boardId, List<int> projectUserIds);
        
    }
}
