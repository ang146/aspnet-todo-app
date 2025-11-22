using TodoApp.Data;

namespace TodoApp.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetItemsAsync();
    Task AddItemAsync(TodoItem item);
    Task UpdateItemAsync(TodoItem item);
    Task DeleteItemAsync(int id);
}