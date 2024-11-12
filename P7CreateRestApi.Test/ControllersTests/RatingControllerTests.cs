using Moq;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace P7CreateRestApi.Tests
{
    public class RatingControllerTests
    {
        private readonly Mock<IRatingRepository> _ratingRepositoryMock;
        private readonly Mock<ILogger<RatingController>> _loggerMock;
        private readonly RatingController _controller;

        public RatingControllerTests()
        {
            _ratingRepositoryMock = new Mock<IRatingRepository>();
            _loggerMock = new Mock<ILogger<RatingController>>();
            _controller = new RatingController(_ratingRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour GetAllRatings - Cas où des ratings existent
        [Fact]
        public async Task GetAllRatings_ShouldReturnOk_WhenRatingsExist()
        {
            // Arrange
            var ratings = new List<Rating>
            {
                new Rating { Id = 1, MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA" },
                new Rating { Id = 2, MoodysRating = "Baa", SandPRating = "BBB", FitchRating = "BBB" }
            };
            _ratingRepositoryMock.Setup(r => r.GetAllRatingsAsync()).ReturnsAsync(ratings);

            // Act
            var result = await _controller.GetAllRatings();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedRatings = Assert.IsType<List<Rating>>(actionResult.Value);
            Assert.Equal(ratings.Count, returnedRatings.Count);
        }

        // Test pour GetAllRatings - Cas où aucun rating n'existe
        [Fact]
        public async Task GetAllRatings_ShouldReturnOk_WhenNoRatingsExist()
        {
            // Arrange
            _ratingRepositoryMock.Setup(r => r.GetAllRatingsAsync()).ReturnsAsync(new List<Rating>());

            // Act
            var result = await _controller.GetAllRatings();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedRatings = Assert.IsType<List<Rating>>(actionResult.Value);
            Assert.Empty(returnedRatings);
        }

        // Test pour GetRatingById - Cas de succès
        [Fact]
        public async Task GetRatingById_ShouldReturnOk_WhenRatingExists()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA" };
            _ratingRepositoryMock.Setup(r => r.GetRatingByIdAsync(1)).ReturnsAsync(rating);

            // Act
            var result = await _controller.GetRatingById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(rating, actionResult.Value);
        }

        // Test pour GetRatingById - Cas de rating non trouvé
        [Fact]
        public async Task GetRatingById_ShouldReturnNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            _ratingRepositoryMock.Setup(r => r.GetRatingByIdAsync(1)).ReturnsAsync((Rating)null);

            // Act
            var result = await _controller.GetRatingById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour CreateRating - Cas de succès
        [Fact]
        public async Task CreateRating_ShouldReturnCreatedAtAction_WhenRatingIsValid()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA" };
            _ratingRepositoryMock.Setup(r => r.CreateRatingAsync(rating)).ReturnsAsync(rating);

            // Act
            var result = await _controller.CreateRating(rating);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetRatingById", actionResult.ActionName);
            Assert.Equal(rating.Id, actionResult.RouteValues["id"]);
            Assert.Equal(rating, actionResult.Value);
        }

        // Test pour CreateRating - Cas de données invalides
        [Fact]
        public async Task CreateRating_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var rating = new Rating();
            _controller.ModelState.AddModelError("MoodysRating", "MoodysRating is required.");

            // Act
            var result = await _controller.CreateRating(rating);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);
            Assert.True(modelState.ContainsKey("MoodysRating"));
        }

        // Test pour UpdateRating - Cas de succès
        [Fact]
        public async Task UpdateRating_ShouldReturnOk_WhenRatingExistsAndIsUpdated()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA" };
            _ratingRepositoryMock.Setup(r => r.GetRatingByIdAsync(1)).ReturnsAsync(rating);
            _ratingRepositoryMock.Setup(r => r.UpdateRatingAsync(rating)).ReturnsAsync(rating);

            // Act
            var result = await _controller.UpdateRating(1, rating);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(rating, actionResult.Value);
        }

        // Test pour UpdateRating - Cas de rating non trouvé
        [Fact]
        public async Task UpdateRating_ShouldReturnNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            var rating = new Rating { Id = 1, MoodysRating = "Aaa", SandPRating = "AAA", FitchRating = "AAA" };
            _ratingRepositoryMock.Setup(r => r.GetRatingByIdAsync(1)).ReturnsAsync((Rating)null);

            // Act
            var result = await _controller.UpdateRating(1, rating);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour DeleteRating - Cas de succès
        [Fact]
        public async Task DeleteRating_ShouldReturnNoContent_WhenRatingExists()
        {
            // Arrange
            _ratingRepositoryMock.Setup(r => r.DeleteRatingAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRating(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteRating - Cas de rating non trouvé
        [Fact]
        public async Task DeleteRating_ShouldReturnNotFound_WhenRatingDoesNotExist()
        {
            // Arrange
            _ratingRepositoryMock.Setup(r => r.DeleteRatingAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRating(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
