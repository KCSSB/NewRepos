using API.Repositories.Interfaces;
using API.Repositories.Queries.Intefaces;
using API.Repositories.Queries.Interfaces;

namespace API.Repositories.Queries
{
    public class Queries:IQueries
    {
        public Queries(
        IBoardQueries boardQueries,
        IUserQueries userQueries,
        IProjectQueries projectQueries,
        IProjectUserQueries projectUserQueries,
        ISessionQueries sessionQueries)
        {
            BoardQueries = boardQueries;
            UserQueries = userQueries;
            ProjectQueries = projectQueries;
            ProjectUserQueries = projectUserQueries;
            SessionQueries = sessionQueries;
        }
        public IBoardQueries BoardQueries { get; set; }
        public IUserQueries UserQueries { get; set; }
        public IProjectQueries ProjectQueries { get; set; }
        public IProjectUserQueries ProjectUserQueries { get; set; }
        public ISessionQueries SessionQueries { get; set; }

    }
}
