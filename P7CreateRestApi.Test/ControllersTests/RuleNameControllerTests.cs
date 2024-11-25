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
    public class RuleNameControllerTests
    {
        private readonly Mock<IRuleRepository> _mockRuleRepository;
        private readonly Mock<ILogger<RuleNameController>> _mockLogger;
        private readonly RuleNameController _controller;

        public RuleNameControllerTests()
        {
            _mockRuleRepository = new Mock<IRuleRepository>();
            _mockLogger = new Mock<ILogger<RuleNameController>>();
            _controller = new RuleNameController(_mockRuleRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllRuleNames_ReturnsOkResult_WithRuleNames()
        {
            // Arrange
            var ruleNames = new List<RuleName>
            {
                new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" },
                new RuleName { Id = 2, Name = "Rule2", Description = "Description2", Json = "Json2", Template = "Template2", SqlStr = "SqlStr2", SqlPart = "SqlPart2" }
            };
            _mockRuleRepository.Setup(repo => repo.GetAllRuleNamesAsync()).ReturnsAsync(ruleNames);

            // Act
            var result = await _controller.GetAllRuleNames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<RuleName>>(okResult.Value);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task GetAllRuleNames_ReturnsOkResult_WithEmptyList()
        {
            // Arrange
            _mockRuleRepository.Setup(repo => repo.GetAllRuleNamesAsync()).ReturnsAsync(new List<RuleName>());

            // Act
            var result = await _controller.GetAllRuleNames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<RuleName>>(okResult.Value);
            Assert.Empty(model);
        }

        [Fact]
        public async Task GetAllRuleNames_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRuleRepository.Setup(repo => repo.GetAllRuleNamesAsync()).Throws(new Exception());

            // Act
            var result = await _controller.GetAllRuleNames();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetRuleNameById_ReturnsOkResult_WithRuleName()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _mockRuleRepository.Setup(repo => repo.GetRuleNameByIdAsync(1)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<RuleName>(okResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task GetRuleNameById_ReturnsNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            _mockRuleRepository.Setup(repo => repo.GetRuleNameByIdAsync(1)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetRuleNameById_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRuleRepository.Setup(repo => repo.GetRuleNameByIdAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task CreateRuleName_ReturnsCreatedAtActionResult_WithValidRuleName()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _mockRuleRepository.Setup(repo => repo.CreateRuleNameAsync(ruleName)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var model = Assert.IsType<RuleName>(createdAtActionResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task CreateRuleName_ReturnsBadRequest_WithNullRuleName()
        {
            // Act
            var result = await _controller.CreateRuleName(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet RuleName nul", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateRuleName_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task CreateRuleName_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _mockRuleRepository.Setup(repo => repo.CreateRuleNameAsync(ruleName)).Throws(new Exception());

            // Act
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateRuleName_ReturnsOkResult_WithValidRuleName()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _mockRuleRepository.Setup(repo => repo.UpdateRuleNameAsync(ruleName)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<RuleName>(okResult.Value);
            Assert.Equal(1, model.Id);
        }

        [Fact]
        public async Task UpdateRuleName_ReturnsBadRequest_WithNullRuleName()
        {
            // Act
            var result = await _controller.UpdateRuleName(1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet RuleName nul ou ID de RuleName invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateRuleName_ReturnsBadRequest_WithIdMismatch()
        {
            // Arrange
            var ruleName = new RuleName { Id = 2, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Objet RuleName nul ou ID de RuleName invalide", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateRuleName_ReturnsBadRequest_WithInvalidModelState()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateRuleName_ReturnsNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _mockRuleRepository.Setup(repo => repo.UpdateRuleNameAsync(ruleName)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateRuleName_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1", Json = "Json1", Template = "Template1", SqlStr = "SqlStr1", SqlPart = "SqlPart1" };
            _mockRuleRepository.Setup(repo => repo.UpdateRuleNameAsync(ruleName)).Throws(new Exception());

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task DeleteRuleName_ReturnsNoContent_WithValidId()
        {
            // Arrange
            _mockRuleRepository.Setup(repo => repo.DeleteRuleNameAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRuleName(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteRuleName_ReturnsNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            _mockRuleRepository.Setup(repo => repo.DeleteRuleNameAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRuleName(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteRuleName_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRuleRepository.Setup(repo => repo.DeleteRuleNameAsync(1)).Throws(new Exception());

            // Act
            var result = await _controller.DeleteRuleName(1);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
    }
}