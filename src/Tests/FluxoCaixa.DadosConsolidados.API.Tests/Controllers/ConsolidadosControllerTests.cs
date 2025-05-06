using Moq;
using MediatR;
using FluxoCaixa.DadosConsolidados.API.Controllers;
using FluxoCaixa.Shared.DTOs.Consolidados.Responses;
using Microsoft.AspNetCore.Mvc;
using FluxoCaixa.Application.Queries;

public class ConsolidadosControllerTests
{
    [Fact]
    public async Task GetSaldoDiario_ExistingDate_ReturnsOk()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        mockMediator.Setup(m => m.Send(It.IsAny<ObterConsolidadoDiarioQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ConsolidadoDiarioResponse());

        var controller = new ConsolidadosController(mockMediator.Object);

        // Act
        var result = await controller.GetSaldoDiario(DateTime.Today);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}