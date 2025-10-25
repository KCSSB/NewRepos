using DataBaseInfo.Entities;
using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface ISubTaskRepository
    {
        public Task AddAsync(SubTask subTask);
        public Task<SubTask?> GetAsync(int subTaskId);
        public void Delete(SubTask subTask);
    }
}
