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
//if (app.Environment.IsDevelopment())
//{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "GerenciadorDeTarefasApi";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
//}

var configuracaoDadosBasicoTeste = app.MapGroup("/configuracaoDadosBasicoTeste");
var usuarios = app.MapGroup("/usuarios");
var projetos = app.MapGroup("/projetos");
var tarefas =  app.MapGroup("/tarefas");

configuracaoDadosBasicoTeste.MapPost("/", ConfiguraDadosBasicos);
usuarios.MapGet("/", GetAllUsuarios);
projetos.MapGet("/", GetAllProjetos);
projetos.MapPost("/", CreateProjeto);
tarefas.MapGet("/{projetoId}", GetTarefasProjeto);
tarefas.MapPost("/", CreateTarefa);
tarefas.MapPut("/{id}", UpdateTarefa);
tarefas.MapDelete("/{id}", DeleteTarefa);

//Configura dados basicos para teste.
static async Task<IResult> ConfiguraDadosBasicos(GerenciadorTarefasDb db)
{
    Usuario usuario = new Usuario { Id = new Guid("92974a6f-a28e-4337-8d8c-7e326ea3c15a"), Nome = "Rafael da Silva" };
    Projeto projeto = new Projeto { Id = new Guid("655a71a1-c983-4eb2-95e1-4f08c6bc1498"), Nome = "Projeto de Gerenciamento de Tarefas V1", Descricao = "Esse projeto consiste em criar uma api para gerenciamento de tarefas." };


    db.Usuarios.Add(usuario);
    db.Projetos.Add(projeto);
    await db.SaveChangesAsync();

    /*Tarefa tarefa1 = new Tarefa{
         Titulo = "Criar Solution",
         Descricao = "Criar solucao do projeto",
         Prioridade = 1,
         Status = 1,
         Vencimento = DateTime.Now.AddDays(5),
         ProjetoId = new Guid("655a71a1-c983-4eb2-95e1-4f08c6bc1498"),
         UsuarioId = new Guid("92974a6f-a28e-4337-8d8c-7e326ea3c15a")
    };
    Tarefa tarefa2 = new Tarefa{
         Titulo = "Criar WebApi",
         Descricao = "Criar cproj da Api minimal",
         Prioridade = 1,
         Status = 1,
         Vencimento = DateTime.Now.AddDays(5),
         ProjetoId = new Guid("655a71a1-c983-4eb2-95e1-4f08c6bc1498"),
         UsuarioId = new Guid("92974a6f-a28e-4337-8d8c-7e326ea3c15a")
    };

    db.Tarefas.Add(tarefa1);
    db.Tarefas.Add(tarefa2);

    await db.SaveChangesAsync();*/

    return TypedResults.Ok();
}

//Usuarios
static async Task<IResult>GetAllUsuarios(GerenciadorTarefasDb db)
{
    return TypedResults.Ok(await db.Usuarios.ToArrayAsync());
}

//Projetos
static async Task<IResult> GetAllProjetos(GerenciadorTarefasDb db)
{
    return TypedResults.Ok(await db.Projetos.ToArrayAsync());
}

static async Task<IResult> CreateProjeto(ProjetoPostDTO projetoDTO, GerenciadorTarefasDb db)
{
    Projeto projeto = new Projeto{
        Nome = projetoDTO.Nome,
        Descricao = projetoDTO.Descricao
    };

    //valida projeto
    if(string.IsNullOrWhiteSpace(projeto.Nome))
        return TypedResults.Problem($"O Campo {nameof(projeto.Nome)} deve ser preenchido.");

    if(string.IsNullOrWhiteSpace(projeto.Descricao))
        return TypedResults.Problem($"O Campo {nameof(projeto.Descricao)} deve ser preenchido.");


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
          Prioridade = ((int)tarefaDTO.Prioridade),
          Status = ((int)tarefaDTO.Status),
          ProjetoId = tarefaDTO.ProjetoId,
          UsuarioId = tarefaDTO.UsuarioId
    };

    //valida tarefa
    if(string.IsNullOrWhiteSpace(tarefa.Descricao))
        return TypedResults.Problem($"O Campo {nameof(tarefa.Descricao)} deve ser preenchido.");

    if(string.IsNullOrWhiteSpace(tarefa.Titulo))
        return TypedResults.Problem($"O Campo {nameof(tarefa.Titulo)} deve ser preenchido.");

    if(tarefa.Prioridade < 1 || tarefa.Prioridade > 3)
        return TypedResults.Problem($"O Campo {nameof(tarefa.Prioridade)} deve ser preenchido com valor de 1 a 3. Onde 1 e prioridade alta, 2 media e 3 baixa.");
    
    if(tarefa.Status < 1 || tarefa.Status > 3)
        return TypedResults.Problem($"O Campo {nameof(tarefa.Status)} deve ser preenchido com valor de 1 a 3. Onde 1 e status pendente, 2 em andamento e 3 concluida.");
    


    //valida projeto tarefa
    if(string.IsNullOrWhiteSpace(tarefa.ProjetoId.ToString()))
        return TypedResults.Problem($"O Campo {nameof(tarefa.ProjetoId)} deve ser preenchido.");

    var projeto = await db.Projetos.Where(t => t.Id == tarefa.ProjetoId).FirstOrDefaultAsync();
    if (projeto == null)
        return TypedResults.Problem($"O projeto de id \"{tarefa.ProjetoId}\" nao foi encontrado.");
    
    if(projeto.Tarefas != null && projeto.Tarefas.Count() == 20)
        return TypedResults.Problem("Cada projeto pode ter no maximo 20 tarefas.");

    //valida usuario tarefa
    if(string.IsNullOrWhiteSpace(tarefa.UsuarioId.ToString()))
        return TypedResults.Problem($"O Campo {nameof(tarefa.UsuarioId)} deve ser preenchido.");

    var usuario = await db.Usuarios.Where(u => u.Id == tarefa.UsuarioId).FirstOrDefaultAsync();
    if (usuario == null)
        return TypedResults.Problem($"O usuario de id \"{tarefa.UsuarioId}\" nao foi encontrado.");


    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/tarefas/{tarefa.Id}", tarefaDTO);
}

static async Task<IResult> UpdateTarefa(Guid id, TarefaPutDTO inputTarefa, GerenciadorTarefasDb db)
{
    var tarefa = await db.Tarefas.FindAsync(id);

    if (tarefa is null) return TypedResults.NotFound();

    tarefa.Titulo = inputTarefa.Titulo;
    tarefa.Descricao = inputTarefa.Descricao;
    tarefa.Status = ((int)inputTarefa.Status);
    tarefa.Vencimento = inputTarefa.Vencimento;
    tarefa.UsuarioId = inputTarefa.UsuarioId;

    //valida tarefa
    if(string.IsNullOrWhiteSpace(tarefa.Descricao))
        return TypedResults.Problem($"O Campo {nameof(tarefa.Descricao)} deve ser preenchido.");

    if(string.IsNullOrWhiteSpace(tarefa.Titulo))
        return TypedResults.Problem($"O Campo {nameof(tarefa.Titulo)} deve ser preenchido.");

    //valida usuario tarefa
    if(string.IsNullOrWhiteSpace(tarefa.UsuarioId.ToString()))
        return TypedResults.Problem($"O Campo {nameof(tarefa.UsuarioId)} deve ser preenchido.");

    var usuario = await db.Tarefas.Where(t => t.UsuarioId == tarefa.UsuarioId).ToListAsync();
    if (!usuario.Any())
        return TypedResults.Problem($"O usuario de id \"{tarefa.UsuarioId}\" nao foi encontrado.");

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