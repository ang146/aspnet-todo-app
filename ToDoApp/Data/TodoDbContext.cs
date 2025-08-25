using System;
using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }
    
    public DbSet<TodoItem> Items { get; set; }
}
