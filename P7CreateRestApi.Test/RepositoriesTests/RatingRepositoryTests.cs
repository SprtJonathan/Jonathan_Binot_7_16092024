using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;
using P7CreateRestApi.Repositories;

namespace P7CreateRestApi.Tests.RepositoriesTests
{
    public class RatingRepositoryTests
    {
        private readonly DbContextOptions<LocalDbContext> _options;

        public RatingRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private Rating CreateValidRating(int id = 1)
        {
            return new Rating
            {
                Id = id,
                MoodysRating = "Aaa",
                SandPRating = "AAA",
                FitchRating = "AAA",
                OrderNumber = 1
            };
        }

        [Fact]
        public async Task CreateRatingAsync_WithValidRating_ShouldAddRatingToContext()
        {
            // Arrange
            var rating = CreateValidRating();
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);

                // Act
                var result = await repository.CreateRatingAsync(rating);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(rating.Id, result.Id);

                var savedRating = await context.Ratings.FindAsync(rating.Id);
                Assert.NotNull(savedRating);
                Assert.Equal(rating.MoodysRating, savedRating.MoodysRating);
            }
        }

        [Fact]
        public async Task CreateRatingAsync_ShouldThrowException_WhenRatingIsInvalid()
        {
            // Arrange
            var invalidRating = new Rating(); // Rating sans les champs requis

            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);

                // Act & Assert
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateRatingAsync(invalidRating));
            }
        }

        [Fact]
        public async Task GetRatingByIdAsync_WithExistingId_ShouldReturnRating()
        {
            // Arrange
            var rating = CreateValidRating();
            using (var context = new LocalDbContext(_options))
            {
                context.Ratings.Add(rating);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);
                var result = await repository.GetRatingByIdAsync(rating.Id);

                Assert.NotNull(result);
                Assert.Equal(rating.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetRatingByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);
                var result = await repository.GetRatingByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllRatingsAsync_ShouldReturnAllRatings()
        {
            // Arrange
            var ratings = new List<Rating>
            {
                CreateValidRating(1),
                CreateValidRating(2)
            };

            using (var context = new LocalDbContext(_options))
            {
                await context.Ratings.AddRangeAsync(ratings);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);
                var results = await repository.GetAllRatingsAsync();

                var resultList = results.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.Equal(ratings[0].Id, resultList[0].Id);
                Assert.Equal(ratings[1].Id, resultList[1].Id);
            }
        }

        [Fact]
        public async Task GetAllRatingsAsync_ShouldReturnEmptyList_WhenNoRatingsExist()
        {
            // Arrange
            using (var context = new LocalDbContext(_options))
            {
                // Aucun ajout de données au contexte
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);
                var results = await repository.GetAllRatingsAsync();

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        [Fact]
        public async Task UpdateRatingAsync_WithValidRating_ShouldUpdateAndReturnRating()
        {
            // Arrange
            var rating = CreateValidRating();
            using (var context = new LocalDbContext(_options))
            {
                context.Ratings.Add(rating);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);
                rating.MoodysRating = "Baa";

                // Act
                var result = await repository.UpdateRatingAsync(rating);

                // Assert
                Assert.Equal("Baa", result.MoodysRating);

                var updatedRating = await context.Ratings.FindAsync(rating.Id);
                Assert.Equal("Baa", updatedRating.MoodysRating);
            }
        }

        [Fact]
        public async Task DeleteRatingAsync_WithExistingRating_ShouldReturnTrue()
        {
            // Arrange
            var rating = CreateValidRating();
            using (var context = new LocalDbContext(_options))
            {
                context.Ratings.Add(rating);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);

                // Act
                var result = await repository.DeleteRatingAsync(rating.Id);

                // Assert
                Assert.True(result);
                var deletedRating = await context.Ratings.FindAsync(rating.Id);
                Assert.Null(deletedRating);
            }
        }

        [Fact]
        public async Task DeleteRatingAsync_WithNonExistingRating_ShouldReturnFalse()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new RatingRepository(context);
                var result = await repository.DeleteRatingAsync(999);
                Assert.False(result);
            }
        }
    }
}
