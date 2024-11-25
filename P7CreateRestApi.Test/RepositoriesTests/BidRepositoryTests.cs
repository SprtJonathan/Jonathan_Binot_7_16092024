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
    public class BidRepositoryTests
    {
        private readonly DbContextOptions<LocalDbContext> _options;

        public BidRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private BidList CreateValidBid(int id = 1)
        {
            return new BidList
            {
                BidListId = id,
                Account = "TestAccount",
                BidType = "TestType",
                BidQuantity = 100.0,
                AskQuantity = 50.0,
                Bid = 10.0,
                Ask = 11.0,
                Benchmark = "TestBenchmark",
                BidListDate = DateTime.Now,
                Commentary = "Test Commentary",
                BidSecurity = "TestSecurity",
                BidStatus = "Active",
                Trader = "TestTrader",
                Book = "TestBook",
                CreationName = "TestCreator",
                CreationDate = DateTime.Now,
                RevisionName = "TestRevisor",
                RevisionDate = DateTime.Now,
                DealName = "TestDeal",
                DealType = "Buy",
                SourceListId = "SRC001",
                Side = "Buy"
            };
        }

        [Fact]
        public async Task CreateBidAsync_WithValidBid_ShouldAddBidToContext()
        {
            // Arrange
            var bid = CreateValidBid();
            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);

                // Act
                var result = await repository.CreateBidAsync(bid);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(bid.BidListId, result.BidListId);

                var savedBid = await context.BidLists.FindAsync(bid.BidListId);
                Assert.NotNull(savedBid);
                Assert.Equal(bid.Account, savedBid.Account);
            }
        }

        [Fact]
        public async Task CreateBidAsync_ShouldThrowException_WhenBidIsInvalid()
        {
            // Arrange
            var invalidBid = new BidList(); // BidList sans les champs requis

            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);

                // Act & Assert
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateBidAsync(invalidBid));
            }
        }

        [Fact]
        public async Task GetBidByIdAsync_WithExistingId_ShouldReturnBid()
        {
            // Arrange
            var bid = CreateValidBid();
            using (var context = new LocalDbContext(_options))
            {
                context.BidLists.Add(bid);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);
                var result = await repository.GetBidByIdAsync(bid.BidListId);

                Assert.NotNull(result);
                Assert.Equal(bid.BidListId, result.BidListId);
            }
        }

        [Fact]
        public async Task GetBidByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);
                var result = await repository.GetBidByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllBidsAsync_ShouldReturnAllBids()
        {
            // Arrange
            var bids = new List<BidList>
            {
                CreateValidBid(1),
                CreateValidBid(2)
            };

            using (var context = new LocalDbContext(_options))
            {
                await context.BidLists.AddRangeAsync(bids);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);
                var results = await repository.GetAllBidsAsync();

                var resultList = results.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.Equal(bids[0].BidListId, resultList[0].BidListId);
                Assert.Equal(bids[1].BidListId, resultList[1].BidListId);
            }
        }

        [Fact]
        public async Task GetAllBidsAsync_ShouldReturnEmptyList_WhenNoBidsExist()
        {
            // Arrange
            using (var context = new LocalDbContext(_options))
            {
                // Aucun ajout de données au contexte
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);
                var results = await repository.GetAllBidsAsync();

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        [Fact]
        public async Task UpdateBidAsync_WithValidBid_ShouldUpdateAndReturnBid()
        {
            // Arrange
            var bid = CreateValidBid();
            using (var context = new LocalDbContext(_options))
            {
                context.BidLists.Add(bid);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);
                bid.Commentary = "Updated Commentary";

                // Act
                var result = await repository.UpdateBidAsync(bid);

                // Assert
                Assert.Equal("Updated Commentary", result.Commentary);

                var updatedBid = await context.BidLists.FindAsync(bid.BidListId);
                Assert.Equal("Updated Commentary", updatedBid.Commentary);
            }
        }

        [Fact]
        public async Task DeleteBidAsync_WithExistingBid_ShouldReturnTrue()
        {
            // Arrange
            var bid = CreateValidBid();
            using (var context = new LocalDbContext(_options))
            {
                context.BidLists.Add(bid);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);

                // Act
                var result = await repository.DeleteBidAsync(bid.BidListId);

                // Assert
                Assert.True(result);
                var deletedBid = await context.BidLists.FindAsync(bid.BidListId);
                Assert.Null(deletedBid);
            }
        }

        [Fact]
        public async Task DeleteBidAsync_WithNonExistingBid_ShouldReturnFalse()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new BidRepository(context);
                var result = await repository.DeleteBidAsync(999);
                Assert.False(result);
            }
        }
    }
}
