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
var historicoTarefas = app.MapGroup("historicoTarefas");
var relatorioDesempenho = app.MapGroup("relatorioDesempenho");

configuracaoDadosBasicoTeste.MapPost("/", ConfiguraDadosBasicos);
usuarios.MapGet("/", GetAllUsuarios);
projetos.MapGet("/", GetAllProjetos);
projetos.MapPost("/", CreateProjeto);
tarefas.MapGet("/{projetoId}", GetTarefasProjeto);
tarefas.MapPost("/", CreateTarefa);
tarefas.MapPut("/{id}", UpdateTarefa);
tarefas.MapDelete("/{id}", DeleteTarefa);
historicoTarefas.MapGet("/{tarefaId}", GetHistoricoTarefa);
relatorioDesempenho.MapGet("/{usuarioId}", GetRelatorioDesempenho);

//Configura dados basicos para teste.
static async Task<IResult> ConfiguraDadosBasicos(GerenciadorTarefasDb db)
{
    Usuario usuario = new Usuario { Id = new Guid("92974a6f-a28e-4337-8d8c-7e326ea3c15a"), Nome = "Rafael da Silva", PerfilId = 1 };
    Usuario usuario2 = new Usuario { Id = new Guid("987da092-ba8c-422a-8497-d8c2b8340f5d"), Nome = "Luis da Silva", PerfilId = 2 };
    Usuario usuario3 = new Usuario { Id = new Guid("810a4a18-4226-4996-8a5e-5e07a1b6d855"), Nome = "Michel da Silva", PerfilId = 3 };
    Usuario usuario4 = new Usuario { Id = new Guid("408259bf-acdd-4bfa-92c2-cd3be7f846f7"), Nome = "Diogo da Silva", PerfilId = 3 };
    Usuario usuario5 = new Usuario { Id = new Guid("87fd580a-addc-4ade-a9c8-af53f41a5e0f"), Nome = "Guido da Silva", PerfilId = 3 };

    Projeto projeto = new Projeto { Id = new Guid("655a71a1-c983-4eb2-95e1-4f08c6bc1498"), Nome = "Projeto de Gerenciamento de Tarefas V1", Descricao = "Esse projeto consiste em criar uma api para gerenciamento de tarefas." };

    db.Usuarios.Add(usuario);
    db.Projetos.Add(projeto);
    await db.SaveChangesAsync();

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
          DataCasdastro = DateTime.Now,
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

    //Inserir Historico de Atualizacao da tarefa
    await insereHistoricoTarefaAsync(db, tarefa, tarefaDTO.Comentario);

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

    if(tarefa.Status < 1 || tarefa.Status > 3)
        return TypedResults.Problem($"O Campo {nameof(tarefa.Status)} deve ser preenchido com valor de 1 a 3. Onde 1 e status pendente, 2 em andamento e 3 concluida.");
    
    //valida usuario tarefa
    if(string.IsNullOrWhiteSpace(tarefa.UsuarioId.ToString()))
        return TypedResults.Problem($"O Campo {nameof(tarefa.UsuarioId)} deve ser preenchido.");

    var usuario = await db.Tarefas.Where(t => t.UsuarioId == tarefa.UsuarioId).ToListAsync();
    if (!usuario.Any())
        return TypedResults.Problem($"O usuario de id \"{tarefa.UsuarioId}\" nao foi encontrado.");

    await db.SaveChangesAsync();
    
    //Inserir Historico de Atualizacao da tarefa
    await insereHistoricoTarefaAsync(db, tarefa, inputTarefa.Comentario);

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

static async Task insereHistoricoTarefaAsync(GerenciadorTarefasDb db, Tarefa tarefa, string comentario)
{
    var versao = db.HistoricoTarefas.Where(w => w.TarefaId == tarefa.Id).Select(s => s.Versao).DefaultIfEmpty().Max();

    HistoricoTarefa historicoTarefa = new HistoricoTarefa
    {
         DataAlteracao = DateTime.Now,
         Status = tarefa.Status,
         Prioridade = tarefa.Prioridade,
         Descricao = tarefa.Descricao,
         TarefaId = tarefa.Id,
         Titulo = tarefa.Titulo,
         UsuarioId = tarefa.UsuarioId,
         ProjetoId = tarefa.ProjetoId,
         Versao = (versao + 1),
         Comentario = comentario
    };

    db.HistoricoTarefas.Add(historicoTarefa);
    await db.SaveChangesAsync();
}

//Historico Tarefas
static async Task<IResult> GetHistoricoTarefa(Guid tarefaId, GerenciadorTarefasDb db)
{
    return TypedResults.Ok(await db.HistoricoTarefas.Where(t => t.TarefaId == tarefaId).OrderBy(o => o.Versao).ToListAsync());
}

static async Task<IResult> GetRelatorioDesempenho(Guid usuarioId, GerenciadorTarefasDb db)
{
    /*- A API deve fornecer endpoints para gerar relatórios de desempenho, como o número médio de tarefas concluídas por usuário nos últimos 30 dias.
    - Os relatórios devem ser acessíveis apenas por usuários com uma função específica de "gerente".*/
    var quantidadeUsuarios = await db.Usuarios.CountAsync();
    var quantidadeTarefasAtendidas = await db.Tarefas.Where(w => w.Status == 3 && w.DataCasdastro >= DateTime.Now.AddDays(-30)).CountAsync();

    var mediaTarefasConcluidasUsuario = (quantidadeTarefasAtendidas / quantidadeUsuarios);

    return TypedResults.Ok(mediaTarefasConcluidasUsuario);

}

app.Run();