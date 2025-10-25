using API.DTO.Requests.Change;
using API.DTO.Requests.Delete;
using API.DTO.Responses;
using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Repositories.Queries;
using API.Repositories.Uof;
using API.Services.Application.Interfaces;
using DataBaseInfo;
using DataBaseInfo.Entities;
using DataBaseInfo.models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Application.Implementations
{
    public class TaskService
    {
        private readonly string ServiceName = nameof(TaskService);
        private readonly IUnitOfWork _unitOfWork;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private readonly AppDbContext _context;
        private ErrorContextCreator? _errorContextCreator;
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IGetPagesService));
        public TaskService(IUnitOfWork unitOfWork, IErrorContextCreatorFactory errCreatorFactory, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _errCreatorFactory = errCreatorFactory;
            _context = context;
        }
        public async Task<_Task> CreateTaskAsync(int cardId)
        {
            Card? card = await _unitOfWork.CardRepository.GetCardAsync(cardId);
            if (card == null)
                throw new AppException(_errCreator.NotFound("Карточка не найдена"));
            _Task task = new _Task()
            {
                CardId = cardId,
            };

            await _unitOfWork.TaskRepository.AddAsync(task);

            card.Tasks.Add(task);

            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при создании задачи");

            return task;
        }
        public async Task<SubTask> CreateSubTaskAsync(int taskId)
        {
            var task = await _unitOfWork.TaskRepository.GetAsync(taskId);
            if (task == null)
                throw new AppException(_errCreator.NotFound("Задача не найдена"));
            SubTask subTask = new SubTask()
            {
                TaskId = taskId,
            };

            await _unitOfWork.SubTaskRepository.AddAsync(subTask);

            task.SubTasks.Add(subTask);

            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при создании подзадачи");

            return subTask;
        }
        public async Task<SubTask> UpdateSubTaskStatusAsync(int subTaskId)
        {
            var subTask = await _unitOfWork.SubTaskRepository.GetAsync(subTaskId);
            subTask.IsCompleted = !subTask.IsCompleted;
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при изменении состояния подзадачи");
            return subTask;
        }
        public async Task<ResponsibleDTO> AddResponsibleForTaskAsync(int taskId, int memberId)
        {
           var task = await _unitOfWork.TaskRepository.GetAsync(taskId);
            if (task == null)
                throw new AppException(_errCreator.NotFound("Задача не найдена"));
            var member = await _context.MembersOfBoards.Include(mb => mb.Responsibles).Include(mb => mb.ProjectUser).ThenInclude(pu => pu.User).Where(mb => mb.Id == memberId).FirstOrDefaultAsync();
            if (member == null)
                throw new AppException(_errCreator.NotFound("Участник доски не найден"));

            foreach(var mb in member.Responsibles)
            {
                if(mb.TaskId == taskId && mb.MemberOfBoardId == memberId)
                    throw new AppException(_errCreator.Forbidden("Данный пользователь уже является ответственным за задачу"));
            }
            var responsible = new ResponsibleForTask
            {
                TaskId = task.Id,
                MemberOfBoardId = memberId,
            };
            task.Responsibles.Add(responsible);
            member.Responsibles.Add(responsible);
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибки про назначении ответственного");

            return new ResponsibleDTO
            {
                MemberId = memberId,
                FirstName = member.ProjectUser.User.FirstName,
                SecondName = member.ProjectUser.User.SecondName,
                AvatarUrl = member.ProjectUser.User.Avatar,
            };
        }
        public async Task RemoveResponsiblesForTasksAsync(List<DeleteResponsibleForTask> responsibles)
        {
            int count = 0;
            foreach (var responsible in responsibles )
            {
                int memberId = responsible.MemberId;
                int taskId = responsible.TaskId;
               var result = await _context.ResponsibleForTasks.FirstOrDefaultAsync(rft => rft.TaskId == taskId && rft.MemberOfBoardId == memberId);
                if (result != null)
                {
                    _context.ResponsibleForTasks.Remove(result);
                    count++;
                }
            }
            if(count==0)
                throw new AppException(_errCreator.NotFound("Пользователи для отвязки не найдены"));
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при отвязке ответственных");
        }
        public async Task DeleteTasksAsync(List<int> taskIds)
        {
            int count = 0;
            foreach (var taskId in taskIds)
            {
                var task = await _unitOfWork.TaskRepository.GetAsync(taskId);
                if (task != null)
                {
                    await _unitOfWork.TaskRepository.Delete(task);
                    count++;
                }
            }
            if (count == 0)
                throw new AppException(_errCreator.NotFound("Задачи для удаления не были найдены"));
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при удалении задач");
        }
        public async Task DeleteSubTasksAsync(List<int> subTaskIds)
        {
            int count = 0;
            foreach (var subTaskId in subTaskIds)
            {
                var subTask = await _unitOfWork.SubTaskRepository.GetAsync(subTaskId);
                if (subTask != null)
                {
                    _unitOfWork.SubTaskRepository.Delete(subTask);
                    count++;
                }
            }
            if (count == 0)
                throw new AppException(_errCreator.NotFound("Подзадачи для удаления не были найдены"));
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при удалени подзадач");
        }
        public async Task UpdateSubTaskNames(List<ChangedSubTask> subTasks)
        {
            int count = 0;
            foreach (var subTaskDTO in subTasks)
            {
                var subTask = await _unitOfWork.SubTaskRepository.GetAsync(subTaskDTO.SubTaskId);
                if (subTask != null)
                {
                    subTask.Name = subTaskDTO.SubTaskName;
                    count++;
                }
            }
            if (count == 0)
                throw new AppException(_errCreator.NotFound("Подзадачи для обновления не были найдены"));
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при удалени подзадач");
        }
        public async Task ChangeTasksAsync(List<ChangedTask> tasks)
        {
            int count = 0;
            foreach (var taskDTO in tasks)
            {
                var task = await _unitOfWork.TaskRepository.GetAsync(taskDTO.TaskId);
                if (task != null)
                {
                   if(taskDTO.Name != null)
                        task.Name = taskDTO.Name;
                   if(taskDTO.DateOfStartWork!=null)
                        task.DateOfStartWork = taskDTO.DateOfStartWork;
                   if(taskDTO.DateOfDeadline!=null)
                        task.DateOfDeadline = taskDTO.DateOfDeadline;
                   if(taskDTO.Description!=null)
                        task.Description = taskDTO.Description;
                   if(taskDTO.Priority!=null)
                        task.Priority = taskDTO.Priority;
                    count++;
                }
            }
            if (count == 0)
                throw new AppException(_errCreator.NotFound("Задачи для обновления не были найдены"));
            await _unitOfWork.SaveChangesAsync(ServiceName, "Ошибка при удалени подзадач");
        }

    }
}
