using API.Exceptions.Context;
using API.Exceptions.ContextCreator;
using API.Repositories.Uof;
using API.Services.Application.Interfaces;
using DataBaseInfo.Entities;
using DataBaseInfo.models;

namespace API.Services.Application.Implementations
{
    public class TaskService
    {
        private readonly string ServiceName = nameof(TaskService);
        private readonly IUnitOfWork _unitOfWork;
        private readonly IErrorContextCreatorFactory _errCreatorFactory;
        private ErrorContextCreator? _errorContextCreator;
        private ErrorContextCreator _errCreator => _errorContextCreator ??= _errCreatorFactory.Create(nameof(IGetPagesService));
        public TaskService(IUnitOfWork unitOfWork, IErrorContextCreatorFactory errCreatorFactory)
        {
            _unitOfWork = unitOfWork;
            _errCreatorFactory = errCreatorFactory;
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
        
    }
}
