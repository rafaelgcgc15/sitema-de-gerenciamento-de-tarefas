using System;

namespace GerenciadorDeTarefasApi.DTOs;

public class TarefaPostDTO
{ 
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Vencimento { get; set; }    
    public Enums.Status Status { get; set; }
    public Enums.Prioridade Prioridade { get; set; }
    public string Comentario { get; set; }
    public Guid UsuarioId { get; set; }
    public Guid ProjetoId { get; set; }    
}
