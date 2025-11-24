using System;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Mappers;

public class TodoItemMapper : ITodoItemMapper
{
  public TodoItem ToEntity(ITodoItemState viewModel)
  {
    var entity = new TodoItem
    {
      Id = viewModel.Id,
      UserId = viewModel.UserId,
      Title = viewModel.Title,
      Priority = viewModel.Priority,
      Category = viewModel.Category,
      Deadline = viewModel.Deadline,
      IsDone = viewModel.IsDone,
      DateCreated = viewModel.DateCreated,
      DateModified = viewModel.DateModified
    };
    return entity;
  }

  public ITodoItemState ToViewModel(TodoItem entity)
  {
    var viewModel = new TodoItemState(entity.Id, entity.DateCreated, entity.DateModified)
    {
      UserId = entity.UserId,
      Title = entity.Title,
      Priority = entity.Priority,
      Category = entity.Category,
      Deadline = entity.Deadline,
      IsDone = entity.IsDone,
    };
    viewModel.SetCleanState();
    return viewModel;
  }
}
