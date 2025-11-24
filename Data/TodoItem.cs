using System.ComponentModel.DataAnnotations;
using TodoApp.Enums;

namespace TodoApp.Data
{
  public class TodoItem
  {
      [Key]
      public int Id { get; set; }
      public Guid UserId { get; set; }
      public string? Title { get; set; }
      public int Priority { get; set; } = 0;
      public Category Category { get; set; } = Category.Others;
      public DateTime Deadline { get; set; }
      public bool IsDone { get; set; } = false;
      public DateTime DateCreated { get; set; }
      public DateTime DateModified { get; set; }

  }
}
