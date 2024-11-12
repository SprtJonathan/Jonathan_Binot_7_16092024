using Moq;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace P7CreateRestApi.Tests
{
    public class BidListControllerTests
    {
        private readonly Mock<IBidRepository> _bidRepositoryMock;
        private readonly Mock<ILogger<BidListController>> _loggerMock;
        private readonly BidListController _controller;

        public BidListControllerTests()
        {
            _bidRepositoryMock = new Mock<IBidRepository>();
            _loggerMock = new Mock<ILogger<BidListController>>();
            _controller = new BidListController(_bidRepositoryMock.Object, _loggerMock.Object);

            // Simuler un utilisateur authentifié
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.ControllerContext.HttpContext.User = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("id", "1")
                })
            );
        }

        // Test pour CreateBid - Cas de succès
        [Fact]
        public async Task CreateBid_ShouldReturnCreatedAtActionResult_WhenBidIsValid()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" };
            _bidRepositoryMock.Setup(r => r.CreateBidAsync(bid)).ReturnsAsync(bid);

            // Act
            var result = await _controller.CreateBid(bid);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetBidById", actionResult.ActionName);
            Assert.Equal(bid.BidListId, actionResult.RouteValues["id"]);
            Assert.Equal(bid, actionResult.Value);
        }

        // Test pour CreateBid - Cas de l'objet null
        [Fact]
        public async Task CreateBid_ShouldReturnBadRequest_WhenBidIsNull()
        {
            // Act
            var result = await _controller.CreateBid(null);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Bid nul", actionResult.Value);
        }

        // Test pour CreateBid - Cas de validation invalide
        [Fact]
        public async Task CreateBid_ShouldReturnBadRequest_WhenBidIsInvalid()
        {
            // Arrange
            var bid = new BidList();
            _controller.ModelState.AddModelError("Account", "Account is required.");

            // Act
            var result = await _controller.CreateBid(bid);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);
            Assert.True(modelState.ContainsKey("Account"));
            var errors = modelState["Account"] as string[];
            Assert.Contains("Account is required.", errors);
        }

        // Test pour GetAllBids - Cas où des entités sont trouvées
        [Fact]
        public async Task GetAllBids_ShouldReturnOk_WhenBidsExist()
        {
            // Arrange
            var bids = new List<BidList>
            {
                new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" },
                new BidList { BidListId = 2, Account = "Account2", BidType = "Type2" }
            };
            _bidRepositoryMock.Setup(r => r.GetAllBidsAsync()).ReturnsAsync(bids);

            // Act
            var result = await _controller.GetAllBids();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedBids = Assert.IsType<List<BidList>>(actionResult.Value);
            Assert.Equal(bids.Count, returnedBids.Count);
        }

        // Test pour GetAllBids - Cas où aucune entité n'est trouvée
        [Fact]
        public async Task GetAllBids_ShouldReturnOkWithEmptyList_WhenNoBidsExist()
        {
            // Arrange
            _bidRepositoryMock.Setup(r => r.GetAllBidsAsync()).ReturnsAsync(new List<BidList>());

            // Act
            var result = await _controller.GetAllBids();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedBids = Assert.IsType<List<BidList>>(actionResult.Value);
            Assert.Empty(returnedBids);
        }

        // Test pour GetBidById - Cas de succès
        [Fact]
        public async Task GetBidById_ShouldReturnOkResult_WhenBidExists()
        {
            // Arrange
            var bid = new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" };
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(bid);

            // Act
            var result = await _controller.GetBidById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(bid, actionResult.Value);
        }

        // Test pour GetBidById - Cas de l'objet non trouvé
        [Fact]
        public async Task GetBidById_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync((BidList)null);

            // Act
            var result = await _controller.GetBidById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour UpdateBid - Cas de succès
        [Fact]
        public async Task UpdateBid_ShouldReturnOkResult_WhenBidIsValid()
        {
            // Arrange
            var originalBid = new BidList { BidListId = 1, Account = "Account1", BidType = "Type1" };
            var updatedBid = new BidList { BidListId = 1, Account = "AccountUpdated", BidType = "TypeUpdated" };

            _bidRepositoryMock.Setup(r => r.GetBidByIdAsync(1)).ReturnsAsync(originalBid);
            _bidRepositoryMock.Setup(r => r.UpdateBidAsync(updatedBid)).ReturnsAsync(updatedBid);

            // Act
            var result = await _controller.UpdateBid(1, updatedBid);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedBid, actionResult.Value);
        }

        // Test pour UpdateBid - Cas de validation invalide
        [Fact]
        public async Task UpdateBid_ShouldReturnBadRequest_WhenBidIsInvalid()
        {
            // Arrange
            var bid = new BidList { BidListId = 1 };
            _controller.ModelState.AddModelError("Account", "Account is required.");

            // Act
            var result = await _controller.UpdateBid(bid.BidListId, bid);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);
            Assert.True(modelState.ContainsKey("Account"));
            var errors = modelState["Account"] as string[];
            Assert.Contains("Account is required.", errors);
        }

        // Test pour DeleteBid - Cas de succès
        [Fact]
        public async Task DeleteBid_ShouldReturnNoContent_WhenBidExists()
        {
            // Arrange
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBid(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteBid - Cas de l'objet non trouvé
        [Fact]
        public async Task DeleteBid_ShouldReturnNotFound_WhenBidDoesNotExist()
        {
            // Arrange
            _bidRepositoryMock.Setup(r => r.DeleteBidAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBid(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
