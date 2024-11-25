using Microsoft.EntityFrameworkCore;
using Moq;
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
    public class CurvePointRepositoryTests
    {
        private readonly DbContextOptions<LocalDbContext> _options;

        public CurvePointRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private CurvePoint CreateValidCurvePoint(int id = 1)
        {
            return new CurvePoint
            {
                Id = id,
                CurveId = 1,
                AsOfDate = new DateTime(2024, 1, 1),
                Term = 5.5,
                CurvePointValue = 10.0,
                CreationDate = DateTime.Now
            };
        }

        [Fact]
        public async Task CreateCurvePointAsync_WithValidCurvePoint_ShouldAddCurvePointToContext()
        {
            // Arrange
            var curvePoint = CreateValidCurvePoint();
            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);

                // Act
                var result = await repository.CreateCurvePointAsync(curvePoint);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(curvePoint.Id, result.Id);

                var savedCurvePoint = await context.CurvePoints.FindAsync(curvePoint.Id);
                Assert.NotNull(savedCurvePoint);
                Assert.Equal(curvePoint.CurvePointValue, savedCurvePoint.CurvePointValue);
            }
        }

        [Fact]
        public async Task GetCurvePointByIdAsync_WithExistingId_ShouldReturnCurvePoint()
        {
            // Arrange
            var curvePoint = CreateValidCurvePoint();
            using (var context = new LocalDbContext(_options))
            {
                context.CurvePoints.Add(curvePoint);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);
                var result = await repository.GetCurvePointByIdAsync(curvePoint.Id);

                Assert.NotNull(result);
                Assert.Equal(curvePoint.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetCurvePointByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);
                var result = await repository.GetCurvePointByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllCurvePointsAsync_ShouldReturnAllCurvePoints()
        {
            // Arrange
            var curvePoints = new List<CurvePoint>
            {
                CreateValidCurvePoint(1),
                CreateValidCurvePoint(2)
            };

            using (var context = new LocalDbContext(_options))
            {
                await context.CurvePoints.AddRangeAsync(curvePoints);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);
                var results = await repository.GetAllCurvePointsAsync();

                var resultList = results.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.Equal(curvePoints[0].Id, resultList[0].Id);
                Assert.Equal(curvePoints[1].Id, resultList[1].Id);
            }
        }

        [Fact]
        public async Task GetAllCurvePointsAsync_ShouldReturnEmptyList_WhenNoCurvePointsExist()
        {
            // Arrange
            using (var context = new LocalDbContext(_options))
            {
                // Aucun ajout de données au contexte
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);
                var results = await repository.GetAllCurvePointsAsync();

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        [Fact]
        public async Task UpdateCurvePointAsync_WithValidCurvePoint_ShouldUpdateAndReturnCurvePoint()
        {
            // Arrange
            var curvePoint = CreateValidCurvePoint();
            using (var context = new LocalDbContext(_options))
            {
                context.CurvePoints.Add(curvePoint);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);
                curvePoint.CurvePointValue = 20.0;

                // Act
                var result = await repository.UpdateCurvePointAsync(curvePoint);

                // Assert
                Assert.Equal(20.0, result.CurvePointValue);

                var updatedCurvePoint = await context.CurvePoints.FindAsync(curvePoint.Id);
                Assert.Equal(20.0, updatedCurvePoint.CurvePointValue);
            }
        }

        [Fact]
        public async Task DeleteCurvePointAsync_WithExistingCurvePoint_ShouldReturnTrue()
        {
            // Arrange
            var curvePoint = CreateValidCurvePoint();
            using (var context = new LocalDbContext(_options))
            {
                context.CurvePoints.Add(curvePoint);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);

                // Act
                var result = await repository.DeleteCurvePointAsync(curvePoint.Id);

                // Assert
                Assert.True(result);
                var deletedCurvePoint = await context.CurvePoints.FindAsync(curvePoint.Id);
                Assert.Null(deletedCurvePoint);
            }
        }

        [Fact]
        public async Task DeleteCurvePointAsync_WithNonExistingCurvePoint_ShouldReturnFalse()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new CurvePointRepository(context);
                var result = await repository.DeleteCurvePointAsync(999);
                Assert.False(result);
            }
        }
    }
}
