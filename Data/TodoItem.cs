using System.ComponentModel.DataAnnotations;

namespace TodoApp.Data
{
  public class TodoItem
  {
      [Key]
      public int Id { get; set; }
      public Guid UserId { get; set; }
      public string? Title { get; set; }
      public bool IsDone { get; set; } = false;
      public DateTime DateCreated { get; set; }
      public DateTime DateModified { get; set; }

  }
}
