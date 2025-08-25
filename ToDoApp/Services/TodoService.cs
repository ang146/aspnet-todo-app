using System;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;

namespace ToDoApp.Services;

public class TodoService : ITodoService
{
    private readonly TodoDbContext _db;
    public TodoService(TodoDbContext db)
    {
        _db = db;
    }

    public async Task<List<TodoItem>> GetItemsAsync()
    {
        return await _db.Items.ToListAsync();
    }

    public async Task AddItemAsync(TodoItem item)
    {
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateItemAsync(TodoItem item)
    {
        var toUpdate = (await _db.Items.ToListAsync()).SingleOrDefault(i => i.Id == item.Id);
        if (toUpdate == null)
            return;

        toUpdate.Title = item.Title;
        toUpdate.IsDone = item.IsDone;
        _db.Items.Update(toUpdate);
        await _db.SaveChangesAsync();

    }

    public async Task DeleteItemAsync(int id)
    {
        var toDelete = await _db.Items.FindAsync(id);
        if (toDelete == null)
            return;

        _db.Items.Remove(toDelete);
        await _db.SaveChangesAsync();
    }
}
