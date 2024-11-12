using Moq;
using Microsoft.Extensions.Logging;
using P7CreateRestApi.Controllers;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace P7CreateRestApi.Tests
{
    public class RuleNameControllerTests
    {
        private readonly Mock<IRuleRepository> _ruleNameRepositoryMock;
        private readonly Mock<ILogger<RuleNameController>> _loggerMock;
        private readonly RuleNameController _controller;

        public RuleNameControllerTests()
        {
            _ruleNameRepositoryMock = new Mock<IRuleRepository>();
            _loggerMock = new Mock<ILogger<RuleNameController>>();
            _controller = new RuleNameController(_ruleNameRepositoryMock.Object, _loggerMock.Object);
        }

        // Test pour GetAllRuleNames - Cas où des RuleNames existent
        [Fact]
        public async Task GetAllRuleNames_ShouldReturnOk_WhenRuleNamesExist()
        {
            // Arrange
            var ruleNames = new List<RuleName>
            {
                new RuleName { Id = 1, Name = "Rule1", Description = "Description1" },
                new RuleName { Id = 2, Name = "Rule2", Description = "Description2" }
            };
            _ruleNameRepositoryMock.Setup(r => r.GetAllRuleNamesAsync()).ReturnsAsync(ruleNames);

            // Act
            var result = await _controller.GetAllRuleNames();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedRuleNames = Assert.IsType<List<RuleName>>(actionResult.Value);
            Assert.Equal(ruleNames.Count, returnedRuleNames.Count);
        }

        // Test pour GetAllRuleNames - Cas où aucun RuleName n'existe
        [Fact]
        public async Task GetAllRuleNames_ShouldReturnOk_WhenNoRuleNamesExist()
        {
            // Arrange
            _ruleNameRepositoryMock.Setup(r => r.GetAllRuleNamesAsync()).ReturnsAsync(new List<RuleName>());

            // Act
            var result = await _controller.GetAllRuleNames();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedRuleNames = Assert.IsType<List<RuleName>>(actionResult.Value);
            Assert.Empty(returnedRuleNames);
        }

        // Test pour GetRuleNameById - Cas de succès
        [Fact]
        public async Task GetRuleNameById_ShouldReturnOk_WhenRuleNameExists()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1" };
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(ruleName, actionResult.Value);
        }

        // Test pour GetRuleNameById - Cas de RuleName non trouvé
        [Fact]
        public async Task GetRuleNameById_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.GetRuleNameById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour CreateRuleName - Cas de succès
        [Fact]
        public async Task CreateRuleName_ShouldReturnCreatedAtAction_WhenRuleNameIsValid()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1" };
            _ruleNameRepositoryMock.Setup(r => r.CreateRuleNameAsync(ruleName)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetRuleNameById", actionResult.ActionName);
            Assert.Equal(ruleName.Id, actionResult.RouteValues["id"]);
            Assert.Equal(ruleName, actionResult.Value);
        }

        // Test pour CreateRuleName - Cas de données invalides
        [Fact]
        public async Task CreateRuleName_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var ruleName = new RuleName();
            _controller.ModelState.AddModelError("Name", "Name is required.");

            // Act
            var result = await _controller.CreateRuleName(ruleName);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(actionResult.Value);
            Assert.True(modelState.ContainsKey("Name"));
        }

        // Test pour UpdateRuleName - Cas de succès
        [Fact]
        public async Task UpdateRuleName_ShouldReturnOk_WhenRuleNameExistsAndIsUpdated()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1" };
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync(ruleName);
            _ruleNameRepositoryMock.Setup(r => r.UpdateRuleNameAsync(ruleName)).ReturnsAsync(ruleName);

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(ruleName, actionResult.Value);
        }

        // Test pour UpdateRuleName - Cas de RuleName non trouvé
        [Fact]
        public async Task UpdateRuleName_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            var ruleName = new RuleName { Id = 1, Name = "Rule1", Description = "Description1" };
            _ruleNameRepositoryMock.Setup(r => r.GetRuleNameByIdAsync(1)).ReturnsAsync((RuleName)null);

            // Act
            var result = await _controller.UpdateRuleName(1, ruleName);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Test pour DeleteRuleName - Cas de succès
        [Fact]
        public async Task DeleteRuleName_ShouldReturnNoContent_WhenRuleNameExists()
        {
            // Arrange
            _ruleNameRepositoryMock.Setup(r => r.DeleteRuleNameAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteRuleName(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        // Test pour DeleteRuleName - Cas de RuleName non trouvé
        [Fact]
        public async Task DeleteRuleName_ShouldReturnNotFound_WhenRuleNameDoesNotExist()
        {
            // Arrange
            _ruleNameRepositoryMock.Setup(r => r.DeleteRuleNameAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteRuleName(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
