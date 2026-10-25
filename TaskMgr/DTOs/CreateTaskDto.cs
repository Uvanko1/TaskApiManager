namespace TaskMgr.DTOs;
using System.ComponentModel.DataAnnotations;

public class CreateTaskDto
{
    [Required]
    [StringLength(100)]
    public string Title {get; set;} = "";
    public bool IsCompleted {get; set;}
}
