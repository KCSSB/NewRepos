using API.DTO.Requests;
using DataBaseInfo.models;

namespace API.Services.Application.Interfaces
{
    public interface IBoardService
    {
        public Task<int> CreateBoardAsync(string boardName);
        public Task<Board?> AddLeadToBoardAsync(int boardId, int leadId, int projectId);
        public Task<List<int>> AddProjectUsersInBoardAsync(int boardId, List<int> projectUserIds);
        public Task DeleteBoardsAsync(List<int> boardIds);
        public Task UpdateBoardsNameAsync(List<UpdatedBoard> updateBoards);
    }
}
