using DataBaseInfo.models;

namespace API.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        public Task AddAsync(_Task task);
        public Task<_Task?> GetAsync(int taskId);
    }
}
