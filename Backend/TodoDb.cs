using Microsoft.EntityFrameworkCore;

namespace Backend;

public class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
}
