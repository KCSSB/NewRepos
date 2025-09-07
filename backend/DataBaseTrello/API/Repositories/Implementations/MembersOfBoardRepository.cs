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
        public async Task AddMemberRangeAsync(List<MemberOfBoard> members)
        {
            foreach (var member in members)
                await _context.AddAsync(member);
        }
    }
}
