using Microsoft.EntityFrameworkCore;
using Xunit;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P7CreateRestApi.Tests.RepositoriesTests
{
    public class RuleRepositoryTests
    {
        private readonly DbContextOptions<LocalDbContext> _options;

        public RuleRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private RuleName CreateValidRuleName(int id = 1)
        {
            return new RuleName
            {
                Id = id,
                Name = "Sample Rule",
                Description = "This is a sample rule for testing.",
                Json = "{ \"key\": \"value\" }",
                Template = "Sample Template",
                SqlStr = "SELECT * FROM Rules",
                SqlPart = "WHERE Id = @Id"
            };
        }

        [Fact]
        public async Task CreateRuleNameAsync_WithValidRuleName_ShouldAddRuleNameToContext()
        {
            // Arrange
            var ruleName = CreateValidRuleName();
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);

                // Act
                var result = await repository.CreateRuleNameAsync(ruleName);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(ruleName.Id, result.Id);

                var savedRuleName = await context.RuleNames.FindAsync(ruleName.Id);
                Assert.NotNull(savedRuleName);
                Assert.Equal(ruleName.Name, savedRuleName.Name);
            }
        }

        [Fact]
        public async Task CreateRuleNameAsync_ShouldThrowException_WhenRuleNameIsInvalid()
        {
            // Arrange
            var invalidRuleName = new RuleName(); // RuleName sans les champs requis

            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);

                // Act & Assert
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateRuleNameAsync(invalidRuleName));
            }
        }

        [Fact]
        public async Task GetRuleNameByIdAsync_WithExistingId_ShouldReturnRuleName()
        {
            // Arrange
            var ruleName = CreateValidRuleName();
            using (var context = new LocalDbContext(_options))
            {
                context.RuleNames.Add(ruleName);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);
                var result = await repository.GetRuleNameByIdAsync(ruleName.Id);

                Assert.NotNull(result);
                Assert.Equal(ruleName.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetRuleNameByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);
                var result = await repository.GetRuleNameByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllRuleNamesAsync_ShouldReturnAllRuleNames()
        {
            // Arrange
            var ruleNames = new List<RuleName>
            {
                CreateValidRuleName(1),
                CreateValidRuleName(2)
            };

            using (var context = new LocalDbContext(_options))
            {
                await context.RuleNames.AddRangeAsync(ruleNames);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);
                var results = await repository.GetAllRuleNamesAsync();

                var resultList = results.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.Equal(ruleNames[0].Id, resultList[0].Id);
                Assert.Equal(ruleNames[1].Id, resultList[1].Id);
            }
        }

        [Fact]
        public async Task GetAllRuleNamesAsync_ShouldReturnEmptyList_WhenNoRuleNamesExist()
        {
            // Arrange
            using (var context = new LocalDbContext(_options))
            {
                // Aucun ajout de données au contexte
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);
                var results = await repository.GetAllRuleNamesAsync();

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        [Fact]
        public async Task UpdateRuleNameAsync_WithValidRuleName_ShouldUpdateAndReturnRuleName()
        {
            // Arrange
            var ruleName = CreateValidRuleName();
            using (var context = new LocalDbContext(_options))
            {
                context.RuleNames.Add(ruleName);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);
                ruleName.Name = "Updated Rule Name";

                // Act
                var result = await repository.UpdateRuleNameAsync(ruleName);

                // Assert
                Assert.Equal("Updated Rule Name", result.Name);

                var updatedRuleName = await context.RuleNames.FindAsync(ruleName.Id);
                Assert.Equal("Updated Rule Name", updatedRuleName.Name);
            }
        }

        [Fact]
        public async Task DeleteRuleNameAsync_WithExistingRuleName_ShouldReturnTrue()
        {
            // Arrange
            var ruleName = CreateValidRuleName();
            using (var context = new LocalDbContext(_options))
            {
                context.RuleNames.Add(ruleName);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);

                // Act
                var result = await repository.DeleteRuleNameAsync(ruleName.Id);

                // Assert
                Assert.True(result);
                var deletedRuleName = await context.RuleNames.FindAsync(ruleName.Id);
                Assert.Null(deletedRuleName);
            }
        }

        [Fact]
        public async Task DeleteRuleNameAsync_WithNonExistingRuleName_ShouldReturnFalse()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RuleRepository(context);
                var result = await repository.DeleteRuleNameAsync(999);
                Assert.False(result);
            }
        }
    }
}
