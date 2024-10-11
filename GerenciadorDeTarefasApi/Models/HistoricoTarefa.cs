using System;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeTarefasApi.Models;

public class HistoricoTarefa
{
    public int Id { get; set; }
    public Guid TarefaId { get; set; }
    public DateTime DataAlteracao { get; set; }
    public int Versao { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Vencimento { get; set; }    
    public int Status { get; set; }

    public string Comentario { get; set; }
    public Guid ProjetoId { get; set; }
    public int Prioridade { get; set; }
    public Guid UsuarioId { get; set; }    
}
