using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface IMembersOfBoardRepository
    {
        public Task AddMemberRange(List<MemberOfBoard> members);
    }
}
