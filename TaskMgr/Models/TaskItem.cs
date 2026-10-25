using System.ComponentModel.DataAnnotations;

namespace TaskMgr.Models;

public class TaskItem
{
    [Range(1, int.MaxValue)]
    public int Id {get; set;}
    [Required]
    [StringLength(100)]
    public string Title {get; set;} = "";
    public bool IsCompleted {get; set;}
}
