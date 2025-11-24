using System;
using TodoApp.Models;

namespace TodoApp.Factories;

public interface ITodoItemFactory
{
  ITodoItemState CreateNewTodoItem();
}
