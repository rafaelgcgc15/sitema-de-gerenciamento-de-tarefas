using System;

namespace GerenciadorDeTarefasApi.Models;

public class Projeto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public IEnumerable<Tarefa> Tarefas { get; set; }
}
