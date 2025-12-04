using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Managers;

namespace TodoApp.Services;

public class TodoService : ITodoService
{
    private readonly ApplicationDbContext _db;
    private readonly IUserManager _userManager;
    public TodoService(ApplicationDbContext db, IUserManager userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<List<TodoItem>> GetItemsAsync()
    {
        var userId = _userManager.GetCurrentUserIdByHttpContext();
        if (userId == Guid.Empty)
        {
            return new List<TodoItem>();
        }

        return await _db.TodoItems.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task AddItemAsync(TodoItem item)
    {
        _db.TodoItems.Add(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(TodoItem item)
    {
        var toUpdate = (await _db.TodoItems.ToListAsync()).SingleOrDefault(i => i.Id == item.Id);
        if (toUpdate == null)
            return;

        toUpdate.Title = item.Title;
        toUpdate.IsDone = item.IsDone;
        toUpdate.Priority = item.Priority;
        toUpdate.Category = item.Category;
        toUpdate.Deadline = item.Deadline;
        _db.TodoItems.Update(toUpdate);
        await _db.SaveChangesAsync();

    }

    public async Task DeleteItemAsync(int id)
    {
        var toDelete = await _db.TodoItems.FindAsync(id);
        if (toDelete == null)
            return;

        _db.TodoItems.Remove(toDelete);
        await _db.SaveChangesAsync();
    }
}