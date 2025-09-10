using API.Repositories.Queries.Intefaces;
using API.Repositories.Queries.Interfaces;

namespace API.Repositories.Queries
{
    public interface IQueries
    {
        public IBoardQueries BoardQueries { get; set; }
        public IUserQueries UserQueries { get; set; }
        public IProjectQueries ProjectQueries { get; set; }
        public IProjectUserQueries ProjectUserQueries { get; set; }
        public ISessionQueries SessionQueries { get; set; }
    }
}
