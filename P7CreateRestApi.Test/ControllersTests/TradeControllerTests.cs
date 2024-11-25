using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Xunit;

namespace P7CreateRestApi.Tests.ControllersTests
{
    public class TradeControllerTests
    {
        private readonly Mock<ITradeRepository> _mockTradeRepository;
        private readonly Mock<ILogger<TradeController>> _mockLogger;
        private readonly TradeController _controller;

        public TradeControllerTests()
        {
            _mockTradeRepository = new Mock<ITradeRepository>();
            _mockLogger = new Mock<ILogger<TradeController>>();
            _controller = new TradeController(_mockTradeRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllTrades_ReturnsOkResult_WithTrades()
        {
            // Arrange
            var trades = new List<Trade>
            {
                new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" },
                new Trade { TradeId = 2, Account = "Account2", AccountType = "Type2", BuyQuantity = 200, SellQuantity = 100, BuyPrice = 2000, SellPrice = 1800, TradeDate = DateTime.Now, TradeSecurity = "Security2", TradeStatus = "Status2", Trader = "Trader2", Benchmark = "Benchmark2", Book = "Book2", CreationName = "CreationName2", CreationDate = DateTime.Now, RevisionName = "RevisionName2", RevisionDate = DateTime.Now, DealName = "DealName2", DealType = "DealType2", SourceListId = "SourceListId2", Side = "Side2" }
            };
            _mockTradeRepository.Setup(repo => repo.GetAllTradesAsync()).ReturnsAsync(trades);

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Trade>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetAllTrades_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            _mockTradeRepository.Setup(repo => repo.GetAllTradesAsync()).ReturnsAsync(new List<Trade>());

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Trade>>(okResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task GetAllTrades_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockTradeRepository.Setup(repo => repo.GetAllTradesAsync()).Throws(new Exception());

            // Act
            var result = await _controller.GetAllTrades();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetTradeById_ReturnsOkResult_WithTrade()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _mockTradeRepository.Setup(repo => repo.GetTradeByIdAsync(1)).ReturnsAsync(trade);

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Trade>(okResult.Value);
            Assert.Equal(1, model.TradeId);
        }

        [Fact]
        public async Task GetTradeById_ReturnsNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            _mockTradeRepository.Setup(repo => repo.GetTradeByIdAsync(1)).ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetTradeById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockTradeRepository.Setup(repo => repo.GetTradeByIdAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.GetTradeById(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateTrade_ReturnsCreatedAtActionResult_WithValidTrade()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _mockTradeRepository.Setup(repo => repo.CreateTradeAsync(trade)).ReturnsAsync(trade);

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsType<Trade>(createdAtActionResult.Value);
            Assert.Equal(1, model.TradeId);
        }

        [Fact]
        public async Task CreateTrade_ReturnsBadRequest_WithNullTrade()
        {
            // Act
            var result = await _controller.CreateTrade(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Trade nul", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTrade_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _controller.ModelState.AddModelError("Account", "Required");

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateTrade_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _mockTradeRepository.Setup(repo => repo.CreateTradeAsync(trade)).Throws(new Exception());

            // Act
            var result = await _controller.CreateTrade(trade);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateTrade_ReturnsOkResult_WithValidTrade()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _mockTradeRepository.Setup(repo => repo.UpdateTradeAsync(trade)).ReturnsAsync(trade);

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Trade>(okResult.Value);
            Assert.Equal(1, model.TradeId);
        }

        [Fact]
        public async Task UpdateTrade_ReturnsBadRequest_WithNullTrade()
        {
            // Act
            var result = await _controller.UpdateTrade(1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Trade nul ou ID de Trade invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTrade_ReturnsBadRequest_WithIdMismatch()
        {
            // Arrange
            var trade = new Trade { TradeId = 2, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Trade nul ou ID de Trade invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTrade_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _controller.ModelState.AddModelError("Account", "Required");

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateTrade_ReturnsNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _mockTradeRepository.Setup(repo => repo.UpdateTradeAsync(trade)).ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateTrade_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var trade = new Trade { TradeId = 1, Account = "Account1", AccountType = "Type1", BuyQuantity = 100, SellQuantity = 50, BuyPrice = 1000, SellPrice = 900, TradeDate = DateTime.Now, TradeSecurity = "Security1", TradeStatus = "Status1", Trader = "Trader1", Benchmark = "Benchmark1", Book = "Book1", CreationName = "CreationName1", CreationDate = DateTime.Now, RevisionName = "RevisionName1", RevisionDate = DateTime.Now, DealName = "DealName1", DealType = "DealType1", SourceListId = "SourceListId1", Side = "Side1" };
            _mockTradeRepository.Setup(repo => repo.UpdateTradeAsync(trade)).Throws(new Exception());

            // Act
            var result = await _controller.UpdateTrade(1, trade);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteTrade_ReturnsNoContent_WithValidId()
        {
            // Arrange
            _mockTradeRepository.Setup(repo => repo.DeleteTradeAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTrade(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTrade_ReturnsNotFound_WhenTradeDoesNotExist()
        {
            // Arrange
            _mockTradeRepository.Setup(repo => repo.DeleteTradeAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTrade(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTrade_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockTradeRepository.Setup(repo => repo.DeleteTradeAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.DeleteTrade(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}