using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;

namespace TodoApp.Services;

public class TodoService : ITodoService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TodoService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<TodoItem>> GetItemsAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
        {
            return new List<TodoItem>();
        }

        var userIdS = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var success = Guid.TryParse(userIdS, out var userId);
        if (!success)
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