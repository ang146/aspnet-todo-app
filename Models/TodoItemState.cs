using System;
using TodoApp.Enums;

namespace TodoApp.Models;

public class TodoItemState : ITodoItemState
{
  private Guid _userId;
  private string? _title;
  private int _priority = 0;
  private Category _category = Category.Others;
  private DateTime _deadline;
  private bool _isDone = false;

  public TodoItemState()
  {
    DateCreated = DateTime.UtcNow;
    DateModified = DateTime.UtcNow;
  }

  public TodoItemState(int id, DateTime dateCreated, DateTime dateModified)
  {
    Id = id;
    DateCreated = dateCreated;
    DateModified = dateModified;
  }
  
  private void SetDirty()
  {
    IsDirty = true;
    DateModified = DateTime.UtcNow;
  }

  public int Id { get; }
  public Guid UserId 
  { 
    get => _userId;
    set
    {
      SetDirty();
      _userId = value;
    }
  }
  public string? Title 
  { 
    get => _title;
    set
    {
      SetDirty();
      _title = value;
    }
  }
  public int Priority 
  { 
    get => _priority;
    set
    {
      SetDirty();
      _priority = value;
    }
  }
  public Category Category 
  { 
    get => _category;
    set
    {
      SetDirty();
      _category = value;
    }
  }
  public DateTime Deadline 
  { 
    get => _deadline;
    set
    {
      SetDirty();
      _deadline = value;
    }
  }
  public bool IsDone 
  { 
    get => _isDone;
    set
    {
      SetDirty();
      _isDone = value;
    }
  }
  public DateTime DateCreated { get; }
  public DateTime DateModified { get; private set;}
  
  public bool IsDirty { get; private set;}

  public void SetCleanState()
  {
    IsDirty = false;
  }
}
