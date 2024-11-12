using Moq;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace P7CreateRestApi.Tests
{
    public class TradeControllerTests
    {
        private readonly Mock<ITradeRepository> _tradeRepositoryMock;
        private readonly Mock<ILogger<TradeController>> _loggerMock;
        private readonly TradeController _controller;

        public TradeControllerTests()
        {
            _tradeRepositoryMock = new Mock<ITradeRepository>();
            _loggerMock = new Mock<ILogger<TradeController>>();
            _controller = new TradeController(_tradeRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour GetAllTrades - Cas où des Trades existent
        [Fact]
        public async Task GetAllTrades_ShouldReturnOk_WhenTradesExist()
        {
            // Arrange
            var trades = new List<Trade>
            {
                new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1" },
                new Trade { TradeId = 2, Account = "Account2", AccountType = "Type2" }
            };
            _tradeRepositoryMock.Setup(r => r.GetAllTradesAsync()).ReturnsAsync(trades);

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedTrades = Assert.IsType<List<Trade>>(actionResult.Value);
            Assert.Equal(trades.Count, returnedTrades.Count);
        }

        // Test pour GetAllTrades - Cas où aucun Trade n'existe
        [Fact]
        public async Task GetAllTrades_ShouldReturnOk_WhenNoTradesExist()
        {
            // Arrange
            _tradeRepositoryMock.Setup(r => r.GetAllTradesAsync()).ReturnsAsync(new List<Trade>());

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedTrades = Assert.IsType<List<Trade>>(actionResult.Value);
            Assert.Empty(returnedTrades);
        }

        // Test pour GetTradeById - Cas de succès
        [Fact]
        public async Task GetTradeById_ShouldReturnOk_WhenTradeExists()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1" };
            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ReturnsAsync(trade);

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(trade, actionResult.Value);
        }

        // Test pour GetTradeById - Cas de Trade non trouvé
        [Fact]
        public async Task GetTradeById_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour CreateTrade - Cas de succès
        [Fact]
        public async Task CreateTrade_ShouldReturnCreatedAtAction_WhenTradeIsValid()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1" };
            _tradeRepositoryMock.Setup(r => r.CreateTradeAsync(trade)).ReturnsAsync(trade);

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetTradeById", actionResult.ActionName);
            Assert.Equal(trade.TradeId, actionResult.RouteValues["id"]);
            Assert.Equal(trade, actionResult.Value);
        }

        // Test pour CreateTrade - Cas de données invalides
        [Fact]
        public async Task CreateTrade_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var trade = new Trade();
            _controller.ModelState.AddModelError("Account", "Account is required.");

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);
            Assert.True(modelState.ContainsKey("Account"));
        }

        // Test pour UpdateTrade - Cas de succès
        [Fact]
        public async Task UpdateTrade_ShouldReturnOk_WhenTradeExistsAndIsUpdated()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1" };
            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ReturnsAsync(trade);
            _tradeRepositoryMock.Setup(r => r.UpdateTradeAsync(trade)).ReturnsAsync(trade);

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(trade, actionResult.Value);
        }

        // Test pour UpdateTrade - Cas de Trade non trouvé
        [Fact]
        public async Task UpdateTrade_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1" };
            _tradeRepositoryMock.Setup(r => r.GetTradeByIdAsync(1)).ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour DeleteTrade - Cas de succès
        [Fact]
        public async Task DeleteTrade_ShouldReturnNoContent_WhenTradeExists()
        {
            // Arrange
            _tradeRepositoryMock.Setup(r => r.DeleteTradeAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTrade(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteTrade - Cas de Trade non trouvé
        [Fact]
        public async Task DeleteTrade_ShouldReturnNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            _tradeRepositoryMock.Setup(r => r.DeleteTradeAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTrade(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
