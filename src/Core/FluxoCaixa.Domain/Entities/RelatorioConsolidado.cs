// RelatorioConsolidado.cs
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FluxoCaixa.Domain.Entities
{
    /// <summary>
    /// Representa um relatório consolidado gerado
    /// </summary>
    [BsonIgnoreExtraElements]
    public class RelatorioConsolidado
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DataInicio { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DataFim { get; set; }

        public string Formato { get; set; }

        /// <summary>
        /// Total de créditos no período
        /// </summary>
        /// <example>15000.00</example>
        public decimal TotalCreditos { get; set; }

        /// <summary>
        /// Total de débitos no período
        /// </summary>
        /// <example>7500.50</example>
        public decimal TotalDebitos { get; set; }

        /// <summary>
        /// Saldo líquido do período
        /// </summary>
        /// <example>7499.50</example>
        public decimal SaldoPeriodo { get; set; }

        /// <summary>
        /// Conteúdo do relatório serializado
        /// </summary>
        public string Conteudo { get; set; } // Alterado para string
    }
}