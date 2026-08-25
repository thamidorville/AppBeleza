namespace BelezaApp.API.DTOs;

public class AgendamentoRequestDTO
{
    public string AgendaId { get; set; } = string.Empty;
}


// O hashId do slot de agenda que o cliente quer agendar. 
// É o único dado que o frontend precisa enviar. 
// Todo resto (quem é o cliente, qual serviço, 
// qual funcionário, que dia/horário) já está no banco

// KISS: De 8 campos pra 1. O frontend não repete dados que o banco já tem.
// Segurança (OWASP): O cliente não pode adulterar servicoId, funcionarioId,
// nem data porque esses dados vêm do banco, não do request.
// Single Source of Truth: Os dados reais estão na tabela Agenda. Não no frontend.