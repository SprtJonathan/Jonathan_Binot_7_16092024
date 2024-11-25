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
    public class UserRepositoryTests
    {
        private readonly DbContextOptions<LocalDbContext> _options;

        public UserRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private User CreateValidUser(int id = 1)
        {
            return new User
            {
                Id = id,
                Fullname = "Test User",
                UserName = "testuser",
                Role = "User",
                Email = "testuser@example.com",
                NormalizedEmail = "TESTUSER@EXAMPLE.COM",
                PasswordHash = "hashedpassword",
                SecurityStamp = Guid.NewGuid().ToString()
            };
        }

        [Fact]
        public async Task CreateUserAsync_WithValidUser_ShouldAddUserToContext()
        {
            // Arrange
            var user = CreateValidUser();
            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.CreateUserAsync(user);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(user.Id, result.Id);

                var savedUser = await context.Users.FindAsync(user.Id);
                Assert.NotNull(savedUser);
                Assert.Equal(user.Fullname, savedUser.Fullname);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_WithExistingId_ShouldReturnUser()
        {
            // Arrange
            var user = CreateValidUser();
            using (var context = new LocalDbContext(_options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetUserByIdAsync(user.Id);

                Assert.NotNull(result);
                Assert.Equal(user.Id, result.Id);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetUserByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                CreateValidUser(1),
                CreateValidUser(2)
            };

            using (var context = new LocalDbContext(_options))
            {
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);
                var results = await repository.GetAllUsersAsync();

                var resultList = results.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.Equal(users[0].Id, resultList[0].Id);
                Assert.Equal(users[1].Id, resultList[1].Id);
            }
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange
            using (var context = new LocalDbContext(_options))
            {
                // Aucun ajout de données au contexte
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);
                var results = await repository.GetAllUsersAsync();

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        [Fact]
        public async Task UpdateUserAsync_WithValidUser_ShouldUpdateAndReturnUser()
        {
            // Arrange
            var user = CreateValidUser();
            using (var context = new LocalDbContext(_options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);
                user.Fullname = "Updated Name";

                // Act
                var result = await repository.UpdateUserAsync(user);

                // Assert
                Assert.Equal("Updated Name", result.Fullname);

                var updatedUser = await context.Users.FindAsync(user.Id);
                Assert.Equal("Updated Name", updatedUser.Fullname);
            }
        }

        [Fact]
        public async Task DeleteUserAsync_WithExistingUser_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateValidUser();
            using (var context = new LocalDbContext(_options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.DeleteUserAsync(user.Id);

                // Assert
                Assert.True(result);
                var deletedUser = await context.Users.FindAsync(user.Id);
                Assert.Null(deletedUser);
            }
        }

        [Fact]
        public async Task DeleteUserAsync_WithNonExistingUser_ShouldReturnFalse()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new UserRepository(context);
                var result = await repository.DeleteUserAsync(999);
                Assert.False(result);
            }
        }
    }
}
