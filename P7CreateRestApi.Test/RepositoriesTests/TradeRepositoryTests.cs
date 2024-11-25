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
    public class TradeRepositoryTests
    {
        private readonly DbContextOptions<LocalDbContext> _options;

        public TradeRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<LocalDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private Trade CreateValidTrade(int id = 1)
        {
            return new Trade
            {
                TradeId = id,
                Account = "TestAccount",
                AccountType = "TestAccountType",
                BuyQuantity = 100.0,
                SellQuantity = 50.0,
                BuyPrice = 10.0,
                SellPrice = 20.0,
                TradeDate = DateTime.Now,
                TradeSecurity = "TestSecurity",
                TradeStatus = "Active",
                Trader = "TestTrader",
                Benchmark = "TestBenchmark",
                Book = "TestBook",
                CreationName = "Creator",
                CreationDate = DateTime.Now,
                RevisionName = "Revisor",
                RevisionDate = DateTime.Now,
                DealName = "TestDeal",
                DealType = "Buy",
                SourceListId = "SRC001",
                Side = "Buy"
            };
        }

        [Fact]
        public async Task CreateTradeAsync_WithValidTrade_ShouldAddTradeToContext()
        {
            // Arrange
            var trade = CreateValidTrade();
            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);

                // Act
                var result = await repository.CreateTradeAsync(trade);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(trade.TradeId, result.TradeId);

                var savedTrade = await context.Trades.FindAsync(trade.TradeId);
                Assert.NotNull(savedTrade);
                Assert.Equal(trade.Account, savedTrade.Account);
            }
        }

        [Fact]
        public async Task CreateTradeAsync_ShouldThrowException_WhenTradeIsInvalid()
        {
            // Arrange
            var invalidTrade = new Trade(); // Trade sans les champs requis

            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);

                // Act & Assert
                await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.CreateTradeAsync(invalidTrade));
            }
        }

        [Fact]
        public async Task GetTradeByIdAsync_WithExistingId_ShouldReturnTrade()
        {
            // Arrange
            var trade = CreateValidTrade();
            using (var context = new LocalDbContext(_options))
            {
                context.Trades.Add(trade);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);
                var result = await repository.GetTradeByIdAsync(trade.TradeId);

                Assert.NotNull(result);
                Assert.Equal(trade.TradeId, result.TradeId);
            }
        }

        [Fact]
        public async Task GetTradeByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);
                var result = await repository.GetTradeByIdAsync(999);
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetAllTradesAsync_ShouldReturnAllTrades()
        {
            // Arrange
            var trades = new List<Trade>
            {
                CreateValidTrade(1),
                CreateValidTrade(2)
            };

            using (var context = new LocalDbContext(_options))
            {
                await context.Trades.AddRangeAsync(trades);
                await context.SaveChangesAsync();
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);
                var results = await repository.GetAllTradesAsync();

                var resultList = results.ToList();
                Assert.Equal(2, resultList.Count);
                Assert.Equal(trades[0].TradeId, resultList[0].TradeId);
                Assert.Equal(trades[1].TradeId, resultList[1].TradeId);
            }
        }

        [Fact]
        public async Task GetAllTradesAsync_ShouldReturnEmptyList_WhenNoTradesExist()
        {
            // Arrange
            using (var context = new LocalDbContext(_options))
            {
                // Aucun ajout de données au contexte
            }

            // Act & Assert
            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);
                var results = await repository.GetAllTradesAsync();

                // Assert
                Assert.NotNull(results);
                Assert.Empty(results);
            }
        }

        [Fact]
        public async Task UpdateTradeAsync_WithValidTrade_ShouldUpdateAndReturnTrade()
        {
            // Arrange
            var trade = CreateValidTrade();
            using (var context = new LocalDbContext(_options))
            {
                context.Trades.Add(trade);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);
                trade.Account = "UpdatedAccount";

                // Act
                var result = await repository.UpdateTradeAsync(trade);

                // Assert
                Assert.Equal("UpdatedAccount", result.Account);

                var updatedTrade = await context.Trades.FindAsync(trade.TradeId);
                Assert.Equal("UpdatedAccount", updatedTrade.Account);
            }
        }

        [Fact]
        public async Task DeleteTradeAsync_WithExistingTrade_ShouldReturnTrue()
        {
            // Arrange
            var trade = CreateValidTrade();
            using (var context = new LocalDbContext(_options))
            {
                context.Trades.Add(trade);
                await context.SaveChangesAsync();
            }

            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);

                // Act
                var result = await repository.DeleteTradeAsync(trade.TradeId);

                // Assert
                Assert.True(result);
                var deletedTrade = await context.Trades.FindAsync(trade.TradeId);
                Assert.Null(deletedTrade);
            }
        }

        [Fact]
        public async Task DeleteTradeAsync_WithNonExistingTrade_ShouldReturnFalse()
        {
            using (var context = new LocalDbContext(_options))
            {
                var repository = new TradeRepository(context);
                var result = await repository.DeleteTradeAsync(999);
                Assert.False(result);
            }
        }
    }
}
