using System;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorDeTarefasApi.Models;

[Keyless]
public class HistoricoTarefa
{
    public Guid TarefaId { get; set; }
    public DateTime DataAlteracao { get; set; }
    public int Versao { get; set; }
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Vencimento { get; set; }    
    public int Status { get; set; }
    public Guid UsuarioId { get; set; }    
}
