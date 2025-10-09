using System.Security.Cryptography.X509Certificates;
using API.DTO.Mappers;
using API.DTO.Requests;
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

            return Ok("Пользователи успешно отвязаны");
        }
        [HttpDelete("DeleteTasks")]
        public async Task<IActionResult> DeleteTasks(int projectId, int boardId, List<int> TasksIds)
        {
            var userId = User.GetUserId();
            return Ok("Задачи были удалены");
        }
        [HttpDelete("DeleteSubTasks")]
        public async Task<IActionResult> DeleteSubTasks(int projectId, int boardId, List<int> SubTasks)
        {
            var userId = User.GetUserId();
            return Ok("Подзадачи были удалены");
        }
        [HttpPatch("ChangeSubTasksNames")]
        public async Task<IActionResult> ChangeSubTasksNames(int projectId, int boardId, [FromBody] ChangedSubTasksRequest request)
        {
            var userId = User.GetUserId();
            return Ok("Подзадачи были изменены");
        }
        [HttpPatch("UpdateSubTaskStatus/{subTaskId}")]
        public async Task<IActionResult> UpdateSubTaskStatus(int projectId, int boardId, [FromBody] UpdateSubTaskRequest request)
        {
            return Ok("состояние подзадачи изменено");
        }
        [HttpPatch("ChangeTasks")]
        public async Task<IActionResult> ChangeTasks(int projectId, int boardId, [FromBody] TasksChangeRequest request)
        {
            var userId = User.GetUserId();
            var tasks = request.tasks;
            return Ok("Задачи изменены");
        }
        [HttpPost("CreateTask")]
        public async Task<IActionResult> CreateTask(int projectId, int boardId)
        {
            var userId = User.GetUserId();
            await _rolesHelper.IsProjectOwnerOrLeaderOfBoard(userId, projectId, boardId);

            var task = await _taskService.CreateTaskAsync(boardId);
            var taskResponse = ToResponseMapper.ToWorkSpaceTask(task);
            return Ok(task);
        }
        [HttpPost("CreateSubTask")]
        public async Task<IActionResult> CreateSubTask(int projectId, int boardId)
        {
            WorkSpaceSubTask subTask = new WorkSpaceSubTask();
            return Ok(subTask);
        }
    }
}
