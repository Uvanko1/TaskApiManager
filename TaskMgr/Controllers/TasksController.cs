using Microsoft.AspNetCore.Mvc;
using TaskMgr.Models;
using TaskMgr.Services;
using TaskMgr.DTOs;

[ApiController]
[Route("api/[controller]")]
public class TasksController: ControllerBase
{
    private readonly TaskStore _taskStore;
    public TasksController(TaskStore tasks)
    {
        _taskStore = tasks;
    }
    [HttpGet]
    public async Task<TaskItem[]> GetTasks()
    {
        return await _taskStore.GetAll();
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto taskDto)
    {
        var task = new TaskItem
        {
            Title = taskDto.Title,
            IsCompleted = taskDto.IsCompleted
        };
        await _taskStore.Add(task);
        return CreatedAtAction(nameof(GetTaskById), new {id = task.Id}, task);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetTaskById(int id)
    {
        TaskItem? result = await _taskStore.GetById(id);
        if (result == null)
        {
            return NotFound();
        }
        return result;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTaskById(int id)
    {
        var result = await _taskStore.DeleteById(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskItem[]>> UpdateTaskById([FromBody] CreateTaskDto taskDto, int id)
    {
        var task = new TaskItem
        {
            Title = taskDto.Title,
            IsCompleted = taskDto.IsCompleted
        };
        var result = await _taskStore.UpdateById(task, id);
        if (!result)
        {
            return NotFound();
        }
        return await _taskStore.GetAll();
    }
}
