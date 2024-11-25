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
    public class CurveControllerTests
    {
        private readonly Mock<ICurvePointRepository> _mockCurvePointRepository;
        private readonly Mock<ILogger<CurveController>> _mockLogger;
        private readonly CurveController _controller;

        public CurveControllerTests()
        {
            _mockCurvePointRepository = new Mock<ICurvePointRepository>();
            _mockLogger = new Mock<ILogger<CurveController>>();
            _controller = new CurveController(_mockCurvePointRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllCurvePoints_ReturnsOkResult_WithCurvePoints()
        {
            // Arrange
            var curvePoints = new List<CurvePoint>
            {
                new CurvePoint { Id = 1, CurveId = 10 },
                new CurvePoint { Id = 2, CurveId = 20 }
            };
            _mockCurvePointRepository.Setup(repo => repo.GetAllCurvePointsAsync()).ReturnsAsync(curvePoints);

            // Act
            var result = await _controller.GetAllCurvePoints();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CurvePoint>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetAllCurvePoints_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            _mockCurvePointRepository.Setup(repo => repo.GetAllCurvePointsAsync()).ReturnsAsync(new List<CurvePoint>());

            // Act
            var result = await _controller.GetAllCurvePoints();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<CurvePoint>>(okResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task GetAllCurvePoints_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockCurvePointRepository.Setup(repo => repo.GetAllCurvePointsAsync()).Throws(new Exception());

            // Act
            var result = await _controller.GetAllCurvePoints();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetCurvePointById_ReturnsOkResult_WithCurvePoint()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _mockCurvePointRepository.Setup(repo => repo.GetCurvePointByIdAsync(1)).ReturnsAsync(curvePoint);

            // Act
            var result = await _controller.GetCurvePointById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<CurvePoint>(okResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task GetCurvePointById_ReturnsNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange
            _mockCurvePointRepository.Setup(repo => repo.GetCurvePointByIdAsync(1)).ReturnsAsync((CurvePoint)null);

            // Act
            var result = await _controller.GetCurvePointById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCurvePointById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockCurvePointRepository.Setup(repo => repo.GetCurvePointByIdAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.GetCurvePointById(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateCurvePoint_ReturnsCreatedAtActionResult_WithValidCurvePoint()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _mockCurvePointRepository.Setup(repo => repo.CreateCurvePointAsync(curvePoint)).ReturnsAsync(curvePoint);

            // Act
            var result = await _controller.CreateCurvePoint(curvePoint);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsType<CurvePoint>(createdAtActionResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task CreateCurvePoint_ReturnsBadRequest_WithNullCurvePoint()
        {
            // Act
            var result = await _controller.CreateCurvePoint(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet CurvePoint nul", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCurvePoint_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _controller.ModelState.AddModelError("CurveId", "Required");

            // Act
            var result = await _controller.CreateCurvePoint(curvePoint);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCurvePoint_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _mockCurvePointRepository.Setup(repo => repo.CreateCurvePointAsync(curvePoint)).Throws(new Exception());

            // Act
            var result = await _controller.CreateCurvePoint(curvePoint);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateCurvePoint_ReturnsOkResult_WithValidCurvePoint()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _mockCurvePointRepository.Setup(repo => repo.UpdateCurvePointAsync(curvePoint)).ReturnsAsync(curvePoint);

            // Act
            var result = await _controller.UpdateCurvePoint(1, curvePoint);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<CurvePoint>(okResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task UpdateCurvePoint_ReturnsBadRequest_WithNullCurvePoint()
        {
            // Act
            var result = await _controller.UpdateCurvePoint(1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet CurvePoint nul ou ID de CurvePoint invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCurvePoint_ReturnsBadRequest_WithIdMismatch()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 2, CurveId = 10 };

            // Act
            var result = await _controller.UpdateCurvePoint(1, curvePoint);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet CurvePoint nul ou ID de CurvePoint invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCurvePoint_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _controller.ModelState.AddModelError("CurveId", "Required");

            // Act
            var result = await _controller.UpdateCurvePoint(1, curvePoint);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCurvePoint_ReturnsNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _mockCurvePointRepository.Setup(repo => repo.UpdateCurvePointAsync(curvePoint)).ReturnsAsync((CurvePoint)null);

            // Act
            var result = await _controller.UpdateCurvePoint(1, curvePoint);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateCurvePoint_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, CurveId = 10 };
            _mockCurvePointRepository.Setup(repo => repo.UpdateCurvePointAsync(curvePoint)).Throws(new Exception());

            // Act
            var result = await _controller.UpdateCurvePoint(1, curvePoint);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCurvePoint_ReturnsNoContent_WithValidId()
        {
            // Arrange
            _mockCurvePointRepository.Setup(repo => repo.DeleteCurvePointAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCurvePoint(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCurvePoint_ReturnsNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange
            _mockCurvePointRepository.Setup(repo => repo.DeleteCurvePointAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCurvePoint(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCurvePoint_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockCurvePointRepository.Setup(repo => repo.DeleteCurvePointAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.DeleteCurvePoint(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}