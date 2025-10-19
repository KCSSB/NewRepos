using API.Repositories.Interfaces;
using DataBaseInfo.models;
using DataBaseInfo;
using DataBaseInfo.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace API.Repositories.Implementations
{
    public class SubTaskRepository:ISubTaskRepository
    {
        private readonly AppDbContext _context;
        public SubTaskRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(SubTask subTask)
        {
            await _context.SubTasks.AddAsync(subTask);
        }
        public async Task<SubTask?> GetAsync(int subTaskId)
        {
            return await _context.SubTasks.FirstOrDefaultAsync(st => st.Id == subTaskId);
        }
        public void Delete(SubTask subTask)
        {
            _context.SubTasks.Remove(subTask);
        }
    }
}
