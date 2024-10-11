using System;

namespace GerenciadorDeTarefasApi.Models;

public class Tarefa
{
    public Guid Id { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime DataCasdastro { get; set; }
    public DateTime Vencimento { get; set; }    
    public int Status { get; set; }
    public int Prioridade { get; set; }
    public Guid ProjetoId { get; set; }
    public Projeto Projeto { get; set; }
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
}
