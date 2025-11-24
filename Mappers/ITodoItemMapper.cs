using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Mappers;

public interface ITodoItemMapper
{
  ITodoItemState ToViewModel(TodoItem entity);
  TodoItem ToEntity(ITodoItemState viewModel);
}