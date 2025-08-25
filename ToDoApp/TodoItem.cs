using System.ComponentModel.DataAnnotations;

namespace ToDoApp;

public class TodoItem
{
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsDone { get; set; } = false;
}
