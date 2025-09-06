using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;

namespace API.Repositories.Implementations
{
    public class MembersOfBoardRepository: IMembersOfBoardRepository
    {
        private readonly AppDbContext _context;
        public MembersOfBoardRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task AddMemberRange(List<MemberOfBoard> members)
        {

        }
    }
}
