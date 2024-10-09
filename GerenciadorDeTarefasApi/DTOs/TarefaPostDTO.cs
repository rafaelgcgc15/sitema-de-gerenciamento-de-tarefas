using System;

namespace GerenciadorDeTarefasApi.DTOs;

public class TarefaPostDTO
{ 
    public string Titulo { get; set; }
    public string Descricao { get; set; }
    public DateTime Vencimento { get; set; }    
    public int Status { get; set; }
    public int Prioridade { get; set; }
    public Guid ProjetoId { get; set; }    
}
