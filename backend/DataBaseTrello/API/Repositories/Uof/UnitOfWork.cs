using System.Runtime.CompilerServices;
using API.Extensions;
using API.Repositories.Interfaces;
using DataBaseInfo;

namespace API.Repositories.Uof
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IUserRepository UserRepository { get; set; }
        public IProjectRepository ProjectRepository { get; set; }
        public ISessionRepository SessionRepository { get; set; }
        public IMembersOfBoardRepository MembersOfBoardRepository { get; set; }
        public IBoardRepository BoardRepository { get; set; }

        public async Task SaveChangesAsync(string loggerMessage,string serviceName, [CallerMemberName] string? operationName = null)
        {
            await _context.SaveChangesWithContextAsync(loggerMessage,serviceName, operationName);
        }


    }
}
