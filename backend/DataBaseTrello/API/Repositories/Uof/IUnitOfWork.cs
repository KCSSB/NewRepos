using System.Runtime.CompilerServices;
using API.Repositories.Interfaces;

namespace API.Repositories.Uof
{
    public interface IUnitOfWork
    {
        IBoardRepository BoardRepository { get; set; }
        IMembersOfBoardRepository MembersOfBoardRepository { get; set; }
        IProjectRepository ProjectRepository { get; set; }
        ISessionRepository SessionRepository { get; set; }
        IUserRepository UserRepository { get; set; }
        IProjectUserRepository ProjectUserRepository { get; set; }
        public Task SaveChangesAsync(string loggerMessage, string serviceName, [CallerMemberName] string? operationName = null);
    }
}