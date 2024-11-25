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
    public class RatingControllerTests
    {
        private readonly Mock<IRatingRepository> _mockRatingRepository;
        private readonly Mock<ILogger<RatingController>> _mockLogger;
        private readonly RatingController _controller;

        public RatingControllerTests()
        {
            _mockRatingRepository = new Mock<IRatingRepository>();
            _mockLogger = new Mock<ILogger<RatingController>>();
            _controller = new RatingController(_mockRatingRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllRatings_ReturnsOkResult_WithRatings()
        {
            // Arrange
            var ratings = new List<Rating>
            {
                new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" },
                new Rating { Id = 2, MoodysRating = "A2", SandPRating = "BB", FitchRating = "BBB" }
            };
            _mockRatingRepository.Setup(repo => repo.GetAllRatingsAsync()).ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetAllRatings();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Rating>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetAllRatings_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            _mockRatingRepository.Setup(repo => repo.GetAllRatingsAsync()).ReturnsAsync(new List<Rating>());

            // Act
            var result = await _controller.GetAllRatings();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Rating>>(okResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task GetAllRatings_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRatingRepository.Setup(repo => repo.GetAllRatingsAsync()).Throws(new Exception());

            // Act
            var result = await _controller.GetAllRatings();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetRatingById_ReturnsOkResult_WithRating()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _mockRatingRepository.Setup(repo => repo.GetRatingByIdAsync(1)).ReturnsAsync(rating);

            // Act
            var result = await _controller.GetRatingById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Rating>(okResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task GetRatingById_ReturnsNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            _mockRatingRepository.Setup(repo => repo.GetRatingByIdAsync(1)).ReturnsAsync((Rating)null);

            // Act
            var result = await _controller.GetRatingById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetRatingById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRatingRepository.Setup(repo => repo.GetRatingByIdAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.GetRatingById(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateRating_ReturnsCreatedAtActionResult_WithValidRating()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _mockRatingRepository.Setup(repo => repo.CreateRatingAsync(rating)).ReturnsAsync(rating);

            // Act
            var result = await _controller.CreateRating(rating);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsType<Rating>(createdAtActionResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task CreateRating_ReturnsBadRequest_WithNullRating()
        {
            // Act
            var result = await _controller.CreateRating(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Rating nul", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateRating_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _controller.ModelState.AddModelError("MoodysRating", "Required");

            // Act
            var result = await _controller.CreateRating(rating);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateRating_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _mockRatingRepository.Setup(repo => repo.CreateRatingAsync(rating)).Throws(new Exception());

            // Act
            var result = await _controller.CreateRating(rating);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateRating_ReturnsOkResult_WithValidRating()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _mockRatingRepository.Setup(repo => repo.UpdateRatingAsync(rating)).ReturnsAsync(rating);

            // Act
            var result = await _controller.UpdateRating(1, rating);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<Rating>(okResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task UpdateRating_ReturnsBadRequest_WithNullRating()
        {
            // Act
            var result = await _controller.UpdateRating(1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Rating nul ou ID de Rating invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateRating_ReturnsBadRequest_WithIdMismatch()
        {
            // Arrange
            var rating = new Rating { Id = 2, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };

            // Act
            var result = await _controller.UpdateRating(1, rating);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet Rating nul ou ID de Rating invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateRating_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _controller.ModelState.AddModelError("MoodysRating", "Required");

            // Act
            var result = await _controller.UpdateRating(1, rating);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateRating_ReturnsNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _mockRatingRepository.Setup(repo => repo.UpdateRatingAsync(rating)).ReturnsAsync((Rating)null);

            // Act
            var result = await _controller.UpdateRating(1, rating);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateRating_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "A1", SandPRating = "AA", FitchRating = "AAA" };
            _mockRatingRepository.Setup(repo => repo.UpdateRatingAsync(rating)).Throws(new Exception());

            // Act
            var result = await _controller.UpdateRating(1, rating);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteRating_ReturnsNoContent_WithValidId()
        {
            // Arrange
            _mockRatingRepository.Setup(repo => repo.DeleteRatingAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRating(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteRating_ReturnsNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            _mockRatingRepository.Setup(repo => repo.DeleteRatingAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRating(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteRating_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRatingRepository.Setup(repo => repo.DeleteRatingAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.DeleteRating(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}