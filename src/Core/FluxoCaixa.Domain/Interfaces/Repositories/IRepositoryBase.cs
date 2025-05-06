namespace FluxoCaixa.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface base para operações genéricas de repositório
    /// </summary>
    /// <typeparam name="T">Tipo da entidade gerenciada pelo repositório</typeparam>
    public interface IRepositoryBase<T> where T : class
    {
        /// <summary>
        /// Obtém uma entidade pelo seu identificador único
        /// </summary>
        /// <param name="id">Identificador único da entidade</param>
        /// <returns>Entidade correspondente ou null se não encontrada</returns>
        Task<T> GetByIdAsync(long id);

        /// <summary>
        /// Lista todas as entidades com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (iniciando em 1)</param>
        /// <param name="pageSize">Quantidade de itens por página</param>
        /// <returns>Coleção de entidades paginadas</returns>
        Task<IEnumerable<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Adiciona uma nova entidade ao repositório
        /// </summary>
        /// <param name="entity">Entidade a ser persistida</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Atualiza uma entidade existente
        /// </summary>
        /// <param name="entity">Entidade com dados atualizados</param>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Remove uma entidade pelo seu identificador
        /// </summary>
        /// <param name="id">Identificador único da entidade</param>
        Task DeleteAsync(long id);
    }
}
