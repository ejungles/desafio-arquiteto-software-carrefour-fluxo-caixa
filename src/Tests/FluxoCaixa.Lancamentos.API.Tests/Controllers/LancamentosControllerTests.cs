using FluxoCaixa.Application.Interfaces;
using FluxoCaixa.Application.Validations;
using FluxoCaixa.Domain.Interfaces.Repositories;
using FluxoCaixa.Lancamentos.API.Controllers;
using FluxoCaixa.Shared.DTOs.Lancamentos.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace FluxoCaixa.Lancamentos.API.Tests.Controllers
{
    public class LancamentosControllerTests
    {
        // Classe mock simplificada que implementa apenas o necessário
        private class TestExecutionStrategy : IExecutionStrategy
        {
            public bool RetriesOnFailure => false;

            public Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default)
                => operation();

            public Task<TResult> ExecuteAsync<TState, TResult>(
                TState state,
                Func<DbContext, TState, CancellationToken, Task<TResult>> operation,
                Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>> verifySucceeded,
                CancellationToken cancellationToken = default)
            {
                // Implementação simplificada que ignora o DbContext
                return operation(null!, state, cancellationToken);
            }

            // Não necessário para os testes atuais
            public TResult Execute<TState, TResult>(TState state, Func<DbContext, TState, TResult> operation,
                Func<DbContext, TState, ExecutionResult<TResult>> verifySucceeded)
                => throw new NotImplementedException();
        }

        [Fact]
        public async Task CriarLancamento_DeveRetornarCreated()
        {
            // Arrange
            var mockRepo = new Mock<ILancamentoRepository>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockValidator = new Mock<CriarLancamentoValidator>();
            var mockRabbit = new Mock<IRabbitMQPublisher>();

            // Configuração mínima necessária
            mockUow.Setup(u => u.CreateExecutionStrategy())
                .Returns(() => new TestExecutionStrategy());

            mockUow.Setup(u => u.BeginTransactionAsync())
                .Returns(Task.CompletedTask);

            mockUow.Setup(u => u.CommitAsync())
                .Returns(Task.CompletedTask);

            var controller = new LancamentosController(
                mockRepo.Object,
                mockUow.Object,
                mockValidator.Object,
                mockRabbit.Object);

            var request = new CriarLancamentoRequest
            {
                Descricao = "Teste",
                Valor = 100,
                TipoLancamentoId = 1,
                Data = DateTime.Now
            };

            // Act
            var result = await controller.CriarLancamento(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task ObterPorId_QuandoExistir_DeveRetornarOk()
        {
            // Arrange
            var mockRepo = new Mock<ILancamentoRepository>();
            mockRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new Domain.Entities.Lancamento
                {
                    Id = 1,
                    TipoLancamento = new Domain.Entities.TipoLancamento
                    {
                        Id = 1,
                        Descricao = "Teste",
                        Natureza = 'C'
                    }
                });

            var controller = new LancamentosController(
                mockRepo.Object,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<CriarLancamentoValidator>(),
                Mock.Of<IRabbitMQPublisher>());

            // Act
            var result = await controller.ObterPorId(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ObterPorId_QuandoNaoExistir_DeveRetornarNotFound()
        {
            // Arrange
            var mockRepo = new Mock<ILancamentoRepository>();
            mockRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync((Domain.Entities.Lancamento)null);

            var controller = new LancamentosController(
                mockRepo.Object,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<CriarLancamentoValidator>(),
                Mock.Of<IRabbitMQPublisher>());

            // Act
            var result = await controller.ObterPorId(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}