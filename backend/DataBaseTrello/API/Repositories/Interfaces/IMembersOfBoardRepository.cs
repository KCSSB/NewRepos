using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IMembersOfBoardRepository
    {
        public Task AddMemberRangeAsync(List<MemberOfBoard> members);
    }
}
