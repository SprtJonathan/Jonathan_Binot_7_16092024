using Moq;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace P7CreateRestApi.Tests
{
    public class CurveControllerTests
    {
        private readonly Mock<ICurvePointRepository> _curvePointRepositoryMock;
        private readonly Mock<ILogger<CurveController>> _loggerMock;
        private readonly CurveController _controller;

        public CurveControllerTests()
        {
            _curvePointRepositoryMock = new Mock<ICurvePointRepository>();
            _loggerMock = new Mock<ILogger<CurveController>>();
            _controller = new CurveController(_curvePointRepositoryMock.Object, _loggerMock.Object);

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

        // Test pour GetAllCurvePoints - Cas où des entités sont trouvées
        [Fact]
        public async Task GetAllCurvePoints_ShouldReturnOk_WhenCurvePointsExist()
        {
            // Arrange
            var curvePoints = new List<CurvePoint>
            {
                new CurvePoint { Id = 1, Term = 1.5 },
                new CurvePoint { Id = 2, Term = 2.0 }
            };
            _curvePointRepositoryMock.Setup(r => r.GetAllCurvePointsAsync()).ReturnsAsync(curvePoints);

            // Act
            var result = await _controller.GetAllCurvePoints();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedCurvePoints = Assert.IsType<List<CurvePoint>>(actionResult.Value);
            Assert.Equal(curvePoints.Count, returnedCurvePoints.Count);
        }

        // Test pour GetAllCurvePoints - Cas où aucune entité n'est trouvée
        [Fact]
        public async Task GetAllCurvePoints_ShouldReturnOkWithEmptyList_WhenNoCurvePointsExist()
        {
            // Arrange
            _curvePointRepositoryMock.Setup(r => r.GetAllCurvePointsAsync()).ReturnsAsync(new List<CurvePoint>());

            // Act
            var result = await _controller.GetAllCurvePoints();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedCurvePoints = Assert.IsType<List<CurvePoint>>(actionResult.Value);
            Assert.Empty(returnedCurvePoints);
        }

        // Test pour GetCurvePointById - Cas de succès
        [Fact]
        public async Task GetCurvePointById_ShouldReturnOkResult_WhenCurvePointExists()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, Term = 1.5 };
            _curvePointRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync(curvePoint);

            // Act
            var result = await _controller.GetCurvePointById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(curvePoint, actionResult.Value);
        }

        // Test pour GetCurvePointById - Cas de l'objet non trouvé
        [Fact]
        public async Task GetCurvePointById_ShouldReturnNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange
            _curvePointRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync((CurvePoint)null);

            // Act
            var result = await _controller.GetCurvePointById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour CreateCurvePoint - Cas de succès
        [Fact]
        public async Task CreateCurvePoint_ShouldReturnCreatedAtActionResult_WhenCurvePointIsValid()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1, Term = 1.5 };
            _curvePointRepositoryMock.Setup(r => r.CreateCurvePointAsync(curvePoint)).ReturnsAsync(curvePoint);

            // Act
            var result = await _controller.CreateCurvePoint(curvePoint);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetCurvePointById", actionResult.ActionName);
            Assert.Equal(curvePoint.Id, actionResult.RouteValues["id"]);
            Assert.Equal(curvePoint, actionResult.Value);
        }

        // Test pour CreateCurvePoint - Cas de l'objet null
        [Fact]
        public async Task CreateCurvePoint_ShouldReturnBadRequest_WhenCurvePointIsNull()
        {
            // Act
            var result = await _controller.CreateCurvePoint(null);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet CurvePoint nul", actionResult.Value);
        }

        // Test pour UpdateCurvePoint - Cas de succès
        [Fact]
        public async Task UpdateCurvePoint_ShouldReturnOkResult_WhenCurvePointIsValid()
        {
            // Arrange
            var originalCurvePoint = new CurvePoint { Id = 1, Term = 1.5 };
            var updatedCurvePoint = new CurvePoint { Id = 1, Term = 2.0 };

            _curvePointRepositoryMock.Setup(r => r.GetCurvePointByIdAsync(1)).ReturnsAsync(originalCurvePoint);
            _curvePointRepositoryMock.Setup(r => r.UpdateCurvePointAsync(updatedCurvePoint)).ReturnsAsync(updatedCurvePoint);

            // Act
            var result = await _controller.UpdateCurvePoint(1, updatedCurvePoint);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(updatedCurvePoint, actionResult.Value);
        }

        // Test pour UpdateCurvePoint - Cas de validation invalide
        [Fact]
        public async Task UpdateCurvePoint_ShouldReturnBadRequest_WhenCurvePointIsInvalid()
        {
            // Arrange
            var curvePoint = new CurvePoint { Id = 1 };
            _controller.ModelState.AddModelError("Term", "Term is required.");

            // Act
            var result = await _controller.UpdateCurvePoint(curvePoint.Id, curvePoint);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);
            Assert.True(modelState.ContainsKey("Term"));
            var errors = modelState["Term"] as string[];
            Assert.Contains("Term is required.", errors);
        }

        // Test pour DeleteCurvePoint - Cas de succès
        [Fact]
        public async Task DeleteCurvePoint_ShouldReturnNoContent_WhenCurvePointExists()
        {
            // Arrange
            _curvePointRepositoryMock.Setup(r => r.DeleteCurvePointAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCurvePoint(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteCurvePoint - Cas de l'objet non trouvé
        [Fact]
        public async Task DeleteCurvePoint_ShouldReturnNotFound_WhenCurvePointDoesNotExist()
        {
            // Arrange
            _curvePointRepositoryMock.Setup(r => r.DeleteCurvePointAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteCurvePoint(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
