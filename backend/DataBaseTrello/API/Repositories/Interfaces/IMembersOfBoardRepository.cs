using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IMembersOfBoardRepository
    {
        public Task AddMemberRangeAsync(List<MemberOfBoard> members);
        public Task AddMemberAsync(MemberOfBoard memberOfBoard);
        public Task<MemberOfBoard?> GetMemberOfBoardAsync(int userId, int boardId);
    }
}
