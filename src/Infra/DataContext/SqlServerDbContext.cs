using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FluxoCaixa.Infra.DataContext
{
    /// <summary>
    /// Contexto de banco de dados para o SQL Server usando Entity Framework Core.
    /// Implementa a interface IUnitOfWork para gerenciamento transacional.
    /// </summary>
    public class SqlServerDbContext : DbContext, IUnitOfWork
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

        public DbSet<Lancamento> Lancamentos { get; set; }
        public DbSet<TipoLancamento> TiposLancamento { get; set; }
        public DbSet<ProcessamentoConsolidacao> ProcessamentoConsolidacao { get; set; }

        public IDbContextTransaction CurrentTransaction => Database.CurrentTransaction;

        public IExecutionStrategy CreateExecutionStrategy() => Database.CreateExecutionStrategy();

        // Implementação da IUnitOfWork
        public async Task BeginTransactionAsync() => await Database.BeginTransactionAsync();

        public async Task CommitAsync() => await Database.CommitTransactionAsync();

        public async Task RollbackAsync() => await Database.RollbackTransactionAsync();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureLancamentoEntity(modelBuilder);
            ConfigureTipoLancamentoEntity(modelBuilder);
            ConfigureProcessamentoConsolidacaoEntity(modelBuilder);
        }

        private static void ConfigureLancamentoEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lancamento>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_Lancamentos");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(18,2)") // Precisão e escala definidas
                    .IsRequired();

                entity.Property(e => e.DataLancamento)
                    .HasColumnType("date")
                    .IsRequired();

                entity.HasOne(e => e.TipoLancamento)
                    .WithMany()
                    .HasForeignKey(e => e.TipoLancamentoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Lancamentos_TiposLancamento");

                entity.HasIndex(e => e.DataLancamento)
                    .HasDatabaseName("IX_Lancamentos_DataLancamento");
            });
        }

        private static void ConfigureTipoLancamentoEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoLancamento>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_TiposLancamento");

                entity.Property(e => e.Descricao)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Natureza)
                    .IsRequired()
                    .HasColumnType("char(1)")
                    .HasConversion<string>(); // Garante conversão explícita para string

                entity.HasIndex(e => e.Descricao)
                    .IsUnique()
                    .HasDatabaseName("IX_TiposLancamento_Descricao");
            });
        }

        private static void ConfigureProcessamentoConsolidacaoEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProcessamentoConsolidacao>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_ProcessamentoConsolidacao");

                entity.Property(e => e.DataProcessamento)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasColumnType("varchar(20)")
                    .HasMaxLength(20);

                entity.Property(e => e.ValorTotalCreditos)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.ValorTotalDebitos)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(e => e.SaldoConsolidado)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.HasIndex(e => e.DataProcessamento)
                    .IsUnique()
                    .HasDatabaseName("IX_ProcessamentoConsolidacao_Data");
            });
        }
    }
}