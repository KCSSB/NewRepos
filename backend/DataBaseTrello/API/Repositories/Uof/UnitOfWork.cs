using System.Runtime.CompilerServices;
using API.Extensions;
using API.Repositories.Interfaces;
using DataBaseInfo;

namespace API.Repositories.Uof
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IUserRepository UserRepository { get; set; }
        public IProjectRepository ProjectRepository { get; set; }
        public IProjectUserRepository ProjectUserRepository { get; set; }
        public ISessionRepository SessionRepository { get; set; }
        public IMembersOfBoardRepository MembersOfBoardRepository { get; set; }
        public ICardRepository  CardRepository { get; set; }
        public ITaskRepository TaskRepository { get; set; }
        public ISubTaskRepository SubTaskRepository { get; set; }

        public IBoardRepository BoardRepository { get; set; }

        public UnitOfWork(AppDbContext context,
            IUserRepository userRepository,
            IProjectRepository projectRepository,
            ISessionRepository sessionRepository,
            IMembersOfBoardRepository membersOfBoardRepository,
            IBoardRepository boardRepository,
            IProjectUserRepository projectUserRepository,
            ICardRepository cardRepository,
            ITaskRepository taskRepository,
            ISubTaskRepository subTaskRepository)
        {
            _context = context;
            UserRepository = userRepository;
            ProjectRepository = projectRepository;
            SessionRepository = sessionRepository;
            MembersOfBoardRepository = membersOfBoardRepository;
            BoardRepository = boardRepository;
            ProjectUserRepository = projectUserRepository;
            CardRepository = cardRepository;
            TaskRepository = taskRepository;
            SubTaskRepository = subTaskRepository;
        }

        public async Task SaveChangesAsync(string loggerMessage,string serviceName, [CallerMemberName] string? operationName = null)
        {
            await _context.SaveChangesWithContextAsync(loggerMessage,serviceName, operationName);
        }


    }
}
