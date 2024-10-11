using System;

namespace GerenciadorDeTarefasApi.DTOs;

public class TarefaPutDTO
{
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Vencimento { get; set; }    
    public Guid UsuarioId { get; set; }
    public Enums.Status Status { get; set; }
    public string Comentario { get; set; }
}
