using API.Repositories.Interfaces;
using DataBaseInfo;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

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
        public async Task AddMemberAsync(MemberOfBoard member)
        {
            await _context.AddAsync(member);
        }
        public async Task<MemberOfBoard?> GetMemberOfBoardAsync(int userId, int boardId)
        {
            return await _context.Users
        .Where(u => u.Id == userId)
        .SelectMany(u => u.ProjectUsers)
        .SelectMany(pu => pu.MembersOfBoards) 
        .Where(m => m.BoardId == boardId) 
        .FirstOrDefaultAsync();
        }
        public async Task<MemberOfBoard?> GetMemberOfBoardAsync(int memberId)
        {
            return await _context.MembersOfBoards.FirstOrDefaultAsync(mb => mb.Id == memberId);
        }
        public void RemoveMember(MemberOfBoard member)
        {
            _context.MembersOfBoards.Remove(member);
        }
    }
}
