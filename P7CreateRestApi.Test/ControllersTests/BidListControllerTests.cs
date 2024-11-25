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
    public class BidListControllerTests
    {
        private readonly Mock<IBidRepository> _mockBidRepository;
        private readonly Mock<ILogger<BidListController>> _mockLogger;
        private readonly BidListController _controller;

        public BidListControllerTests()
        {
            _mockBidRepository = new Mock<IBidRepository>();
            _mockLogger = new Mock<ILogger<BidListController>>();
            _controller = new BidListController(_mockBidRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllBids_ReturnsOkResult_WithBids()
        {
            // Arrange
            var bids = new List<BidList>
        {
            new BidList { BidListId = 1, Account = "Account1" },
            new BidList { BidListId = 2, Account = "Account2" }
        };
            _mockBidRepository.Setup(repo => repo.GetAllBidsAsync()).ReturnsAsync(bids);

            // Act
            var result = await _controller.GetAllBids();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BidList>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetAllBids_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            _mockBidRepository.Setup(repo => repo.GetAllBidsAsync()).ReturnsAsync(new List<BidList>());

            // Act
            var result = await _controller.GetAllBids();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<BidList>>(okResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task GetAllBids_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockBidRepository.Setup(repo => repo.GetAllBidsAsync()).Throws(new Exception());

            // Act
            var result = await _controller.GetAllBids();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetBidById_ReturnsOkResult_WithBid()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _mockBidRepository.Setup(repo => repo.GetBidByIdAsync(1)).ReturnsAsync(bid);

            // Act
            var result = await _controller.GetBidById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<BidList>(okResult.Value);
            Assert.Equal(1, model.BidListId);
        }

        [Fact]
        public async Task GetBidById_ReturnsNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            _mockBidRepository.Setup(repo => repo.GetBidByIdAsync(1)).ReturnsAsync((BidList)null);

            // Act
            var result = await _controller.GetBidById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetBidById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockBidRepository.Setup(repo => repo.GetBidByIdAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.GetBidById(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateBid_ReturnsCreatedAtActionResult_WithValidBid()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _mockBidRepository.Setup(repo => repo.CreateBidAsync(bid)).ReturnsAsync(bid);

            // Act
            var result = await _controller.CreateBid(bid);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsType<BidList>(createdAtActionResult.Value);
            Assert.Equal(1, model.BidListId);
        }

        [Fact]
        public async Task CreateBid_ReturnsBadRequest_WithNullBid()
        {
            // Act
            var result = await _controller.CreateBid(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Bid nul", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateBid_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _controller.ModelState.AddModelError("Account", "Required");

            // Act
            var result = await _controller.CreateBid(bid);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateBid_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _mockBidRepository.Setup(repo => repo.CreateBidAsync(bid)).Throws(new Exception());

            // Act
            var result = await _controller.CreateBid(bid);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateBid_ReturnsOkResult_WithValidBid()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _mockBidRepository.Setup(repo => repo.UpdateBidAsync(bid)).ReturnsAsync(bid);

            // Act
            var result = await _controller.UpdateBid(1, bid);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<BidList>(okResult.Value);
            Assert.Equal(1, model.BidListId);
        }

        [Fact]
        public async Task UpdateBid_ReturnsBadRequest_WithNullBid()
        {
            // Act
            var result = await _controller.UpdateBid(1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Bid nul ou ID de Bid invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateBid_ReturnsBadRequest_WithIdMismatch()
        {
            // Arrange
            var bid = new BidList { BidListId = 2, Account = "Account1" };

            // Act
            var result = await _controller.UpdateBid(1, bid);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Bid nul ou ID de Bid invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateBid_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _controller.ModelState.AddModelError("Account", "Required");

            // Act
            var result = await _controller.UpdateBid(1, bid);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateBid_ReturnsNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _mockBidRepository.Setup(repo => repo.UpdateBidAsync(bid)).ReturnsAsync((BidList)null);

            // Act
            var result = await _controller.UpdateBid(1, bid);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateBid_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1" };
            _mockBidRepository.Setup(repo => repo.UpdateBidAsync(bid)).Throws(new Exception());

            // Act
            var result = await _controller.UpdateBid(1, bid);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteBid_ReturnsNoContent_WithValidId()
        {
            // Arrange
            _mockBidRepository.Setup(repo => repo.DeleteBidAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBid(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBid_ReturnsNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            _mockBidRepository.Setup(repo => repo.DeleteBidAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBid(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteBid_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockBidRepository.Setup(repo => repo.DeleteBidAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.DeleteBid(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}