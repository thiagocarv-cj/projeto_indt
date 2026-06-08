using System.ComponentModel;

namespace Compartilhado.Contratos.Propostas;

/// <summary>Status de proposta compartilhado entre microserviços.</summary>
public enum StatusPropostaCompartilhado
{
    /// <summary>Aguardando análise.</summary>
    [Description("Aguardando análise — status inicial")]
    EmAnalise,

    /// <summary>Proposta aprovada.</summary>
    [Description("Aprovada — gera contratação assíncrona")]
    Aprovada,

    /// <summary>Proposta rejeitada.</summary>
    [Description("Rejeitada definitivamente")]
    Rejeitada,

    /// <summary>Proposta com pendências.</summary>
    [Description("Com pendências — observação obrigatória (mín. 10 caracteres)")]
    Pendencias
}
