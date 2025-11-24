using System;
using TodoApp.Enums;

namespace TodoApp.Models;

public interface ITodoItemState
{
    int Id { get; }
    Guid UserId { get; set; }
    string? Title { get; set; }
    int Priority { get; set; }
    Category Category { get; set; }
    DateTime Deadline { get; set; }
    bool IsDone { get; set; }
    DateTime DateCreated { get; }
    DateTime DateModified { get; }
    void SetCleanState();
    bool IsDirty {get;}
}
