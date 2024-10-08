using GerenciadorDeTarefasApi.DbContexts;
using GerenciadorDeTarefasApi.Models;
using NSwag.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GerenciadorTarefasDb>(opt => opt.UseInMemoryDatabase("GerenciadorTarefasDB"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "GerenciadorDeTarefasApi";
    config.Title = "GerenciadorDeTarefas v1";
    config.Version = "v1";
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "GerenciadorDeTarefasApi";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

//Projetos
app.MapGet("/projetos", async (GerenciadorTarefasDb db) =>
    await db.Projetos.ToListAsync());

app.MapPost("/projetos", async (Projeto projeto, GerenciadorTarefasDb db) =>
{
    db.Projetos.Add(projeto);
    await db.SaveChangesAsync();

    return Results.Created($"/projetos/{projeto.Id}", projeto);
});

//Tarefas
app.MapGet("/tarefas/{projetoId}", async (Guid projetoId, GerenciadorTarefasDb db) =>
    await db.Tarefas.Where(t => t.ProjetoId == projetoId).ToListAsync());

app.MapPost("/tarefas", async (Tarefa tarefa, GerenciadorTarefasDb db) =>
{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();

    return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
});

app.MapPut("/tarefas/{id}", async (Guid id, Tarefa inputTarefa, GerenciadorTarefasDb db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);

    if (tarefa is null) return Results.NotFound();

    tarefa.Titulo = inputTarefa.Titulo;
    tarefa.Descricao = inputTarefa.Descricao;
    tarefa.Status = inputTarefa.Status;
    tarefa.Vencimento = inputTarefa.Vencimento;
    tarefa.UsuarioId = inputTarefa.UsuarioId;

    //Inserir Historico de Atualizacao da tarefa


    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/tarefas/{id}", async (Guid id, GerenciadorTarefasDb db) =>
{
    if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
    {
        db.Tarefas.Remove(tarefa);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

//#################### Apagar Exemplos
#region  exemplos abaixo
/*app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});*/
#endregion

app.Run();