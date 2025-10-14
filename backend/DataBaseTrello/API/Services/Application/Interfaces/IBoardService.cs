using System.Dynamic;
using API.DTO.Requests.Change;
using API.DTO.Requests.Delete;
using API.DTO.Responses.Pages.WorkSpacePage;
using DataBaseInfo.models;

namespace API.Services.Application.Interfaces
{
    public interface IBoardService
    {
        public Task<int> CreateBoardAsync(string boardName, int projectId);
        public Task<Board?> AddLeadToBoardAsync(int boardId, int leadId, int projectId);
        public Task<List<int>> AddProjectUsersInBoardAsync(int boardId, List<int> projectUserIds);
        public Task DeleteBoardsAsync(List<int> boardIds);
        public Task UpdateBoardsNameAsync(List<UpdatedBoard> updateBoards);
        public Task UpdateBoardNameAsync(int boardId, string name);
        public Task DeleteMembersAsync(int boardId,List<int> memberIds);
        public Task<WorkSpaceMember?> AddMemberAsync(int boardId, int projectUserId);
    }
}
