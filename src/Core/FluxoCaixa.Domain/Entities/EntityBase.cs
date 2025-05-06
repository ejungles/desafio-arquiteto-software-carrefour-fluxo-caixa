using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluxoCaixa.Domain.Entities
{
    /// <summary>
    /// Entidade base com propriedades comuns
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Data de criação do registro
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
