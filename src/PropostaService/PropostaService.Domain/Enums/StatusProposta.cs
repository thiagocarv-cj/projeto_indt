using System.ComponentModel;

namespace PropostaService.Domain.Enums;

/// <summary>Status do ciclo de vida de uma proposta de seguro.</summary>
public enum StatusProposta
{
    /// <summary>Proposta recebida e aguardando análise (status inicial após criação).</summary>
    [Description("Aguardando análise — status inicial")]
    EmAnalise,

    /// <summary>Proposta aprovada; dispara evento para o serviço de contratação.</summary>
    [Description("Aprovada — gera contratação assíncrona")]
    Aprovada,

    /// <summary>Proposta rejeitada definitivamente.</summary>
    [Description("Rejeitada definitivamente")]
    Rejeitada,

    /// <summary>Proposta com pendências documentais ou cadastrais.</summary>
    [Description("Com pendências — observação obrigatória (mín. 10 caracteres)")]
    Pendencias
}
