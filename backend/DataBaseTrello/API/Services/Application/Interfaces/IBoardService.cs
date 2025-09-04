namespace API.Services.Application.Interfaces
{
    public interface IBoardService
    {
        public Task<int> CreateBoardAsync(string boardName);
        public Task<List<int>> AddProjectUsersInBoardAsync(int boardId, int boardLeadId, List<int> projectUserIds);
        
    }
}
