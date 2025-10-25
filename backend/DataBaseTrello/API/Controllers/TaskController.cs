using System.Security.Cryptography.X509Certificates;
using API.DTO.Mappers;
using API.DTO.Requests.Change;
using API.DTO.Requests.Delete;
using API.DTO.Responses.Pages.WorkSpacePage;
using API.Exceptions.ContextCreator;
using API.Extensions;
using API.Services.Application.Implementations;
using API.Services.Application.Interfaces;
using API.Services.Helpers;
using API.Services.Helpers.Interfaces;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/project/{projectId}/board/{boardId}/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;
        private readonly IRolesHelper _rolesHelper;
        public TaskController(TaskService taskService, IRolesHelper rolesHelper)
        {
            _taskService = taskService;
            _rolesHelper = rolesHelper;
        }

        [HttpDelete("DeleteResponsibles")]
        public async Task<IActionResult> DeleteResponsiblesForTask(int projectId,int boardId, [FromBody] DeleteResponsiblesForTask request)
        {
            var userId = User.GetUserId();
            var responsibles = request.responsibles;
            await _taskService.RemoveResponsiblesForTasksAsync(request.responsibles);
            return Ok("Пользователи успешно отвязаны");
        }
        [HttpDelete("DeleteTasks")]
        public async Task<IActionResult> DeleteTasks(int projectId, int boardId, [FromBody] DeleteTasksRequest request)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);
            await _taskService.DeleteTasksAsync(request.TasksIds);

            return Ok("Задачи были удалены");
        }
        [HttpDelete("DeleteSubTasks")]
        public async Task<IActionResult> DeleteSubTasks(int projectId, int boardId, [FromBody] DeleteSubTasksRequest request)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);

            await _taskService.DeleteSubTasksAsync(request.SubTasksIds);
            return Ok("Подзадачи были удалены");
        }
        [HttpPatch("ChangeSubTasksNames")]
        public async Task<IActionResult> ChangeSubTasksNames(int projectId, int boardId, [FromBody] ChangedSubTasksRequest request)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);

            await _taskService.UpdateSubTaskNames(request.subTasks);
            return Ok("Подзадачи были изменены");
        }
        [HttpPatch("UpdateSubTaskStatus/{subTaskId}")]
        public async Task<IActionResult> UpdateSubTaskStatus(int projectId, int boardId, int subTaskId, [FromBody] UpdateSubTaskRequest request)
        {
            var userId = User.GetUserId();
            
            await _rolesHelper.IsMemberOfBoard(userId, boardId);
            bool status = request.IsCompleted;
            var subTask = await _taskService.UpdateSubTaskStatusAsync(subTaskId);
            ToResponseMapper.ToWorkSpaceSubTask(subTask);
            return Ok(subTask);
        }
        [HttpPatch("ChangeTasks")]
        public async Task<IActionResult> ChangeTasks(int projectId, int boardId, [FromBody] TasksChangeRequest request)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);

            var tasks = request.tasks;
            await _taskService.ChangeTasksAsync(tasks);
            return Ok("Задачи изменены");
        }
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask(int projectId, int boardId, int cardId)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);

            var task = await _taskService.CreateTaskAsync(cardId);
            var taskResponse = ToResponseMapper.ToWorkSpaceTask(task);
            return Ok(taskResponse);
        }
        [HttpPost("CreateSubTask")]
        public async Task<IActionResult> CreateSubTask(int projectId, int boardId, int taskId)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);

            var subTask = await _taskService.CreateSubTaskAsync(taskId);
            var subTaskResponse = ToResponseMapper.ToWorkSpaceSubTask(subTask);
            return Ok(subTaskResponse);
        }
        [HttpPost("AddResponsibleTask")]
        public async Task<IActionResult> AddResponsibleTask(int projectId, int boardId, int taskId, int memberId)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);
            var responsible = await _taskService.AddResponsibleForTaskAsync(taskId,memberId);
            return Ok(responsible);
        }
        [HttpDelete("RemoveResponsiblesTasks")]
        public async Task<IActionResult> RemoveResponsiblesTasks(int projectId, int boardId, [FromBody] DeleteResponsiblesForTask request)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);
            await _taskService.RemoveResponsiblesForTasksAsync(request.responsibles);
            return Ok("Участники успешно отвязаны");
        }
    }
}
