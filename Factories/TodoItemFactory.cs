using System;
using TodoApp.Models;

namespace TodoApp.Factories;

public class TodoItemFactory : ITodoItemFactory
{
  public ITodoItemState CreateNewTodoItem()
  {
    return new TodoItemState();
  }
}
