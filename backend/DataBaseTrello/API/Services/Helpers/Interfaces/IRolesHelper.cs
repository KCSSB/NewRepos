namespace API.Services.Helpers.Interfaces
{
    public interface IRolesHelper
    {
        public Task<bool> IsProjectOwner(int userId, int projectId);
        public Task<bool> IsLeadOfBoard(int userId, int boardId);
        public Task<bool> IsProjectOwnerOrLeaderOfBoard(int userId, int projectId, int boardId);
        public Task<bool> IsMemberOfBoard(int userId, int boardId);
    }
}
