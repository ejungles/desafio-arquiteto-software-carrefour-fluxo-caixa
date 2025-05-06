using MongoDB.Bson.Serialization.Attributes;

namespace FluxoCaixa.Domain.Entities
{
    /// <summary>
    /// Saldo consolidado diário (MongoDB)
    /// </summary>
    [BsonIgnoreExtraElements]
    public class SaldoConsolidado
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Data { get; set; }
        public decimal TotalCreditos { get; set; }
        public decimal TotalDebitos { get; set; }
        public decimal Saldo { get; set; }
    }
}
