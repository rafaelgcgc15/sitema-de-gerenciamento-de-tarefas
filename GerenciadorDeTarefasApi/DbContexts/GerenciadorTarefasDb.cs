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

        modelBuilder.Entity<Tarefa>(
            entity =>
            {
                entity.HasOne(t => t.Projeto)
                .WithMany(p => p.Tarefas)
                .HasForeignKey(f => f.ProjetoId);
            }
        );
        
        base.OnModelCreating(modelBuilder);
    }


}
