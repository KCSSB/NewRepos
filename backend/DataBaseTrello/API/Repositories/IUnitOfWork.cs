using System.Runtime.CompilerServices;
using API.Repositories.Interfaces;

namespace API.Repositories
{
    public interface IUnitOfWork
    {
        IBoardRepository BoardRepository { get; set; }
        IMembersOfBoardRepository MembersOfBoardRepository { get; set; }
        IProjectRepository ProjectRepository { get; set; }
        ISessionRepository SessionRepository { get; set; }
        IUserRepository UserRepository { get; set; }

        Task SaveChangesAsync(string serviceName, [CallerMemberName] string? operationName = null);
    }
}