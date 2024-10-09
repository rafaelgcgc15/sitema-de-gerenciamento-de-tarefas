using GerenciadorDeTarefasApi.DbContexts;
using GerenciadorDeTarefasApi.Models;
using NSwag.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using GerenciadorDeTarefasApi.DTOs;

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

var projetos = app.MapGroup("/projetos");
var tarefas =  app.MapGroup("/tarefas");

projetos.MapGet("/", GetAllProjetos);
projetos.MapPost("/", CreateProjeto);
tarefas.MapGet("/{projetoId}", GetTarefasProjeto);
tarefas.MapPost("/", CreateTarefa);
tarefas.MapPut("/{id}", UpdateTarefa);
tarefas.MapDelete("/{id}", DeleteTarefa);

//Projetos
static async Task<IResult> GetAllProjetos(GerenciadorTarefasDb db)
{
    return TypedResults.Ok(await db.Projetos.ToArrayAsync());
}

static async Task<IResult> CreateProjeto(Projeto projeto, GerenciadorTarefasDb db)
{
    db.Projetos.Add(projeto);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/projetos/{projeto.Id}", projeto);
}

//Tarefas
static async Task<IResult> GetTarefasProjeto(Guid projetoId, GerenciadorTarefasDb db)
{
    return TypedResults.Ok(await db.Tarefas.Where(t => t.ProjetoId == projetoId).ToListAsync());
}

static async Task<IResult> CreateTarefa(TarefaPostDTO tarefaDTO, GerenciadorTarefasDb db)
{
    Tarefa tarefa = new Tarefa{
          Descricao = tarefaDTO.Descricao,
          Titulo = tarefaDTO.Titulo,
          Prioridade = tarefaDTO.Prioridade,
          Status = tarefaDTO.Status,
          ProjetoId = tarefaDTO.ProjetoId     
    };

    //valida tarefa
    if(string.IsNullOrWhiteSpace(tarefa.Descricao))
        return TypedResults.Problem($"O Campo {nameof(tarefa.Descricao)} deve ser preenchido.");

    if(string.IsNullOrWhiteSpace(tarefa.Titulo))
        return TypedResults.Problem($"O Campo {nameof(tarefa.Titulo)} deve ser preenchido.");

    if(tarefa.Prioridade < 1 || tarefa.Prioridade > 3)
        return TypedResults.Problem($"O Campo {nameof(tarefa.Prioridade)} deve ser preenchido com valor de 1 a 3. Onde 1 e prioridade alta, 2 media e 3 baixa.");
    
    //valida projeto tarefa
    if(string.IsNullOrWhiteSpace(tarefa.ProjetoId.ToString()))
        return TypedResults.Problem($"O Campo {nameof(tarefa.ProjetoId)} deve ser preenchido.");

    var projeto = await db.Tarefas.Where(t => t.ProjetoId == tarefa.ProjetoId).ToListAsync();
    if (!projeto.Any())
        return TypedResults.Problem($"O projeto de id \"{tarefa.ProjetoId}\" nao foi encontrado.");
    
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/tarefas/{tarefa.Id}", tarefa);
}

static async Task<IResult> UpdateTarefa(Guid id, Tarefa inputTarefa, GerenciadorTarefasDb db)
{
    var tarefa = await db.Tarefas.FindAsync(id);

    if (tarefa is null) return TypedResults.NotFound();

    tarefa.Titulo = inputTarefa.Titulo;
    tarefa.Descricao = inputTarefa.Descricao;
    tarefa.Status = inputTarefa.Status;
    tarefa.Vencimento = inputTarefa.Vencimento;
    tarefa.UsuarioId = inputTarefa.UsuarioId;

    //Inserir Historico de Atualizacao da tarefa

    await db.SaveChangesAsync();

    return TypedResults.NoContent();

}

static async Task<IResult> DeleteTarefa(Guid id, GerenciadorTarefasDb db)
{
    if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
    {
        db.Tarefas.Remove(tarefa);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
};

app.Run();