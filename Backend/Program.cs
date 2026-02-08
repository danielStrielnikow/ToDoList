using Microsoft.EntityFrameworkCore;
using Backend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TodoDb")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDb>();
    db.Database.EnsureCreated();
}

app.UseCors();

// GET /api/todos - pobierz wszystkie zadania
app.MapGet("/api/todos", async (TodoDb db) =>
    await db.Todos.ToListAsync());

// POST /api/todos - dodaj nowe zadanie
app.MapPost("/api/todos", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

// PUT /api/todos/{id} - zaktualizuj zadanie
app.MapPut("/api/todos/{id}", async (int id, TodoItem updatedTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = updatedTodo.Title;
    todo.IsCompleted = updatedTodo.IsCompleted;
    await db.SaveChangesAsync();

    return Results.Ok(todo);
});

// DELETE /api/todos/{id} - usun zadanie
app.MapDelete("/api/todos/{id}", async (int id, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
