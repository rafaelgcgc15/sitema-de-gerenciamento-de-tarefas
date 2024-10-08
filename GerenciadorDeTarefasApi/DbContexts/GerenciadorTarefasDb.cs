using System;
using GerenciadorDeTarefasApi.Models;
using Microsoft.EntityFrameworkCore;


namespace GerenciadorDeTarefasApi.DbContexts;

public class GerenciadorTarefasDb : DbContext
{
    public GerenciadorTarefasDb(DbContextOptions<GerenciadorTarefasDb> options)
        : base(options) { }

    public DbSet<Projeto> Projetos => Set<Projeto>();
    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<HistoricoTarefa> HistoricoTarefas => Set<HistoricoTarefa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
        .Entity<HistoricoTarefa>( eb => eb.HasNoKey() );

        modelBuilder.Entity<Usuario>().HasData(new Usuario { Id = new Guid("92974a6f-a28e-4337-8d8c-7e326ea3c15a"), Nome = "Rafael da Silva" });

        base.OnModelCreating(modelBuilder);
    }


}
